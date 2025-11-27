using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries.Handlers;

public class FindPostByIdQueryHandler : IRequestHandler<FindPostByIdQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindPostByIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id);
        return post is null ? new() : new List<PostEntity> { post };
    }
}
