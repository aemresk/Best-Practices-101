namespace BestPracticesApi.Common.Errors;

public static class ResultExtensions
{
    // Result<T> → uygun HTTP yanıtına çevirir
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        var error = new { result.Error.Code, result.Error.Message };

        if (result.Error.Code.EndsWith(".NotFound"))  return Results.NotFound(error);
        if (result.Error.Code.EndsWith(".Conflict"))  return Results.Conflict(error);

        return Results.BadRequest(error);
    }
}
