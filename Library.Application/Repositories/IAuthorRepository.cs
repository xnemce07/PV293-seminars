using Library.Application.Dtos;
using Library.Domain.Entities;

namespace Library.Application.Repositories;

public interface IAuthorRepository: IRepository<Author>
{
    Task<List<AuthorDto>> GetAllAuthorDtosAsync(CancellationToken cancellationToken);
    Task UpdateAuthorStatisticsAsync(Guid authorId, CancellationToken cancellationToken);
}
