using Carter;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Comments.AddComment;

public class AddCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/posts/{id}/comments", async (Guid id, [FromBody] AddCommentCommand command, IMediator mediator, ILogger<AddCommentEndpoint> logger) =>
        {
            try
            {
                command.Id = id;
                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = "Add comment request completed successfully!"
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
                const string SAFE_ERROR_MESSAGE = "Error while processing request to add a comment!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("AddComment")
        .WithTags("Comments")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
