using Library.Domain.Entities;

namespace Library.Application.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(Guid authorId);
        Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre);
        Task<Book?> GetBookByIsbnAsync(string isbn);
        Task<List<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken);
    }
}