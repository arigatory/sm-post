using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Features.Posts.GetPostsWithComments;

public class GetPostsWithCommentsQuery : BaseQuery<List<PostEntity>>
{
}
