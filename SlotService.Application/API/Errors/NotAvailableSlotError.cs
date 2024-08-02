using FluentResults;
using SlotService.Domain;


namespace SlotService.Application.API.Errors;

public class NotAvailableSlotError(Slot slot)
    : Error(
        $"The slot with {slot.Date} and from {slot.Start} to {slot.End} is not available. Check slot duration or date time data.");
