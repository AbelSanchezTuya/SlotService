using System.Net;
using System.Net.Http.Headers;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Newtonsoft.Json;
using SlotService.Domain;
using SlotService.Integration.Common;
using SlotService.Storage;
using ResponseWeekSchedule = SlotService.API.REST.Model.WeekSchedule;
using ResponseDaySchedule = SlotService.API.REST.Model.DaySchedule;


namespace SlotService.Integration;

[FeatureFixture]
public class GetWeeklyAvailabilityFeature
    : AvailabilityFeatureBase
{
    private const int DefaultSlotDuration = 60;
    private const string FacilityName = "Hobbiton";
    private const string FacilityAddress = "501 Buckland Road, Matamata 3472, New Zeland";
    private HttpResponseMessage _response = new();
    private ResponseWeekSchedule? _responseObject;
    private WeekSchedule? _weekSchedule;
    private WorkPeriod? _workPeriod;

    public GetWeeklyAvailabilityFeature(TestApplicationFactory factory): base(factory)
    {
        InMemoryAgendaRepository.Instance.Clear();
    }

    [Scenario]
    public void A_date_in_the_past_does_not_have_schedules_available()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => When_requesting_availability_for_a_week_ago(),
            _ => Then_it_is_a_bad_request_with_error_containing("in the past"));
    }

    [Scenario]
    public void A_wrong_date_format()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => When_requesting_with_invalid_date_format(),
            _ => Then_it_is_a_bad_request_with_error_containing("Invalid date format"));
    }

    [Scenario]
    public void A_date_in_the_future_without_schedules_available()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_no_week_schedules_in_future(),
            _ => When_requesting_availability_two_weeks_in_the_future(),
            _ => Then_it_is_a_bad_request_with_error_containing("is not available"));
    }

    [Scenario]
    public void A_date_in_current_week_with_schedule()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_a_week_schedule_for_current_week(),
            _ => And_a_work_period(
                9,
                14,
                15,
                16),
            _ => And_the_week_schedule_has_available(DayOfWeek.Monday),
            _ => And_the_week_schedule_has_available(DayOfWeek.Thursday),
            _ => And_there_is_a_busy_slot_on(DayOfWeek.Thursday, 10, 11),
            _ => When_requesting_availability_for_this_week(),
            _ => Then_the_week_schedule_is_returned(),
            _ => And_contains_the_available_day_schedules());
    }

    [Scenario]
    public void A_date_in_next_week_with_schedule()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_a_week_schedule_in_two_weeks(),
            _ => And_a_work_period(
                9,
                13,
                17,
                19),
            _ => And_the_week_schedule_has_available(DayOfWeek.Monday),
            _ => And_the_week_schedule_has_available(DayOfWeek.Thursday),
            _ => And_there_is_a_busy_slot_on(DayOfWeek.Thursday, 17, 18),
            _ => When_requesting_availability_in_two_weeks(),
            _ => Then_the_week_schedule_is_returned(),
            _ => And_contains_the_available_day_schedules());
    }

    private void Given_an_authorized_user()
    {
        // Set authorization header with valid credentials
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String("techuser:secretpassWord"u8.ToArray()));
    }

    private void And_a_week_schedule_in_two_weeks()
    {
        var facility = new Facility
                       {
                           Name = FacilityName,
                           Address = FacilityAddress
                       };
        _weekSchedule = new WeekSchedule(
            DateOnly.FromDateTime(DateTime.Today.AddDays(14)),
            DefaultSlotDuration,
            facility);
        InMemoryAgendaRepository.Instance.AddWeekSchedule(_weekSchedule);
    }

    private void And_there_is_a_busy_slot_on(DayOfWeek weekDay, int start, int end)
    {
        if (_weekSchedule == null)
        {
            Assert.Fail("Has to set first Week schedule");
        }

        var daysToAdd = weekDay - _weekSchedule.FistDay.DayOfWeek;
        if (daysToAdd < 0)
        {
            daysToAdd += 7;
        }

        var slot = new Slot(
            _weekSchedule.FistDay.AddDays(daysToAdd),
            new TimeOnly(start, 0),
            new TimeOnly(end, 0));

        if (_weekSchedule.CanAccomodate(slot))
        {
            _weekSchedule.Book(slot);
        }
    }

    private void And_a_work_period(
        int start,
        int lunchStart,
        int lunchEnd,
        int end)
    {
        _workPeriod = new WorkPeriod
                      {
                          MorningShift = new Shift(
                              new TimeOnly(start, 0),
                              new TimeOnly(lunchStart, 0)),
                          AfternoonShift = new Shift(
                              new TimeOnly(lunchEnd, 0),
                              new TimeOnly(end, 0))
                      };
    }

    private void And_the_week_schedule_has_available(DayOfWeek weekDay)
    {
        if (_workPeriod == null)
        {
            Assert.Fail("Should set work period before to create week schedule");
        }
        if (_weekSchedule == null)
        {
            Assert.Fail("Should set a week schedule to open a day schedule");
        }

        _weekSchedule.OpenDaySchedule(weekDay, _workPeriod);
    }

    private void And_a_week_schedule_for_current_week()
    {
        var facility = new Facility
                       {
                           Name = FacilityName,
                           Address = FacilityAddress
                       };
        _weekSchedule = new WeekSchedule(
            DateOnly.FromDateTime(DateTime.Today),
            DefaultSlotDuration,
            facility);
        InMemoryAgendaRepository.Instance.AddWeekSchedule(_weekSchedule);
    }

    private void And_no_week_schedules_in_future()
    {
        // Nothing to do
    }

    private void When_requesting_availability_two_weeks_in_the_future()
    {
        When_requesting_availability_in(14);
    }

    private void When_requesting_availability_for_a_week_ago()
    {
        When_requesting_availability_in(-7);
    }

    private void When_requesting_availability_for_this_week()
    {
        When_requesting_availability_in(0);
    }

    private void When_requesting_availability_in_two_weeks()
    {
        When_requesting_availability_in(14);
    }

    private void When_requesting_availability_in(int days)
    {
        var url = $"{GetWeeklyAvailabilityUrl}/{DateTime.Now.AddDays(days):yyyyMMdd}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        _response = Client.SendAsync(request).Result;
    }

    private void When_requesting_with_invalid_date_format()
    {
        var url = $"{GetWeeklyAvailabilityUrl}/23072024";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        _response = Client.SendAsync(request).Result;
    }

    private void Then_it_is_a_bad_request_with_error_containing(string messageContains)
    {
        Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
        Assert.NotNull(_response.ReasonPhrase);
        var error = _response.Content.ReadAsStringAsync().Result;
        Assert.True(error.Contains(messageContains));
    }

    private void Then_the_week_schedule_is_returned()
    {
        var jsonResponse = _response.Content.ReadAsStringAsync().Result;
        _responseObject = JsonConvert.DeserializeObject<ResponseWeekSchedule>(jsonResponse);
        Assert.NotNull(_responseObject);
    }

    private void And_contains_the_available_day_schedules()
    {
        if (_responseObject == null)
        {
            Assert.Fail("there is no response to verify");
        }
        if (_weekSchedule == null)
        {
            Assert.Fail("requires the week schedule information");
        }
        if (_workPeriod == null)
        {
            Assert.Fail("requires the work period information");
        }
        foreach (var daySchedule in _weekSchedule.GetAvailability())
        {
            var responseDay
                = GetPropertyValue(_responseObject, daySchedule.DayOfWeek.ToString()) as
                      ResponseDaySchedule;
            Assert.NotNull(responseDay);
            Assert.Equal(responseDay.WorkPeriod.StartHour, _workPeriod.MorningShift.Start.Hour);
            Assert.Equal(responseDay.WorkPeriod.LunchStartHour, _workPeriod.MorningShift.End.Hour);
            Assert.Equal(
                responseDay.WorkPeriod.LunchEndHour,
                _workPeriod.AfternoonShift.Start.Hour);
            Assert.Equal(responseDay.WorkPeriod.EndHour, _workPeriod.AfternoonShift.End.Hour);
            Assert.Equal(responseDay.BusySlots.Count, daySchedule.BusySlots.Count);
            foreach (var slot in daySchedule.BusySlots)
            {
                Assert.Contains(
                    responseDay.BusySlots,
                    x => x.Start.Equals(ConvertToString(slot.Date, slot.Start)));
                Assert.Contains(
                    responseDay.BusySlots,
                    x => x.End.Equals(ConvertToString(slot.Date, slot.End)));
            }
        }
    }

    private string ConvertToString(DateOnly date, TimeOnly time)
    {
        return $"{date.ToString("yyyy-MM-dd")}T{time.ToString("HH:mm:ss")}";
    }

    public static object? GetPropertyValue(object obj, string propertyName)
    {
        var type = obj.GetType();
        var propertyInfo = type.GetProperty(propertyName);

        return propertyInfo != null ? propertyInfo.GetValue(obj) : null;
    }
}
