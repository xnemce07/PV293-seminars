using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(Guid authorId)
    {
        return await Entities
            .Where(b => b.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre)
    {
        return await Entities
            .Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase))
            .ToListAsync();
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        return await Entities
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<List<Book>> GetAllWithAuthorsAsync(CancellationToken cancellationToken)
    {
        return await Entities
            .Include(b => b.Author)
            .ToListAsync(cancellationToken);
    }

    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;
}
