using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostLookupController : ControllerBase
{
    private readonly ILogger<PostLookupController> _logger;
    private readonly IMediator _mediator;

    public PostLookupController(
        IMediator mediator,
        ILogger<PostLookupController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _mediator.Send(new FindAllPostsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while prcessing request to retrieve all posts!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("byId/{postId}")]
    public async Task<IActionResult> GetByPostIdAsync(Guid postId)
    {
        try
        {
            var posts = await _mediator.Send(new FindPostByIdQuery { Id = postId });

            if (posts == null || !posts.Any()) return NoContent();

            var count = posts.Count();

            return Ok(new PostLookupResponse
            {
                Message = $"Successfully return post!",
                Posts = posts
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while prcessing request to retrieve post by ID!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await _mediator.Send(new FindPostsByAuthorQuery { Author = author });

            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while prcessing request to retrieve posts by author!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withComments")]
    public async Task<ActionResult> GetPostsCommentsAsync()
    {
        try
        {
            var posts = await _mediator.Send(new FindAllPostsWithCommentsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while prcessing request to retrieve posts with comments!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await _mediator.Send(new FindAllPostsWithLikesQuery{ NumberOfLikes = numberOfLikes});
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while prcessing request to retrieve posts with likes!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
    {
        _logger.LogError(ex, safeErrorMessage);

        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = safeErrorMessage
        });
    }

    private ActionResult NormalResponse(List<PostEntity> posts)
    {
        if (posts == null || !posts.Any()) return NoContent();

        var count = posts.Count();

        return Ok(new PostLookupResponse
        {
            Message = $"Successfully return {count} post{(count > 1 ? "s" : string.Empty)}!",
            Posts = posts
        });
    }
}
