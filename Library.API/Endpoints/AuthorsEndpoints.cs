using Library.Application.Authors.Queries;
using Library.Application.Dtos;
using MediatR;

namespace Library.API.Endpoints;

public static class AuthorsEndpoints
{
    public static void MapAuthorsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/authors").WithTags("Authors");

        group.MapGet("/", GetAuthors)
            .WithName("GetAuthors")
            .WithSummary("Get all authors with their statistics")
            .Produces<List<AuthorDto>>()
            .WithOpenApi()
            .AllowAnonymous(); // Public access for viewing authors
    }

    private static async Task<IResult> GetAuthors(IMediator mediator)
    {
        var query = new GetAllAuthorsQuery();
        var authors = await mediator.Send(query);
        return Results.Ok(authors);
    }
}