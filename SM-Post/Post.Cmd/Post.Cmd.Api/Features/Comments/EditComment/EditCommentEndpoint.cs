using Carter;
using CQRS.Core.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Extensions;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Comments.EditComment;

public class EditCommentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/posts/{id}/comments/{commentId}", async (Guid id, Guid commentId, [FromBody] EditCommentCommand command, IMediator mediator, ILogger<EditCommentEndpoint> logger) =>
        {
            try
            {
                command.Id = id;
                command.CommentId = commentId;
                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = "Edit comment request completed successfully!"
                });
            }
            catch (ValidationException ex)
            {
                return ValidationExtensions.HandleValidationException(ex, logger);
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
                const string SAFE_ERROR_MESSAGE = "Error while processing request to edit a comment!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("EditComment")
        .WithTags("Comments")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
