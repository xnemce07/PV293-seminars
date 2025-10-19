namespace Library.Domain.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Country { get; set; } = string.Empty;

    // Navigation property
    public List<Book> Books { get; set; } = new();

    // Statistics that need updating when books are added/removed
    public int TotalBooksPublished { get; set; }
    public DateTime? LastPublishedDate { get; set; }
    public string MostPopularGenre { get; set; } = string.Empty;
}