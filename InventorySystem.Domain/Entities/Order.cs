namespace InventorySystem.Domain.Entities;

public class Order : BaseEntity
{
    public Guid WarehouseId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? InventoryWrittenOffAt { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}