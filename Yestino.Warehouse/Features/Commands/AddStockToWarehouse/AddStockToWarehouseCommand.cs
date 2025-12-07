using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yestino.Warehouse.Features.Commands.AddStockToWarehouse;

public record AddStockToWarehouseCommand(Guid productId, int addedQuantity);
