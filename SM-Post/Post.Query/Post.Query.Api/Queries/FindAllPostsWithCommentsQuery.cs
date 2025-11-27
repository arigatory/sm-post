using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries;

public class FindAllPostsWithCommentsQuery : BaseQuery<List<PostEntity>>
{
    
}