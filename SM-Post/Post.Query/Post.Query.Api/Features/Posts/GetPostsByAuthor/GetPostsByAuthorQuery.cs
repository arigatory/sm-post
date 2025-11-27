using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Features.Posts.GetPostsByAuthor;

public class GetPostsByAuthorQuery : BaseQuery<List<PostEntity>>
{
    public string Author { get; set; }
}
