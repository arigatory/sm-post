using Carter;
using MediatR;
using Post.Query.Api.DTOs;

namespace Post.Query.Api.Features.Posts.GetPostsWithComments;

public class GetPostsWithCommentsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/posts/with-comments", async (IMediator mediator, ILogger<GetPostsWithCommentsEndpoint> logger) =>
        {
            try
            {
                var query = new GetPostsWithCommentsQuery();
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
                const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve posts with comments!";
                logger.LogError(ex, SAFE_ERROR_MESSAGE);
                return Results.Problem(SAFE_ERROR_MESSAGE, statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetPostsWithComments")
        .WithTags("Posts")
        .Produces<PostLookupResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
