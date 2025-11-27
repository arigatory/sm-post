using Carter;
using MediatR;
using Post.Query.Api.DTOs;

namespace Post.Query.Api.Features.Posts.GetPostById;

public class GetPostByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/posts/{postId:guid}", async (Guid postId, IMediator mediator, ILogger<GetPostByIdEndpoint> logger) =>
        {
            try
            {
                var query = new GetPostByIdQuery { Id = postId };
                var posts = await mediator.Send(query);

                if (posts == null || !posts.Any())
                    return Results.NoContent();

                return Results.Ok(new PostLookupResponse
                {
                    Message = "Successfully return post!",
                    Posts = posts
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve post by ID!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetPostById")
        .WithTags("Posts")
        .Produces<PostLookupResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
