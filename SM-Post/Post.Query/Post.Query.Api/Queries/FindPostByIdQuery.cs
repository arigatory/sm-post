using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries;

public class FindPostByIdQuery : BaseQuery<List<PostEntity>>
{
    public Guid Id { get; set; }
}