using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using SlotService.Domain;
using SlotService.Integration.Common;
using SlotService.Storage;
using ResponseWeekSchedule = SlotService.API.REST.Model.WeekSchedule;
using ResponseDaySchedule = SlotService.API.REST.Model.DaySchedule;


namespace SlotService.Integration;

[FeatureFixture]
public class TakeSlotFeature
    : AvailabilityFeatureBase
{
    private const int DefaultSlotDuration = 60;
    private const string FacilityName = "Hobbiton";
    private const string FacilityAddress = "501 Buckland Road, Matamata 3472, New Zeland";
    private Dictionary<string, object?> _appointmentData = [];
    private HttpResponseMessage _response = new();
    private ResponseWeekSchedule? _responseObjec;
    private WeekSchedule? _weekSchedule;
    private WorkPeriod? _workPeriod;
    public string AppointmentDateTimeFormat => "yyyy-MM-dd HH:mm:ss";

    public TakeSlotFeature(TestApplicationFactory factory) : base(factory)
    {
        InMemoryAgendaRepository.Instance.Clear();
    }

    [Scenario]
    public void Try_to_book_an_appointment_without_data()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_no_available_data(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing(
                "One or more validation errors occurred"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_with_days_switched()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_with_days_switched(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("cannot be after end"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_with_wrong_start_date_format()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_with_wrong_start_date_format(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("Invalid date format"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_with_wrong_end_date_format()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_with_wrong_end_date_format(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("Invalid date format"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_without_patient()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_without_patient(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("The Patient field is required"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_without_patient_name()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_without_patient_name(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("'Name' must not be empty"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_without_patient_second_name()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_without_patient_second_name(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("'Second Name' must not be empty"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_without_patient_email()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_without_patient_email(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("'Email' must not be empty"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_without_patient_phone()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_without_patient_phone(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("'Phone' must not be empty"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_in_the_past()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_in_the_past(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("in the past"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_in_the_future_on_week_without_open_schedule()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_appointment_in_a_future_week_without_open_schedule(),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("is not available"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_on_an_available_slot_on_next_week()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_a_week_schedule_for_next_week(),
            _ => And_a_work_period(
                9,
                13,
                15,
                16),
            _ => And_the_week_schedule_has_available(DayOfWeek.Monday),
            _ => And_the_week_schedule_has_available(DayOfWeek.Thursday),
            _ => And_there_a_busy_slot_exists_on(DayOfWeek.Thursday, 10, 11),
            _ => And_appointment_next_week(DayOfWeek.Thursday, 11, 12),
            _ => When_try_to_book(),
            _ => Then_the_appointment_is_booked_next_week(DayOfWeek.Thursday, 11, 12));
    }

    [Scenario]
    public void Try_to_book_an_appointment_on_overlapping_a_slot_on_next_week()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_a_week_schedule_for_next_week(),
            _ => And_a_work_period(
                9,
                13,
                17,
                19),
            _ => And_the_week_schedule_has_available(DayOfWeek.Monday),
            _ => And_the_week_schedule_has_available(DayOfWeek.Thursday),
            _ => And_there_a_busy_slot_exists_on(DayOfWeek.Thursday, 10, 11),
            _ => And_appointment_next_week(DayOfWeek.Thursday, 10, 11),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("is not available"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_after_another_with_same_data()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_a_week_schedule_for_next_week(),
            _ => And_a_work_period(
                9,
                14,
                15,
                16),
            _ => And_the_week_schedule_has_available(DayOfWeek.Monday),
            _ => And_the_week_schedule_has_available(DayOfWeek.Thursday),
            _ => And_there_a_busy_slot_exists_on(DayOfWeek.Thursday, 10, 11),
            _ => And_appointment_next_week(DayOfWeek.Thursday, 11, 12),
            _ => When_try_to_book(),
            _ => Then_the_appointment_is_booked_next_week(DayOfWeek.Thursday, 11, 12),
            _ => And_appointment_next_week(DayOfWeek.Thursday, 11, 12),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("is not available"));
    }

    [Scenario]
    public void Try_to_book_an_appointment_with_extra_data_next_week()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_a_week_schedule_for_next_week(),
            _ => And_a_work_period(
                9,
                14,
                15,
                16),
            _ => And_the_week_schedule_has_available(DayOfWeek.Monday),
            _ => And_the_week_schedule_has_available(DayOfWeek.Thursday),
            _ => And_there_a_busy_slot_exists_on(DayOfWeek.Thursday, 10, 11),
            _ => And_appointment_with_not_predefined_data(DayOfWeek.Thursday, 11, 12),
            _ => When_try_to_book(),
            _ => Then_the_appointment_is_booked_next_week(DayOfWeek.Thursday, 11, 12));
    }

    [Scenario]
    public void Try_to_book_an_appointment_with_wrong_slot_duration()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => And_a_week_schedule_for_next_week(),
            _ => And_a_work_period(
                9,
                14,
                15,
                16),
            _ => And_the_week_schedule_has_available(DayOfWeek.Monday),
            _ => And_the_week_schedule_has_available(DayOfWeek.Thursday),
            _ => And_there_a_busy_slot_exists_on(DayOfWeek.Thursday, 10, 11),
            _ => And_appointment_next_week(DayOfWeek.Thursday, 11, 13),
            _ => When_try_to_book(),
            _ => Then_it_is_a_bad_request_with_error_containing("is not available"));
    }

    private void And_appointment_with_days_switched()
    {
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", DateTime.Now.ToString(AppointmentDateTimeFormat) },
                               {
                                   "End",
                                   DateTime.Now.AddHours(-1).ToString(AppointmentDateTimeFormat)
                               },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_with_wrong_start_date_format()
    {
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", DateTime.Now.ToString("yyyyMMdd") },
                               {
                                   "End",
                                   DateTime.Now.AddHours(-1).ToString(AppointmentDateTimeFormat)
                               },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_with_wrong_end_date_format()
    {
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", DateTime.Now.ToString(AppointmentDateTimeFormat) },
                               { "End", DateTime.Now.AddHours(-1).ToString("yyyyMMdd") },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_without_patient()
    {
        _appointmentData = new Dictionary<string, object>
                           {
                               { "Start", DateTime.Now.ToString(AppointmentDateTimeFormat) },
                               {
                                   "End",
                                   DateTime.Now.AddHours(1).ToString(AppointmentDateTimeFormat)
                               },
                               { "Comments", "" },
                               { "Patient", null }
                           };
    }

    private void And_appointment_without_patient_name()
    {
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", DateTime.Now.ToString(AppointmentDateTimeFormat) },
                               {
                                   "End",
                                   DateTime.Now.AddHours(1).ToString(AppointmentDateTimeFormat)
                               },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_without_patient_second_name()
    {
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", DateTime.Now.ToString(AppointmentDateTimeFormat) },
                               {
                                   "End",
                                   DateTime.Now.AddHours(1).ToString(AppointmentDateTimeFormat)
                               },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  { "SecondName", "" },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_without_patient_email()
    {
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", DateTime.Now.ToString(AppointmentDateTimeFormat) },
                               {
                                   "End",
                                   DateTime.Now.AddHours(1).ToString(AppointmentDateTimeFormat)
                               },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  { "Email", "" },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_without_patient_phone()
    {
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", DateTime.Now.ToString(AppointmentDateTimeFormat) },
                               {
                                   "End",
                                   DateTime.Now.AddHours(1).ToString(AppointmentDateTimeFormat)
                               },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "" }
                                              }
                               }
                           };
    }

    private void And_appointment_in_the_past()
    {
        var date = DateTime.Now.AddDays(-10);
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", date.ToString(AppointmentDateTimeFormat) },
                               { "End", date.AddHours(1).ToString(AppointmentDateTimeFormat) },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_in_a_future_week_without_open_schedule()
    {
        var date = DateTime.Now.AddDays(100);
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", date.ToString(AppointmentDateTimeFormat) },
                               { "End", date.AddHours(1).ToString(AppointmentDateTimeFormat) },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_next_week(DayOfWeek dayOfWeek, int start, int end)
    {
        var currentDay = DateOnly.FromDateTime(DateTime.Now);
        var daysUntil = ((int) dayOfWeek - (int) currentDay.DayOfWeek + 7) % 7;
        if (daysUntil == 0)
        {
            daysUntil = 7;
        }
        var date = currentDay.AddDays(daysUntil);
        var startDateTime = date.ToDateTime(new TimeOnly(start, 0));
        var endDateTime = date.ToDateTime(new TimeOnly(end, 0));
        _appointmentData = new Dictionary<string, object?>
                           {
                               { "Start", startDateTime.ToString(AppointmentDateTimeFormat) },
                               { "End", endDateTime.ToString(AppointmentDateTimeFormat) },
                               { "Comments", "" },
                               {
                                   "Patient", new Dictionary<string, string>
                                              {
                                                  { "Name", "Bilbo" },
                                                  {
                                                      "SecondName",
                                                      "Baggings"
                                                  },
                                                  {
                                                      "Email",
                                                      "bb@hobitton.com"
                                                  },
                                                  { "Phone", "233456" }
                                              }
                               }
                           };
    }

    private void And_appointment_with_not_predefined_data(DayOfWeek dayOfWeek, int start, int end)
    {
        And_appointment_next_week(dayOfWeek, start, end);
        _appointmentData["Unknown"] = "Unknown";
    }

    private void Then_it_is_a_bad_request_with_error_containing(string messageContains)
    {
        Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
        Assert.NotNull(_response.ReasonPhrase);
        var error = _response.Content.ReadAsStringAsync().Result;
        Assert.True(error.Contains(messageContains));
    }

    private void Then_the_appointment_is_booked_next_week(DayOfWeek dayOfWeek, int start, int end)
    {
        var date = DateOnly.FromDateTime(DateTime.Now.AddDays(7));
        if (InMemoryAgendaRepository.Instance.TryToGetWeekSchedule(date, out var weekSchedule))
        {
            var availability = weekSchedule?.GetAvailability();

            var daySchedule = availability?.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);

            Assert.NotNull(daySchedule);
            var containsBusySlot = daySchedule.BusySlots.Any(x => x.Start.Hour == start && x.End.Hour == end);
            Assert.True(containsBusySlot);
        }
    }

    private void And_no_available_data()
    {
        _appointmentData.Clear();
    }

    private void When_try_to_book()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, TakeSlotUrl);
        var jsonContent = JsonSerializer.Serialize(_appointmentData);
        if (jsonContent == null)
        {
            Assert.Fail("not possible to convert data");
        }
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        _response = Client.SendAsync(request).Result;
    }

    private void Given_an_authorized_user()
    {
        // Set authorization header with valid credentials
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String("techuser:secretpassWord"u8.ToArray()));
    }

    private void And_there_a_busy_slot_exists_on(DayOfWeek weekDay, int start, int end)
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

        _weekSchedule?.OpenDaySchedule(weekDay, _workPeriod);
    }

    private void And_a_week_schedule_for_next_week()
    {
        var facility = new Facility
                       {
                           Name = FacilityName,
                           Address = FacilityAddress
                       };
        _weekSchedule = new WeekSchedule(
            DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
            DefaultSlotDuration,
            facility);
        InMemoryAgendaRepository.Instance.AddWeekSchedule(_weekSchedule);
    }
}
