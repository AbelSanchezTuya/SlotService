using Moq;
using SlotService.Application.API;
using SlotService.Application.Mappers;
using SlotService.Domain;
using Slot = SlotService.Application.API.Dtos.Slot;


namespace SlotService.Application.UnitTests.Mappers;

[TestFixture]
public class GetWeekAvailabilityResponseMapperTest
{
    [SetUp]
    public void SetUp()
    {
        _mapper = new GetWeekAvailabilityResponseMapper();
        _facility = new Facility
                    {
                        Name = "test",
                        Address = "addressTest"
                    };
        _workPeriod = new WorkPeriod
                      {
                          MorningShift = new Shift(TimeOnly.MinValue, TimeOnly.MaxValue),
                          AfternoonShift = new Shift(TimeOnly.MinValue, TimeOnly.MaxValue)
                      };
        _weekScheduleMock = new Mock<IWeekSchedule>();
        _weekScheduleMock.SetupGet(x => x.Facility)
                         .Returns(_facility);
        _weekScheduleMock.SetupGet(x => x.SlotDurationMinutes)
                         .Returns(SlotDurationMinutes);
        _daysSchedule =
        [
            new DaySchedule
            {
                WorkPeriod = _workPeriod,
                DayOfWeek = DayOfWeek.Monday
            },
            new DaySchedule
            {
                WorkPeriod = _workPeriod,
                DayOfWeek = DayOfWeek.Thursday,
                BusySlots =
                {
                    new Domain.Slot(
                        DateOnly.MinValue,
                        TimeOnly.MinValue,
                        TimeOnly.MaxValue)
                }
            }
        ];
        _weekScheduleMock.Setup(x => x.GetAvailability())
                         .Returns(_daysSchedule);
    }

    private GetWeekAvailabilityResponseMapper _mapper;
    private Mock<IWeekSchedule> _weekScheduleMock;
    private List<DaySchedule> _daysSchedule;
    private Facility _facility;
    private WorkPeriod _workPeriod;
    private const int SlotDurationMinutes = 5;

    [Test]
    public void Map_WithArguments_Maps()
    {
        // Act
        var response = _mapper.Map(_weekScheduleMock.Object);

        // Assert
        Assert.IsNotNull(response);
        Assert.NotNull(response.Facility);
        Assert.That(response.Facility.Name, Is.EqualTo(_facility.Name));
        Assert.That(response.Facility.Address, Is.EqualTo(_facility.Address));
        Assert.That(response.SlotDurationMinutes, Is.EqualTo(SlotDurationMinutes));
        ValidateDay(
            response,
            DayOfWeek.Monday,
            true,
            0);
        ValidateDay(
            response,
            DayOfWeek.Tuesday,
            false,
            0);
        ValidateDay(
            response,
            DayOfWeek.Wednesday,
            false,
            0);
        ValidateDay(
            response,
            DayOfWeek.Thursday,
            true,
            1);
        ValidateDay(
            response,
            DayOfWeek.Friday,
            false,
            0);
    }

    private void ValidateDay(
        GetWeekAvailabilityResponse response,
        DayOfWeek day,
        bool exists,
        int expectedSlots)
    {
        var weekDay = response.DaysSchedule.FirstOrDefault(x => x.Name == day);
        if (exists)
        {
            Assert.IsNotNull(weekDay);
        }
        else
        {
            Assert.IsNull(weekDay);

            return;
        }

        ValidateWorkPeriod(weekDay!.WorkPeriod);
        ValidateBusySlots(weekDay.BusySlots, expectedSlots);
    }

    private void ValidateBusySlots(
        List<Slot> slots,
        int expectedBusySlots)
    {
        Assert.That(slots.Count, Is.EqualTo(expectedBusySlots));
        foreach (var slot in slots)
        {
            Assert.That(slot.Start, Is.EqualTo(TimeOnly.MinValue));
            Assert.That(slot.End, Is.EqualTo(TimeOnly.MaxValue));
        }
    }

    private void ValidateWorkPeriod(Application.API.Dtos.WorkPeriod workPeriod)
    {
        Assert.NotNull(workPeriod);
        Assert.That(workPeriod.StartHour, Is.EqualTo(_workPeriod.MorningShift.Start));
        Assert.That(workPeriod.LunchStartHour, Is.EqualTo(_workPeriod.MorningShift.End));
        Assert.That(workPeriod.LunchEndHour, Is.EqualTo(_workPeriod.AfternoonShift.Start));
        Assert.That(workPeriod.EndHour, Is.EqualTo(_workPeriod.AfternoonShift.End));
    }
}
