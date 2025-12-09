using Yestino.Ordering.Application;
using Yestino.Ordering.Infrastructure;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Ordering.Features.EventHandlers;

public static class ChangeProductQuantityOnWarehouseStockChangeHandler
{
    public static void Handle(WarehouseAvailableStockChanged domainEvent, OrderingDbContext dbContext)
    {
        var productModel = dbContext.ProductReadModels.First(m => m.Id == domainEvent.CatalogId);

        productModel.StockQuantity = domainEvent.NewQuantity;
    }
}
