using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.OrderingContracts.DomainEvents;
using Yestino.ProductCatalogContracts.DomainEvents;
using Yestino.Warehouse.Domain;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.EventHandlers;

public static class ChangeQuantityOnOrderCancelHandler
{
    public static void handle(CancelOrder domainEvent, WarehouseDbContext dbContext)
    {
        foreach(var orderItem in domainEvent.Items){
            var item = dbContext.Items.First(i => i.CatalogId == orderItem.ProductId);

            item.ChangeReservedStock(-1 * orderItem.Quantity);
        }
    }
}
