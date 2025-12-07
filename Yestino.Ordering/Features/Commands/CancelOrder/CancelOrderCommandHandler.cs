using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.Commands.CancelOrder;

public static class CancelOrderCommandHandler
{
    public static void handle(CancelOrderCommand command, OrderingDbContext context)
    {
        var order = context.Orders.First(o => o.Id == command.OrderId);
    }
}
