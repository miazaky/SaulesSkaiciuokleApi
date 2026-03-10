namespace InventorySystem.Application.Warehouses;

public sealed record WarehouseDto(
    Guid Id,
    string Name,
    string? Location
);