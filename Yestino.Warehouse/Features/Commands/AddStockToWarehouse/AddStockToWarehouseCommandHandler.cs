using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.Commands.AddStockToWarehouse;

public static class AddStockToWarehouseCommandHandler
{
    public static int Handle(AddStockToWarehouseCommand commmand, WarehouseDbContext context)
    {
        var item = context.Items.FirstOrDefault(i => i.Id.Equals(commmand.productId));

        if(item == null)
        {
            throw new InvalidOperationException("Item not found in warehouse");
        }

        item.TotalQuantity += commmand.addedQuantity;

        return item.TotalQuantity;
    }
}
