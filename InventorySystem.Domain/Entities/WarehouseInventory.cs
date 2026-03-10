namespace InventorySystem.Domain.Entities;

public class WarehouseInventory : BaseEntity
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }

    public int QuantityCurrent { get; set; }
    public int QuantityMin { get; set; }
    public int QuantityMax { get; set; }
    
    public Warehouse Warehouse { get; set; } = null!;
    public Product Product { get; set; } = null!;
}