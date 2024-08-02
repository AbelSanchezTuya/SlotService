namespace SlotService.API.REST.Model;

public class Slot
{
    public const string DateFormat = "yyyy-MM-ddTHH:mm:ss";
    /// <summary>
    ///     Start date-time of the slot with format yyyy-MM-ddTHH:mm:ss
    /// </summary>
    /// <example> yyyy-MM-ddTHH:mm:ss </example>
    public required string Start { get; set; }
    /// <summary>
    ///     Start date-time of the slot with format yyyy-MM-ddTHH:mm:ss
    /// </summary>
    /// <example> yyyy-MM-ddTHH:mm:ss </example>
    public required string End { get; set; }
}
