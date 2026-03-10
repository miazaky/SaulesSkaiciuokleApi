namespace InventorySystem.Application.Common.DTOs;

public record InventorySummaryDto(
    int TotalProducts,
    int TotalWarehouses,
    int TotalInventoryRecords,
    decimal TotalStockUnits,
    int LowStockItems
);