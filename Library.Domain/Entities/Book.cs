namespace Library.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string ISBN { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public int Pages { get; private set; }
    public string Genre { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    // Foreign key
    public Guid AuthorId { get; private set; }
    // Navigation property
    public Author Author { get; set; } = null!;

    // Private constructor for EF Core
    private Book() { }

    // Factory method for creating a new book
    public static Book Create(string title, string isbn, int year, int pages, string genre, Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (authorId == Guid.Empty)
            throw new ArgumentException("Author ID cannot be empty", nameof(authorId));

        return new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            ISBN = isbn,
            Year = year,
            Pages = pages,
            Genre = genre,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string title, int year, int pages, string genre)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title;
        Year = year;
        Pages = pages;
        Genre = genre;
    }

    public bool IsClassic() => Year < 1950;
    public bool IsNewRelease() => Year >= DateTime.Now.Year - 2;

    public TimeSpan GetEstimatedReadingTime()
    {
        const int wordsPerPage = 250;
        const int wordsPerMinute = 200;
        var totalWords = Pages * wordsPerPage;
        var minutes = totalWords / wordsPerMinute;
        return TimeSpan.FromMinutes(minutes);
    }
}