using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Features.Posts.GetPostById;

public class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public GetPostByIdHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id);
        return post != null ? new List<PostEntity> { post } : new List<PostEntity>();
    }
}
