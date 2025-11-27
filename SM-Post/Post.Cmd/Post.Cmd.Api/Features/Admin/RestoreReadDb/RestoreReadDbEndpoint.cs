using Carter;
using MediatR;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Admin.RestoreReadDb;

public class RestoreReadDbEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/admin/restore-read-db", async (IMediator mediator, ILogger<RestoreReadDbEndpoint> logger) =>
        {
            try
            {
                var command = new RestoreReadDbCommand();
                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = "Restore read database request completed successfully!"
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Client made a bad request!");
                return Results.BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to restore read database!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("RestoreReadDatabase")
        .WithTags("Admin")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
