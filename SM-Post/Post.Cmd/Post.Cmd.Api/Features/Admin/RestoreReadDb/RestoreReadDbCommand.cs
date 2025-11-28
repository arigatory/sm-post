using CQRS.Core.Commands;

namespace Post.Cmd.Api.Features.Admin.RestoreReadDb;

public class RestoreReadDbCommand : BaseCommand
{
    public DateTime? RestoreToDateTime { get; set; }
}
