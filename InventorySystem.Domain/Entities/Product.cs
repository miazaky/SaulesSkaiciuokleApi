namespace InventorySystem.Domain.Entities;

public class Product : BaseEntity
{
    public string? Sku { get; set; }
    public string Name { get; set; } = null!;
    public int? Length { get; set; }
    public string? Description { get; set; }
}