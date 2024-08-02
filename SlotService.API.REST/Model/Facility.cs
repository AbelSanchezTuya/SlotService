namespace SlotService.API.REST.Model;

/// <summary>
///     Facility where the appointment will take place.
/// </summary>
public class Facility
{
    /// <summary>
    ///     Facility Name
    /// </summary>
    /// <example> The house of doctors </example>
    public required string Name { get; set; }
    /// <summary>
    ///     Address
    /// </summary>
    /// <example> El xugu la bola 206, Sobrescobiu 339993, Asturias </example>
    public required string Address { get; set; }
}
