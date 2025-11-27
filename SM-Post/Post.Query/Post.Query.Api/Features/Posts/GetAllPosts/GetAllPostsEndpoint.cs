using Carter;
using MediatR;
using Post.Query.Api.DTOs;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Features.Posts.GetAllPosts;

public class GetAllPostsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/posts", async (IMediator mediator, ILogger<GetAllPostsEndpoint> logger) =>
        {
            try
            {
                var query = new GetAllPostsQuery();
                var posts = await mediator.Send(query);

                if (posts == null || !posts.Any())
                    return Results.NoContent();

                return Results.Ok(new PostLookupResponse
                {
                    Message = $"Successfully return {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}!",
                    Posts = posts
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all posts!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetAllPosts")
        .WithTags("Posts")
        .Produces<PostLookupResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
