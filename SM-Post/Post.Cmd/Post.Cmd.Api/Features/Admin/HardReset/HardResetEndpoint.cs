using Carter;
using MediatR;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Admin.HardReset;

public class HardResetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/admin/hard-reset", async (HardResetCommand command, IMediator mediator, ILogger<HardResetEndpoint> logger) =>
        {
            try
            {
                if (command.ResetToDateTime == default)
                {
                    return Results.BadRequest(new BaseResponse
                    {
                        Message = "ResetToDateTime is required for hard reset!"
                    });
                }

                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = $"Hard reset completed successfully! All events after {command.ResetToDateTime:yyyy-MM-dd HH:mm:ss} have been deleted."
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Client made a bad request!");
                return Results.BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing hard reset request!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("HardReset")
        .WithTags("Admin")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
