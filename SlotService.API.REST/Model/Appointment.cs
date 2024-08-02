namespace SlotService.API.REST.Model;

/// <summary>
///     Patient appointment.
/// </summary>
public class Appointment
{
    /// <summary>
    ///     Start timestamp (string with format YYYY-MM-dd HH:mm:ss).
    /// </summary>
    /// <example> 2024-06-08 10:00:00 </example>
    public required string Start { get; set; }
    /// <summary>
    ///     End timestamp (string with format YYYY-MM-dd HH:mm:ss).
    /// </summary>
    /// <example> 2024-06-08 11:00:00 </example>
    public required string End { get; set; }
    /// <summary>
    ///     Additional instructions for the doctor.
    /// </summary>
    /// <example> I have headache. </example>
    public string Comments { get; set; } = string.Empty;
    /// <summary>
    ///     Patient information
    /// </summary>
    public required Patient Patient { get; set; }
}
