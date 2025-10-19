using Library.Application.CQRS;
using Library.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Books.Events;

public class BookCreatedEvent : IDomainEvent
{
    public Guid BookId { get; set; }
    public Guid AuthorId { get; set; }
    public string Genre { get; set; } = string.Empty;
}

public class BookCreatedEventHandler(IAuthorRepository authorRepository) : IDomainEventHandler<BookCreatedEvent>
{
    public async Task Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
    {
        await authorRepository.UpdateAuthorStatisticsAsync(notification.AuthorId, cancellationToken);
    }
}
