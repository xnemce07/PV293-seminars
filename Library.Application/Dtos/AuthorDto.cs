namespace Library.Application.Dtos;

public class AuthorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Country { get; set; } = string.Empty;

    // Statistics
    public int TotalBooksPublished { get; set; }
    public DateTime? LastPublishedDate { get; set; }
    public string MostPopularGenre { get; set; } = string.Empty;
}