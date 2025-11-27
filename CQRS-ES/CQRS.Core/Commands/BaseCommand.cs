using CQRS.Core.Messages;
using MediatR;

namespace CQRS.Core.Commands;

public abstract class BaseCommand : Message, IRequest<Unit>
{
    
}