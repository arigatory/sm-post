using CQRS.Core.Commands;

namespace Post.Cmd.Api.Features.Posts.DeletePost;

public class DeletePostCommand : BaseCommand
{
    public string Username { get; set; }
}
