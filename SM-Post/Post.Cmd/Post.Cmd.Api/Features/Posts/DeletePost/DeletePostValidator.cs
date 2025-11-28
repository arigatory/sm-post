using FluentValidation;

namespace Post.Cmd.Api.Features.Posts.DeletePost;

public class DeletePostValidator : AbstractValidator<DeletePostCommand>
{
    public DeletePostValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID поста не может быть пустым");
    }
}
