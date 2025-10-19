using Library.Application.Books.Commands;
using Library.Application.Books.Queries;
using Library.Application.Dtos;
using Library.Domain.Constants;
using MediatR;

namespace Library.API.Endpoints;

public static class BooksEndpoints
{
    public static void MapBooksEndpoints(this WebApplication app)
    {
        var books = app.MapGroup("/api/books")
            .WithTags("Books")
            .WithOpenApi();

        books.MapGet("", GetBooks)
            .WithName("GetBooks")
            .Produces<List<BookDto>>()
            .AllowAnonymous(); // Public access for viewing books

        books.MapPost("", CreateBook)
            .WithName("CreateBook")
            .Accepts<CreateBookCommand>("application/json")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy
                .RequireRole(UserRoles.Librarian, UserRoles.Admin)); // Only Librarian or Admin can create books
    }

    private static async Task<IResult> GetBooks(IMediator mediator)
    {
        var query = new GetAllBooksQuery();
        var books = await mediator.Send(query);
        return Results.Ok(books);
    }

    private static async Task<IResult> CreateBook(CreateBookCommand command, IMediator mediator)
    {
        var id = await mediator.Send(command);
        return Results.Created($"/api/books/{id}", new { id });
    }
}