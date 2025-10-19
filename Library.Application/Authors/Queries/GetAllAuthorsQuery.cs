using Library.Application.CQRS;
using Library.Application.Dtos;
using Library.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Authors.Queries;

public class GetAllAuthorsQuery : IQuery<List<AuthorDto>>;

public class GetAllAuthorsQueryHandler(IAuthorRepository repository) : IRequestHandler<GetAllAuthorsQuery, List<AuthorDto>>
{
    public async Task<List<AuthorDto>> Handle(GetAllAuthorsQuery query, CancellationToken cancellationToken)
    {
        return await repository.GetAllAuthorDtosAsync(cancellationToken);
    }
}
