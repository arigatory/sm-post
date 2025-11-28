using FluentValidation;

namespace Post.Cmd.Api.Features.Posts.CreatePost;

public class CreatePostValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Автор не может быть пустым")
            .MaximumLength(100).WithMessage("Имя автора не может превышать 100 символов");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Сообщение не может быть пустым")
            .MinimumLength(3).WithMessage("Сообщение должно содержать минимум 3 символа")
            .MaximumLength(1000).WithMessage("Сообщение не может превышать 1000 символов");
    }
}
