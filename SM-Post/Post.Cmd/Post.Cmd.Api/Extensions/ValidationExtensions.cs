using FluentValidation;

namespace Post.Cmd.Api.Extensions;

public static class ValidationExtensions
{
    public static IResult HandleValidationException(ValidationException ex, ILogger logger)
    {
        logger.LogWarning(ex, "Validation failed!");
        var errors = ex.Errors.Select(e => new
        {
            Field = e.PropertyName,
            Message = e.ErrorMessage
        });
        return Results.BadRequest(new
        {
            Message = "Ошибка валидации",
            Errors = errors
        });
    }
}
