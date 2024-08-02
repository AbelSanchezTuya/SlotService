using System.ComponentModel.DataAnnotations;


namespace SlotService.API.REST.Model;

public class WeekSchedule
{
    [Required]
    public required Facility Facility { get; set; }
    /// <summary>
    ///     Set duration for any appointment in minutes.
    /// </summary>
    /// <example> 60 </example>
    [Required]
    public required int SlotDurationMinutes { get; set; }
    public DaySchedule? Monday { get; set; }
    public DaySchedule? Tuesday { get; set; }
    public DaySchedule? Wednesday { get; set; }
    public DaySchedule? Thursday { get; set; }
    public DaySchedule? Friday { get; set; }
}
