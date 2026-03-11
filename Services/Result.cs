using MyFirstProject.Services;

public class Result<TValue>
{
    public bool IsSuccess { get; }
    private readonly TValue? _value;

    public TValue? Value {
        get
        {
            if (!IsSuccess)
            {
                throw new InvalidOperationException("Cannot access Value when the result is a failure.");
            }
            return _value;
        } }
    
    public Error? Error { get; }

    private Result(bool isSuccess, Error error, TValue value)
    {
        IsSuccess = isSuccess;
        Error = error;
        _value = value;
    }
    public static Result<TValue> Success(TValue value) => new(true, null, value);
    public static Result<TValue> Success() => new(true, null, default);
    public static Result<TValue> Failure(Error error) => new(false, error, default);
}