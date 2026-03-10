namespace InventorySystem.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Location { get; set; }
    public ICollection<WarehouseInventory> Inventories { get; set; } = new List<WarehouseInventory>();
}