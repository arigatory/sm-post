using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries.Handlers;

public class FindAllPostsWithLikesQueryHandler : IRequestHandler<FindAllPostsWithLikesQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindAllPostsWithLikesQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindAllPostsWithLikesQuery request, CancellationToken cancellationToken)
    {
        return await _postRepository.ListWithLikesAsync(request.NumberOfLikes);
    }
}
