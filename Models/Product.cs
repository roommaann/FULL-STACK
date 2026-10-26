using System.ComponentModel.DataAnnotations;

namespace Models;

public class Product
{
    public Guid ProductId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 99999999)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
