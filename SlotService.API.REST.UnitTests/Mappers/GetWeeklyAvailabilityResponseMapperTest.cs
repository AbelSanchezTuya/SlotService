using SlotService.API.REST.Mappers;
using SlotService.Application.API;
using SlotService.Application.API.Dtos;


namespace SlotService.API.REST.UnitTests.Mappers;

[TestFixture]
public class GetWeeklyAvailabilityResponseMapperTests
{
    [SetUp]
    public void SetUp()
    {
        _mapper = new GetWeeklyAvailabilityResponseMapper();
    }

    private GetWeeklyAvailabilityResponseMapper _mapper;

    [Test]
    public void Map_WithMondayDaySchedule_Maps()
    {
        // Arrange
        var response = new GetWeekAvailabilityResponse
                       {
                           Facility = new Facility
                                      {
                                          Name = "Test Facility",
                                          Address = "123 Test Street"
                                      },
                           SlotDurationMinutes = 60
                       };
        response.DaysSchedule.Add(
            new DaySchedule
            {
                Name = DayOfWeek.Monday,
                WorkPeriod
                    = new WorkPeriod
                      {
                          StartHour = new TimeOnly(8, 0, 0),
                          LunchStartHour = new TimeOnly(12, 0, 0),
                          LunchEndHour = new TimeOnly(13, 0, 0),
                          EndHour = new TimeOnly(17, 0, 0)
                      },
                BusySlots =
                [
                    new Slot
                    {
                        Date = new DateOnly(2024, 08, 26),
                        Start = new TimeOnly(9, 0, 0),
                        End = new TimeOnly(10, 0, 0)
                    }
                ]
            });

        // Act
        var result = _mapper.Map(response);

        // Assert
        Assert.That(result.Facility.Name, Is.EqualTo("Test Facility"));
        Assert.That(result.Facility.Address, Is.EqualTo("123 Test Street"));
        Assert.That(result.SlotDurationMinutes, Is.EqualTo(60));
        Assert.IsNotNull(result.Monday);
        Assert.That(result.Monday.WorkPeriod.StartHour, Is.EqualTo(8));
        Assert.That(result.Monday.WorkPeriod.LunchStartHour, Is.EqualTo(12));
        Assert.That(result.Monday.WorkPeriod.LunchEndHour, Is.EqualTo(13));
        Assert.That(result.Monday.WorkPeriod.EndHour, Is.EqualTo(17));
        Assert.That(result.Monday.BusySlots.Count, Is.EqualTo(1));
        Assert.That(result.Monday.BusySlots.First().Start, Is.EqualTo("2024-08-26T09:00:00"));
        Assert.That(result.Monday.BusySlots.First().End, Is.EqualTo("2024-08-26T10:00:00"));
    }

    [Test]
    public void Map_WithoutDaySchedules_Map()
    {
        // Arrange
        var response = new GetWeekAvailabilityResponse
                       {
                           Facility = new Facility
                                      {
                                          Name = "Test Facility",
                                          Address = "123 Test Street"
                                      },
                           SlotDurationMinutes = 30

                           // No days schedule
                       };

        // Act
        var result = _mapper.Map(response);

        // Assert
        Assert.That(result.Facility.Name, Is.EqualTo("Test Facility"));
        Assert.That(result.Facility.Address, Is.EqualTo("123 Test Street"));
        Assert.That(result.SlotDurationMinutes, Is.EqualTo(30));
        Assert.IsNull(result.Monday);
        Assert.IsNull(result.Tuesday);
        Assert.IsNull(result.Wednesday);
        Assert.IsNull(result.Thursday);
        Assert.IsNull(result.Friday);
    }

    [Test]
    public void Map_WithMultipleDays_Maps()
    {
        // Arrange
        var response = new GetWeekAvailabilityResponse
                       {
                           Facility = new Facility
                                      {
                                          Name = "Test Facility",
                                          Address = "123 Test Street"
                                      },
                           SlotDurationMinutes = 60
                       };
        response.DaysSchedule.Add(
            new DaySchedule
            {
                Name = DayOfWeek.Wednesday,
                WorkPeriod
                    = new WorkPeriod
                      {
                          StartHour = new TimeOnly(9, 0, 0),
                          LunchStartHour = new TimeOnly(13, 0, 0),
                          LunchEndHour = new TimeOnly(14, 0, 0),
                          EndHour = new TimeOnly(18, 0, 0)
                      },
                BusySlots =
                [
                    new Slot
                    {
                        Date = new DateOnly(2024, 08, 28),
                        Start = new TimeOnly(10, 0, 0),
                        End = new TimeOnly(11, 0, 0)
                    },
                    new Slot
                    {
                        Date = new DateOnly(2024, 08, 28),
                        Start = new TimeOnly(12, 0, 0),
                        End = new TimeOnly(13, 0, 0)
                    }
                ]
            }
        );
        response.DaysSchedule.Add(
            new DaySchedule
            {
                Name = DayOfWeek.Monday,
                WorkPeriod
                    = new WorkPeriod
                      {
                          StartHour = new TimeOnly(8, 0, 0),
                          LunchStartHour = new TimeOnly(12, 0, 0),
                          LunchEndHour = new TimeOnly(13, 0, 0),
                          EndHour = new TimeOnly(17, 0, 0)
                      },
                BusySlots =
                [
                    new Slot
                    {
                        Date = new DateOnly(2024, 08, 26),
                        Start = new TimeOnly(9, 0, 0),
                        End = new TimeOnly(10, 0, 0)
                    }
                ]
            });

        // Act
        var result = _mapper.Map(response);

        // Assert
        Assert.That(result.Facility.Name, Is.EqualTo("Test Facility"));
        Assert.That(result.Facility.Address, Is.EqualTo("123 Test Street"));
        Assert.That(result.SlotDurationMinutes, Is.EqualTo(60));
        Assert.IsNotNull(result.Monday);
        Assert.That(result.Monday.WorkPeriod.StartHour, Is.EqualTo(8));
        Assert.That(result.Monday.WorkPeriod.LunchStartHour, Is.EqualTo(12));
        Assert.That(result.Monday.WorkPeriod.LunchEndHour, Is.EqualTo(13));
        Assert.That(result.Monday.WorkPeriod.EndHour, Is.EqualTo(17));
        Assert.That(result.Monday.BusySlots.Count, Is.EqualTo(1));
        Assert.That(result.Monday.BusySlots.First().Start, Is.EqualTo("2024-08-26T09:00:00"));
        Assert.That(result.Monday.BusySlots.First().End, Is.EqualTo("2024-08-26T10:00:00"));
        Assert.IsNotNull(result.Wednesday);
        Assert.That(result.Wednesday.WorkPeriod.StartHour, Is.EqualTo(9));
        Assert.That(result.Wednesday.WorkPeriod.LunchStartHour, Is.EqualTo(13));
        Assert.That(result.Wednesday.WorkPeriod.LunchEndHour, Is.EqualTo(14));
        Assert.That(result.Wednesday.WorkPeriod.EndHour, Is.EqualTo(18));
        Assert.That(result.Wednesday.BusySlots.Count, Is.EqualTo(2));
        Assert.That(result.Wednesday.BusySlots.First().Start, Is.EqualTo("2024-08-28T10:00:00"));
        Assert.That(result.Wednesday.BusySlots.First().End, Is.EqualTo("2024-08-28T11:00:00"));
        Assert.That(result.Wednesday.BusySlots[1].Start, Is.EqualTo("2024-08-28T12:00:00"));
        Assert.That(result.Wednesday.BusySlots[1].End, Is.EqualTo("2024-08-28T13:00:00"));
    }
}
