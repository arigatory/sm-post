using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries;

public class FindAllPostsWithLikesQuery : BaseQuery<List<PostEntity>>
{
    public int NumberOfLikes { get; set; }
}