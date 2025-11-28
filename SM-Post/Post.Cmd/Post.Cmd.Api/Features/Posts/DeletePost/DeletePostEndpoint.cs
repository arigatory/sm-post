using Carter;
using CQRS.Core.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Extensions;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Posts.DeletePost;

public class DeletePostEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/posts/{id}", async (Guid id, [FromBody] DeletePostCommand command, IMediator mediator, ILogger<DeletePostEndpoint> logger) =>
        {
            try
            {
                command.Id = id;
                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = "Delete post request completed successfully!"
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
                logger.LogWarning(ex, "Could not retrieve aggregate, client passed an incorrect post id");
                return Results.BadRequest(new BaseResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to delete a post!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("DeletePost")
        .WithTags("Posts")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
