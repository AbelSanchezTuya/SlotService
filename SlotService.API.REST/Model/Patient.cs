namespace SlotService.API.REST.Model;

public class Patient
{
    /// <summary>
    ///     Patient Name
    /// </summary>
    /// <example> Bilbo </example>
    public required string Name { get; set; }
    /// <summary>
    ///     Patient second name
    /// </summary>
    /// <example> Baggings </example>
    public required string SecondName { get; set; }
    /// <summary>
    ///     Patient e-mail
    /// </summary>
    /// <example> bb@hobbiton.com </example>
    public required string Email { get; set; }
    /// <summary>
    ///     Patient phone
    /// </summary>
    /// <example> 645798132 </example>
    public required string Phone { get; set; }
}
