using Carter;
using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Comments.RemoveComment;

public class RemoveCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/posts/{id}/comments/{commentId}", async (Guid id, Guid commentId, [FromBody] RemoveCommentCommand command, IMediator mediator, ILogger<RemoveCommentEndpoint> logger) =>
        {
            try
            {
                command.Id = id;
                command.CommentId = commentId;
                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = "Remove comment request completed successfully!"
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Client made a bad request!");
                return Results.BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (AggregateNotFoundException ex)
            {
                logger.LogWarning(ex, "Could not retrieve aggregate");
                return Results.BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to remove a comment!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("RemoveComment")
        .WithTags("Comments")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
