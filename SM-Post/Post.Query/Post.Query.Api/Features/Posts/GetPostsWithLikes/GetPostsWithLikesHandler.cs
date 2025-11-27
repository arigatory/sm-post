using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Features.Posts.GetPostsWithLikes;

public class GetPostsWithLikesHandler : IRequestHandler<GetPostsWithLikesQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public GetPostsWithLikesHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(GetPostsWithLikesQuery request, CancellationToken cancellationToken)
    {
        return await _postRepository.ListWithLikesAsync(request.NumberOfLikes);
    }
}
