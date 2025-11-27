using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Features.Posts.GetPostsByAuthor;

public class GetPostsByAuthorHandler : IRequestHandler<GetPostsByAuthorQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public GetPostsByAuthorHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(GetPostsByAuthorQuery request, CancellationToken cancellationToken)
    {
        return await _postRepository.ListByAuthorAsync(request.Author);
    }
}
