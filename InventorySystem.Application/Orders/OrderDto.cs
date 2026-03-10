using System;
using System.Collections.Generic;

namespace InventorySystem.Application.Orders;

public sealed record OrderItemDto(
    Guid ProductId,
    int Quantity
);

public sealed record OrderDto(
    Guid Id,
    Guid WarehouseId,
    string Status,
    List<OrderItemDto> Items
);