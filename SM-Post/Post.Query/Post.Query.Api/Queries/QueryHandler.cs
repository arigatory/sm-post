using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries;

public class QueryHandler : IQueryHandler
{
    private readonly IPostRepository _postRepository;

    public QueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
    {
        return await _postRepository.ListAllAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
    {
        var post = await _postRepository.GetByIdAsync(query.Id);
        return post is null ? new() : new List<PostEntity> { post };
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
    {
        return await _postRepository.ListByAuthorAsync(query.Author);
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsWithCommentsQuery query)
    {
        return await _postRepository.ListWithCommentsAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsWithLikesQuery query)
    {
        return await _postRepository.ListWithLikesAsync(query.NumberOfLikes);
    }
}
