using System.Globalization;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlotService.API.REST.Mappers;
using SlotService.API.REST.Model;
using SlotService.Application.API;
using SlotService.Application.API.Common;


namespace SlotService.API.REST.Controllers;

[Route("api/availability/[action]")]
[Authorize(AuthenticationSchemes = "BasicAuthentication")]
[ApiController]
public class AvailabilityController(
    IValidator<Appointment> appointmentValidator,
    IMapper<WeekSchedule, GetWeekAvailabilityResponse> getWeekAvailabilityMapper,
    IMessageDispatcher messageDispatcher)
    : ControllerBase
{
    /// <summary>
    ///     Gets week availability.
    /// </summary>
    /// <remarks>
    ///     Given a date (format: yyyyMMdd), returns the schedule of the week containing the given date.
    /// </remarks>
    /// <param name="yyyyMMdd">
    ///     Day contained by the week were to check availability. The only format
    ///     accepted is <b> yyyyMMdd </b>.
    /// </param>
    /// <returns> Week schedule for the week containing the yyyyMMdd/>. </returns>
    [HttpGet("{yyyyMMdd}")]
    [ProducesResponseType<WeekSchedule>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeekSchedule>> GetWeeklyAvailability(string yyyyMMdd)
    {
        var isValid = DateOnly.TryParseExact(
            yyyyMMdd,
            "yyyyMMdd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date);

        if (!isValid)
        {
            return BadRequest("Invalid date format. Use yyyyMMdd format.");
        }

        var result
            = await messageDispatcher.Dispatch(new GetWeeklyAvailabilityQuery { Date = date });

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        if (result is not Result<GetWeekAvailabilityResponse> queryResult)
        {
            return BadRequest("No response");
        }

        var weekSchedule = getWeekAvailabilityMapper.Map(queryResult.Value);

        return Ok(weekSchedule);
    }

    /// <summary>
    ///     Allows to book an appointment.
    /// </summary>
    /// <remarks>
    ///     Allows a patient to book a non-busy slot.
    /// </remarks>
    /// <param name="appointment">
    ///     Required information about the slot and the patient.
    /// </param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TakeSlot([FromBody] Appointment appointment)
    {
        var validation = appointmentValidator.Validate(appointment);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors);
        }

        var command = new BookSlotCommandBuilder()
                     .SetDate(appointment.Start)
                     .SetStart(appointment.Start)
                     .SetEnd(appointment.End)
                     .SetComments(appointment.Comments)
                     .SetPatientName(appointment.Patient.Name)
                     .SetPatientSecondName(appointment.Patient.SecondName)
                     .SetEmail(appointment.Patient.Email)
                     .SetPhone(appointment.Patient.Phone)
                     .Build();

        var postResult = await messageDispatcher.Dispatch(command);

        if (postResult.IsSuccess)
        {
            return Ok("Success");
        }

        return BadRequest(postResult.Errors);
    }
}
