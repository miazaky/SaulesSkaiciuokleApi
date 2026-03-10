using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventorySystem.Domain.Entities;
public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("InventoryTransactions");

        builder.HasKey(it => it.Id);

        builder.Property(it => it.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(it => it.Quantity)
            .HasColumnType("decimal(18,2)");

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(it => it.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(it => it.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}