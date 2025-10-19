using Library.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Author entity
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Biography).HasMaxLength(1000);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.MostPopularGenre).HasMaxLength(50);
        });

        // Configure Book entity
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ISBN).HasMaxLength(20);
            entity.Property(e => e.Genre).HasMaxLength(50);

            // Configure relationship
            entity.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Loan entity
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BorrowerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BorrowerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<string>();

            // Configure Fines as owned entities
            entity.OwnsMany(e => e.Fines, finesBuilder =>
            {
                finesBuilder.ToTable("Fines");
                finesBuilder.WithOwner().HasForeignKey("LoanId");
                finesBuilder.HasKey("Id");
                finesBuilder.Property(f => f.Id).ValueGeneratedNever();
                finesBuilder.Property(f => f.Type).HasConversion<string>();
                finesBuilder.Property(f => f.Status).HasConversion<string>();
                finesBuilder.Property(f => f.Reason).IsRequired().HasMaxLength(500);
                finesBuilder.Property(f => f.PaymentReference).HasMaxLength(100);

                // Configure Money value object within Fine
                finesBuilder.OwnsOne(f => f.Amount, amountBuilder =>
                {
                    amountBuilder.Property(m => m.Amount).HasColumnName("Amount").HasPrecision(18, 2);
                    amountBuilder.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
                });
            });

            // Index for querying active loans by book
            entity.HasIndex(e => new { e.BookId, e.Status });
            entity.HasIndex(e => e.BorrowerId);
            entity.HasIndex(e => e.DueDate);
        });

        // Define GUIDs for seed data
        var authorId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var authorId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var authorId3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var bookId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var bookId2 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var bookId3 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        // Seed data for Authors
        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = authorId1,
                Name = "Robert C. Martin",
                Biography = "Software engineer and author, known for Clean Code",
                BirthDate = new DateTime(1952, 12, 5),
                Country = "USA",
                TotalBooksPublished = 1,
                MostPopularGenre = "Programming"
            },
            new Author
            {
                Id = authorId2,
                Name = "Gang of Four",
                Biography = "Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides",
                BirthDate = new DateTime(1960, 1, 1),
                Country = "Various",
                TotalBooksPublished = 1,
                MostPopularGenre = "Programming"
            },
            new Author
            {
                Id = authorId3,
                Name = "David Thomas & Andrew Hunt",
                Biography = "Authors of The Pragmatic Programmer",
                BirthDate = new DateTime(1965, 1, 1),
                Country = "USA",
                TotalBooksPublished = 1,
                MostPopularGenre = "Programming"
            }
        );

        // Seed data for Books - using anonymous types for DDD entities with private setters
        modelBuilder.Entity<Book>().HasData(
            new { Id = bookId1, Title = "Clean Code", AuthorId = authorId1, ISBN = "978-0132350884", Year = 2008, Pages = 464, Genre = "Programming", CreatedAt = DateTime.UtcNow },
            new { Id = bookId2, Title = "The Pragmatic Programmer", AuthorId = authorId3, ISBN = "978-0135957059", Year = 2019, Pages = 352, Genre = "Programming", CreatedAt = DateTime.UtcNow },
            new { Id = bookId3, Title = "Design Patterns", AuthorId = authorId2, ISBN = "978-0201633610", Year = 1994, Pages = 395, Genre = "Programming", CreatedAt = DateTime.UtcNow }
        );
    }
}