using InventorySystem.Domain.Enums;
using System;

namespace InventorySystem.Application.InventoryTransactions;

public sealed record InventoryTransactionDto(
	Guid Id,
	Guid WarehouseId,
	Guid ProductId,
	decimal Quantity,
	TransactionType Type,
	DateTime CreatedDate
);