using CQRS.Core.Commands;

namespace Post.Cmd.Api.Features.Comments.AddComment;

public class AddCommentCommand : BaseCommand
{
    public string Comment { get; set; }
    public string Username { get; set; }
}
