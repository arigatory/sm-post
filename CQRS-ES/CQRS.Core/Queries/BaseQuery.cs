using MediatR;

namespace CQRS.Core.Queries;

public abstract class BaseQuery<TResponse> : IRequest<TResponse>
{
    
}
