using FluentValidation;

namespace Post.Cmd.Api.Features.Posts.LikePost;

public class LikePostValidator : AbstractValidator<LikePostCommand>
{
    public LikePostValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID поста не может быть пустым");
    }
}
