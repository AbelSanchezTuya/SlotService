using System.Globalization;
using SlotService.Application.API.Dtos;


namespace SlotService.Application.API;

public class BookSlotCommandBuilder
{
    private string _comments = string.Empty;
    private DateOnly _date;
    private string _email = string.Empty;
    private TimeOnly _end;
    private string _patientName = string.Empty;
    private string _patientSecondName = string.Empty;
    private string _phoneNumber = string.Empty;
    private TimeOnly _start;

    public BookSlotCommandBuilder SetDate(string date)
    {
        _date = ConvertDate(date);

        return this;
    }

    public BookSlotCommandBuilder SetStart(string start)
    {
        _start = ConvertTime(start);

        return this;
    }

    public BookSlotCommandBuilder SetEnd(string end)
    {
        _end = ConvertTime(end);

        return this;
    }

    public BookSlotCommandBuilder SetComments(string comments)
    {
        _comments = comments;

        return this;
    }

    public BookSlotCommandBuilder SetPatientName(string patientName)
    {
        _patientName = patientName;

        return this;
    }

    public BookSlotCommandBuilder SetPatientSecondName(string patientSecondName)
    {
        _patientSecondName = patientSecondName;

        return this;
    }

    public BookSlotCommandBuilder SetEmail(string email)
    {
        _email = email;

        return this;
    }

    public BookSlotCommandBuilder SetPhone(string phone)
    {
        _phoneNumber = phone;

        return this;
    }

    public BookSlotCommand Build()
    {
        return new BookSlotCommand
               {
                   Date = _date,
                   Start = _start,
                   End = _end,
                   Comments = _comments,
                   Patient = new Patient
                             {
                                 Name = _patientName,
                                 SecondName = _patientSecondName,
                                 Email = _email,
                                 Phone = _phoneNumber
                             }
               };
    }

    public DateOnly ConvertDate(string dateTime)
    {
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";

        DateOnly.TryParseExact(
            dateTime,
            dateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var convertDate);

        return convertDate;
    }

    public TimeOnly ConvertTime(string dateTime)
    {
        const string dateFormat = "yyyy-MM-dd HH:mm:ss";

        TimeOnly.TryParseExact(
            dateTime,
            dateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var convertTime);

        return convertTime;
    }
}
