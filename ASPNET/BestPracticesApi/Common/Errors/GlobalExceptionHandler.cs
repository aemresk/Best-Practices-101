using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BestPracticesApi.Common.Errors;

// 01. Global Exception Handling — IExceptionHandler + ProblemDetails RFC 9457
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext       httpContext,
        Exception         exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "İşlenmeyen exception: {Type} — {Message}",
            exception.GetType().Name, exception.Message);

        var (status, title) = exception switch
        {
            ArgumentException           => (StatusCodes.Status400BadRequest,          "Geçersiz istek"),
            KeyNotFoundException        => (StatusCodes.Status404NotFound,            "Kaynak bulunamadı"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized,        "Yetkisiz erişim"),
            _                           => (StatusCodes.Status500InternalServerError, "Sunucu hatası")
        };

        var problem = new ProblemDetails
        {
            Status   = status,
            Title    = title,
            Detail   = status == 500 ? "Beklenmeyen bir hata oluştu." : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode  = status;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
