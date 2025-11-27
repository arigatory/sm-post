using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Features.Posts.GetPostsWithComments;

public class GetPostsWithCommentsHandler : IRequestHandler<GetPostsWithCommentsQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public GetPostsWithCommentsHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(GetPostsWithCommentsQuery request, CancellationToken cancellationToken)
    {
        return await _postRepository.ListWithCommentsAsync();
    }
}
