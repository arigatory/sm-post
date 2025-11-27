using CQRS.Core.Commands;

namespace Post.Cmd.Api.Features.Posts.CreatePost;

public class CreatePostCommand : BaseCommand
{
    public string Author { get; set; }
    public string Message { get; set; }
}
