using Medo;
namespace InventorySystem.Domain.Entities;
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Uuid7.NewGuid();

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    public bool Deleted { get; set; } = false;
}