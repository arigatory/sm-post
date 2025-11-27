using Carter;
using CQRS.Core.Exceptions;
using MediatR;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Posts.LikePost;

public class LikePostEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/posts/{id}/like", async (Guid id, IMediator mediator, ILogger<LikePostEndpoint> logger) =>
        {
            try
            {
                var command = new LikePostCommand { Id = id };
                await mediator.Send(command);

                return Results.Ok(new BaseResponse
                {
                    Message = "Like post request completed successfully!"
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
                const string SAFE_ERROR_MESSAGE = "Error while processing request to like a post!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("LikePost")
        .WithTags("Posts")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
