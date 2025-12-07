namespace Yestino.Ordering.Application;

public record ProductReadModel
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public string? ImageUrl { get; init; }
    public int StockQuantity { get; set; }
}