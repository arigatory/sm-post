using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries;

public class FindPostsByAuthorQuery : BaseQuery<List<PostEntity>>
{
    public string Author { get; set; }
}
