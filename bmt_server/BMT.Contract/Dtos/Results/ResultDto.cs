namespace BMT.Contract.Dtos.Results;


public class ResultDto
{
    public ErrorDto? Error { get; init; } = null;

    public bool IsSuccess => Error is null;

    protected ResultDto(ErrorDto error)
    {
        Error = error;
    }

    protected ResultDto() { }

    public static ResultDto Success() => new ResultDto();

    public static ResultDto<T_Value> Success<T_Value>(T_Value payload) => new ResultDto<T_Value>(payload);

    public static ResultDto Failure(ErrorDto error) => new ResultDto(error);

    public static ResultDto<T_Value> Failure<T_Value>(ErrorDto error) => new ResultDto<T_Value>(error);


    // Conversion operators
    public static implicit operator ResultDto(ErrorDto error) => new ResultDto(error);
}

public class ResultDto<T_Value> : ResultDto
{
    public T_Value? Payload { get; init; } = default;

    internal ResultDto(T_Value payload)
    {
        Payload = payload;
    }

    internal ResultDto(ErrorDto error)
    {
        Error = error;
    }

    // Conversion operators
    public static implicit operator ResultDto<T_Value>(T_Value value) => new ResultDto<T_Value>(value);

    public static implicit operator ResultDto<T_Value>(ErrorDto error) => new ResultDto<T_Value>(error);
}

