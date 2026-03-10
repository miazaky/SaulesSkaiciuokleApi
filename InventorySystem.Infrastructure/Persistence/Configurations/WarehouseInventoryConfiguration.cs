using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventorySystem.Domain.Entities;
public class WarehouseInventoryConfiguration : IEntityTypeConfiguration<WarehouseInventory>
{
    public void Configure(EntityTypeBuilder<WarehouseInventory> builder)
    {
        builder.ToTable("WarehouseInventory");

        builder.HasKey(wi => wi.Id);

        builder.Property(wi => wi.QuantityCurrent)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(wi => wi.QuantityMin)
            .HasColumnType("decimal(18,2)");

        builder.Property(wi => wi.QuantityMax)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(wi => new { wi.WarehouseId, wi.ProductId })
            .IsUnique();

        builder.HasOne(wi => wi.Warehouse)
            .WithMany(w => w.Inventories)
            .HasForeignKey(wi => wi.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wi => wi.Product)
            .WithMany()
            .HasForeignKey(wi => wi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}