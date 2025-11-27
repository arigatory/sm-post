using CQRS.Core.Commands;

namespace Post.Cmd.Api.Features.Posts.EditMessage;

public class EditMessageCommand : BaseCommand
{
    public string Message { get; set; }
}
