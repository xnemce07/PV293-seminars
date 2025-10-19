namespace Library.Application.Dtos;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Pages { get; set; }
    public string Genre { get; set; } = string.Empty;

    // Author information
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}