using Library.Application.CQRS;
using Library.Infrastructure.Data;
using MediatR;

namespace Library.Infrastructure.Middleware;

public sealed class TransactionalBehavior<TRequest, TResponse>(ApplicationDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var isCmdOrEvent =
            request is ICommand<TResponse> ||
            request is ICommand ||
            request is IDomainEvent;

        if (!isCmdOrEvent) return await next(ct);

        if (dbContext.Database.CurrentTransaction is null)
        {
            await using var tx = await dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var response = await next(ct);
                await dbContext.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return response;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
        
        return await next(ct);
    }
}