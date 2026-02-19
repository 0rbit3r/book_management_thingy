using BMT.Contract.Dtos.Results;

namespace BMT.Contract.Dtos.Results;

public class ErrorDto
{
    private const string unspecifiedMsg = "unspecified";
    private ErrorDto(ErrorCode code, string? message = null) { Code = code; Message = message is null ? unspecifiedMsg : message; }
    public ErrorCode Code { get; set; }
    public string Message { get; set; }

    public override string ToString()
    {
        return $"{Code}: {Message}";
    }

    public ErrorDto AddMessage(string additionalMessage)
    {
        Message = $"{Message}\n{additionalMessage}".Trim();
        return this;
    }

    public static ErrorDto General(string? message = null) => new ErrorDto(ErrorCode.General, message);

    public static ErrorDto BadRequest(string? message = null) => new ErrorDto(ErrorCode.BadRequest, message);

    public static ErrorDto NotFound(string? message = null) => new ErrorDto(ErrorCode.NotFound, message);

    public static ErrorDto ExceptionThrown(Exception e) => new ErrorDto(ErrorCode.General, e.ToString());
}

