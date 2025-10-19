using Library.Application.Books.Events;
using Library.Application.CQRS;
using Library.Application.Repositories;
using Library.Domain.Entities;
using MediatR;

namespace Library.Application.Books.Commands;

public class CreateBookCommand : ICommand<Guid>
{
    public string Title { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Pages { get; set; }
    public string Genre { get; set; } = string.Empty;
}

public class CreateBookCommandHandler(
    IBookRepository bookRepository,
    IMediator mediator)
    : IRequestHandler<CreateBookCommand, Guid>
{
    public async Task<Guid> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var book = Book.Create(
            command.Title,
            command.ISBN,
            command.Year,
            command.Pages,
            command.Genre,
            command.AuthorId
        );

        bookRepository.Add(book);

        await mediator.Publish(new BookCreatedEvent
        {
            BookId = book.Id,
            AuthorId = book.AuthorId,
            Genre = book.Genre
        }, cancellationToken);

        return book.Id;
    }
}