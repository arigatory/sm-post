using Carter;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Posts.EditMessage;

public class EditMessageEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/posts/{id}/message", async (Guid id, [FromBody] EditMessageCommand command, IMediator mediator, ILogger<EditMessageEndpoint> logger) =>
        {
            try
            {
                command.Id = id;
                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = "Edit message request completed successfully!"
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Client made a bad request!");
                return Results.BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (AggregateNotFoundException ex)
            {
                logger.LogWarning(ex, "Could not retrieve aggregate, client passed an incorrect post id");
                return Results.BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to edit the message!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("EditPostMessage")
        .WithTags("Posts")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
