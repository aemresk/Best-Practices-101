using FluentValidation;

namespace BestPracticesApi.Common.Filters;

// 02. FluentValidation — endpoint filter olarak otomatik model doğrulama
public sealed class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate          next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();

        if (model is null)
            return Results.BadRequest("İstek gövdesi boş veya geçersiz.");

        var result = await validator.ValidateAsync(model, context.HttpContext.RequestAborted);

        if (!result.IsValid)
            return Results.ValidationProblem(result.ToDictionary());

        return await next(context);
    }
}
