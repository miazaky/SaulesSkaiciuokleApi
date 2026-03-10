using InventorySystem.Domain.Enums;
namespace InventorySystem.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? OrderId { get; set; }

    public TransactionType Type { get; set; }
    public decimal Quantity { get; set; }

    public Guid? SourceWarehouseId { get; set; }
    public Guid? DestinationWarehouseId { get; set; }

    public string? Notes { get; set; }
}