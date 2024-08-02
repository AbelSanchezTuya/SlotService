using System.Text.Json;
using SlotService.Domain;


namespace SlotService.Storage.Helper;

public class DevDataLoader
{
    private const string JsonPath = "Helper";
    private const string JsonFile = "DevData.json";

    public void Load()
    {
        var jsonFile = ReadJsonFileAsString();
        var weekSchedules = ConvertJson(jsonFile);
        foreach (var weekSchedule in weekSchedules)
        {
            InMemoryAgendaRepository.Instance.AddWeekSchedule(weekSchedule);
        }
    }

    public static string ReadJsonFileAsString()
    {
        var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(directoryPath, JsonPath, JsonFile);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "File path cannot be null or whitespace.",
                nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified file was not found.", filePath);
        }

        return File.ReadAllText(filePath);
    }

    private static List<WeekSchedule> ConvertJson(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var weekSchedules = new List<WeekSchedule>();

        var weekDay = DateOnly.FromDateTime(DateTime.Now);

        foreach (var weekElement in root.EnumerateArray())
        {
            var facilityElement = weekElement.GetProperty("Facility");
            var facility = new Facility
                           {
                               Name = facilityElement
                                                 .GetProperty("Name")
                                                 .GetString() ?? string.Empty,
                               Address = facilityElement
                                                    .GetProperty("Address")
                                                    .GetString() ?? string.Empty
            };

            var slotDurationMinutes = weekElement.GetProperty("SlotDurationMinutes").GetInt32();

            var weekSchedule = new WeekSchedule(
                weekDay,
                slotDurationMinutes,
                facility
            );

            foreach (var day in weekElement.EnumerateObject())
            {
                if (day.Name == "Facility" ||
                    day.Name == "SlotDurationMinutes")
                {
                    continue;
                }

                if (Enum.TryParse(day.Name, out DayOfWeek dayOfWeek))
                {
                    var workPeriodElement = day.Value.GetProperty("WorkPeriod");
                    var morningShift = new Shift(
                        GetTimeOnly("StartHour", workPeriodElement),
                        GetTimeOnly("LunchStartHour", workPeriodElement));
                    var afternoonShift = new Shift(
                        GetTimeOnly("LunchEndHour", workPeriodElement),
                        GetTimeOnly("EndHour", workPeriodElement));
                    var workPeriod = new WorkPeriod
                                     {
                                         MorningShift = morningShift,
                                         AfternoonShift = afternoonShift
                                     };

                    var daySchedule = weekSchedule.OpenDaySchedule(dayOfWeek, workPeriod);

                    foreach (var busySlot in day.Value.GetProperty("BusySlots").EnumerateArray())
                    {
                        var start = GetTimeOnly("Start", busySlot);
                        var end = GetTimeOnly("End", busySlot);
                        var date = weekDay.AddDays(dayOfWeek - weekDay.DayOfWeek);
                        var slot = new Slot(date, start, end);
                        daySchedule.BusySlots.Add(slot);
                    }
                }
            }

            weekSchedules.Add(weekSchedule);

            weekDay = weekDay.AddDays(7);
        }

        return weekSchedules;
    }

    private static TimeOnly GetTimeOnly(string elementName, JsonElement workPeriodElement)
    {
        return TimeOnly.Parse(workPeriodElement.GetProperty(elementName).GetInt16() + ":00");
    }
}
