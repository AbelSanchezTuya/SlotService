namespace SlotService.API.REST.Model;

public class WorkPeriod
{
    /// <summary>
    ///     Morning opening hour (int, from 0 to 23)
    /// </summary>
    /// <example> 9 </example>
    public int StartHour { get; set; }
    /// <summary>
    ///     Morning closing hour (int, from 0 to 23)
    /// </summary>
    /// <example> 13 </example>
    public int LunchStartHour { get; set; }
    /// <summary>
    ///     Afternoon opening hour (int, from 0 to 23)
    /// </summary>
    /// <example> 15 </example>
    public int LunchEndHour { get; set; }
    /// <summary>
    ///     Afternoon closing hour (int, from 0 to 23)
    /// </summary>
    /// <example> 19 </example>
    public int EndHour { get; set; }
}
