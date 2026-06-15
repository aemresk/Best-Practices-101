namespace BestPracticesApi.Common.Errors;

public sealed record AppError(string Code, string Message)
{
    public static readonly AppError None = new(string.Empty, string.Empty);

    public static AppError NotFound(string resource) =>
        new($"{resource}.NotFound", $"{resource} bulunamadı.");

    public static AppError Conflict(string resource, string detail) =>
        new($"{resource}.Conflict", detail);

    public static AppError Invalid(string detail) =>
        new("Request.Invalid", detail);
}
