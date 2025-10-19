using Library.Application.Dtos;
using Library.Application.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    private ApplicationDbContext dbContext;
    public AuthorRepository(ApplicationDbContext context) : base(context)
    {
        dbContext = context;
    }

    public async Task<List<AuthorDto>> GetAllAuthorDtosAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Authors
            .Select(author => new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Biography = author.Biography,
                BirthDate = author.BirthDate,
                Country = author.Country,
                TotalBooksPublished = author.TotalBooksPublished,
                LastPublishedDate = author.LastPublishedDate,
                MostPopularGenre = author.MostPopularGenre,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAuthorStatisticsAsync(Guid authorId, CancellationToken cancellationToken)
    {
        var author = await dbContext.Authors
            .FirstOrDefaultAsync(a => a.Id == authorId, cancellationToken);

        if (author == null)
            return;

        author.TotalBooksPublished++;
        author.LastPublishedDate = DateTime.UtcNow;

        var mostPopularGenre = await dbContext.Books
            .Where(b => b.AuthorId == author.Id)
            .GroupBy(b => b.Genre)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefaultAsync(cancellationToken);

        if (!string.IsNullOrEmpty(mostPopularGenre))
        {
            author.MostPopularGenre = mostPopularGenre;
        }

        dbContext.Authors.Update(author);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
