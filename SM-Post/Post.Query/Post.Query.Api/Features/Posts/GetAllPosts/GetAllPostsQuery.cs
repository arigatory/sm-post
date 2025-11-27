using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Features.Posts.GetAllPosts;

public class GetAllPostsQuery : BaseQuery<List<PostEntity>>
{
}
