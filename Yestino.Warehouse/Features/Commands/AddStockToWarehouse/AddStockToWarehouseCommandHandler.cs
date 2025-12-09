using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.Commands.AddStockToWarehouse;

public static class AddStockToWarehouseCommandHandler
{
    public static int Handle(AddStockToWarehouseCommand command, WarehouseDbContext context)
    {
        var item = context.Items.FirstOrDefault(i => i.Id.Equals(command.productId));

        if(item == null)
        {
            throw new InvalidOperationException("Item not found in warehouse");
        }

        //item.TotalQuantity += commmand.addedQuantity;
        item.AddQuantity(command.addedQuantity);

        return item.TotalQuantity;
    }
}
