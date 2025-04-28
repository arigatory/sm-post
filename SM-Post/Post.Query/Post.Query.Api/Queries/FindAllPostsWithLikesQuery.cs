using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;

public class FindAllPostsWithLikesQuery : BaseQuery
{
    public int NumberOfLikes { get; set; }
}