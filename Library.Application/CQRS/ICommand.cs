using MediatR;

namespace Library.Application.CQRS;

public interface ICommand : IRequest;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, ICommand>
    where TCommand : ICommand<ICommand>;

public interface ICommand<out TResponse> : IRequest<TResponse>;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;