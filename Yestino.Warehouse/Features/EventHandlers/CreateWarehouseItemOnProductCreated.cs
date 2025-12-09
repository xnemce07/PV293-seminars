using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.ProductCatalogContracts.DomainEvents;
using Yestino.Warehouse.Domain;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.EventHandlers;

public static class CreateWarehouseItemOnProductCreatedHandler
{
    public static void Handle(ProductCreated domainEvent, WarehouseDbContext dbContext)
    {
        var warehouseItem = WarehouseItem.Create(
            catalogId: domainEvent.AggregateId,
            name: domainEvent.Name,
            initialQuantity: 0
        );

        dbContext.Items.Add(warehouseItem);
    }
}
