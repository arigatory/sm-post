using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Features.Posts.GetPostsWithLikes;

public class GetPostsWithLikesQuery : BaseQuery<List<PostEntity>>
{
    public int NumberOfLikes { get; set; }
}
