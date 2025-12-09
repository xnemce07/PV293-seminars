using Yestino.Ordering.Domain;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.Commands.CreateOrder;

public static class CreateOrderCommandHandler
{
    public static Guid Handle(CreateOrderCommand command, OrderingDbContext dbContext)
    {

        foreach (var item in command.Items)
        {
            if (item.Quantity > dbContext.ProductReadModels.First(p => p.Id == item.ProductId).StockQuantity)
            {
                throw new InvalidOperationException($"Not enough stock for product {item.ProductId}");
            }
        }

        var order = Order.Create(
            command.CustomerAddress,
            command.Items
                .Select(i => new CreateOrderItemModel(i.ProductId, i.Quantity, 123, "todo"))
                .ToList()
        );

        dbContext.Orders.Add(order);

        return order.Id;
    }
}