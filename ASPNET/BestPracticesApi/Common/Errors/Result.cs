namespace BestPracticesApi.Common.Errors;

// 03. Result Pattern — exception fırlatmak yerine başarı/hata durumu döndür
public sealed class Result<T>
{
    private Result(T? value, AppError error, bool isSuccess)
    {
        Value     = value;
        Error     = error;
        IsSuccess = isSuccess;
    }

    public bool     IsSuccess { get; }
    public bool     IsFailure => !IsSuccess;
    public T?       Value     { get; }
    public AppError Error     { get; }

    public static Result<T> Success(T value)     => new(value,   AppError.None, true);
    public static Result<T> Failure(AppError error) => new(default, error,        false);
}

public static class Result
{
    public static Result<T> Success<T>(T value)       => Result<T>.Success(value);
    public static Result<T> Failure<T>(AppError error) => Result<T>.Failure(error);
}
