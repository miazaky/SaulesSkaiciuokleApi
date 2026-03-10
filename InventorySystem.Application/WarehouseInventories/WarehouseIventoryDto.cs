using System;

namespace InventorySystem.Application.WarehouseInventories;

public sealed record WarehouseInventoryDto(
    Guid Id,
    Guid WarehouseId,
    Guid ProductId,
    int QuantityCurrent,
    int QuantityMin,
    int QuantityMax
);