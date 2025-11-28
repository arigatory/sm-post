using Carter;
using FluentValidation;
using MediatR;
using Post.Cmd.Api.Extensions;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Posts.CreatePost;

public class CreatePostEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/posts", async (CreatePostCommand command, IMediator mediator, ILogger<CreatePostEndpoint> logger) =>
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await mediator.Send(command);

                return Results.Created($"/api/v1/posts/{id}", new CreatePostResponse
                {
                    Message = "New post creation request completed successfully!",
                    Id = id
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
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("CreatePost")
        .WithTags("Posts")
        .Produces<CreatePostResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
