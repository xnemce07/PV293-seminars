using Library.Application.CQRS;
using Library.Application.Dtos;
using Library.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Books.Queries;

public class GetAllBooksQuery : IQuery<List<BookDto>>;

public class GetAllBooksQueryHandler(IBookRepository bookRepository) : IQueryHandler<GetAllBooksQuery, List<BookDto>>
{
    public async Task<List<BookDto>> Handle(GetAllBooksQuery query, CancellationToken cancellationToken)
    {
        var books = await bookRepository.GetAllWithAuthorsAsync(cancellationToken);

        return books.Select(book => new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            AuthorId = book.AuthorId,
            AuthorName = book.Author?.Name ?? string.Empty,
            ISBN = book.ISBN,
            Year = book.Year,
            Pages = book.Pages,
            Genre = book.Genre,
        }).ToList();
    }
}
