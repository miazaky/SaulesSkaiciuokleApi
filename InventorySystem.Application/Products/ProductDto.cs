using System;

namespace InventorySystem.Application.Products;

public sealed record ProductDto(
    Guid Id,
    string? Sku,
    string Name,
    int? Length,
	string? Description
);