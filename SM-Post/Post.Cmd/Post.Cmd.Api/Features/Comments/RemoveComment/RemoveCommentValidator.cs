using FluentValidation;

namespace Post.Cmd.Api.Features.Comments.RemoveComment;

public class RemoveCommentValidator : AbstractValidator<RemoveCommentCommand>
{
    public RemoveCommentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID поста не может быть пустым");

        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("ID комментария не может быть пустым");
    }
}
