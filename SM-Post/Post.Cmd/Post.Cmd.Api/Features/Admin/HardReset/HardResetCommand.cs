using CQRS.Core.Commands;

namespace Post.Cmd.Api.Features.Admin.HardReset;

public class HardResetCommand : BaseCommand
{
    public DateTime ResetToDateTime { get; set; }
}
