using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.OrderingContracts.DomainEvents;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.EventHandlers;

public static class ReserveStockOnOrderCreatedHandler
{
    public static void Handle(OrderCreated domainEvent, WarehouseDbContext context)
    {
        foreach (var orderItem in domainEvent.Items)
        {
            var warehouseItem = context.Items.First(i => i.CatalogId == orderItem.ProductId);
            warehouseItem.ChangeReservedStock(orderItem.Quantity);
        }
    }
}
