using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Features.Posts.GetPostById;

public class GetPostByIdQuery : BaseQuery<List<PostEntity>>
{
    public Guid Id { get; set; }
}
