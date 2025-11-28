using CQRS.Core.Queries;
using MediatR;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Features.Posts.GetPostsByAuthor;

public class GetPostsByAuthorQuery : IRequest<List<PostEntity>>
{
    public required string Author { get; set; }
}
