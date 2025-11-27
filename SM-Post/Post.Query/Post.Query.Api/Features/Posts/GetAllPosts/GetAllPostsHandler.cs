using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Features.Posts.GetAllPosts;

public class GetAllPostsHandler : IRequestHandler<GetAllPostsQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public GetAllPostsHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        return await _postRepository.ListAllAsync();
    }
}
