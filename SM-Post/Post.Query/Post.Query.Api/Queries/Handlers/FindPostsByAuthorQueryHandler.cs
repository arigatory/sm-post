using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries.Handlers;

public class FindPostsByAuthorQueryHandler : IRequestHandler<FindPostsByAuthorQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository;

    public FindPostsByAuthorQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> Handle(FindPostsByAuthorQuery request, CancellationToken cancellationToken)
    {
        return await _postRepository.ListByAuthorAsync(request.Author);
    }
}
