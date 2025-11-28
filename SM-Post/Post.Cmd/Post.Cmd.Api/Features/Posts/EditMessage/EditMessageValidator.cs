using FluentValidation;

namespace Post.Cmd.Api.Features.Posts.EditMessage;

public class EditMessageValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID поста не может быть пустым");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Сообщение не может быть пустым")
            .MinimumLength(3).WithMessage("Сообщение должно содержать минимум 3 символа")
            .MaximumLength(1000).WithMessage("Сообщение не может превышать 1000 символов");
    }
}
