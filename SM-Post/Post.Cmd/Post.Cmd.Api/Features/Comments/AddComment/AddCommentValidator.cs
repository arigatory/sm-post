using FluentValidation;

namespace Post.Cmd.Api.Features.Comments.AddComment;

public class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID поста не может быть пустым");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Комментарий не может быть пустым")
            .MinimumLength(2).WithMessage("Комментарий должен содержать минимум 2 символа")
            .MaximumLength(500).WithMessage("Комментарий не может превышать 500 символов");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя не может быть пустым")
            .MaximumLength(50).WithMessage("Имя пользователя не может превышать 50 символов");
    }
}
