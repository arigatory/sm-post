using Post.Common.DTOs;

namespace Post.Cmd.Api.Features.Posts.CreatePost;

public class CreatePostResponse : BaseResponse
{
    public Guid Id { get; set; }
}
