using Yestino.Common.Domain;
using Yestino.OrderingContracts.DomainEvents;

namespace Yestino.Ordering.Domain;

public class Order : AggregateRoot
{
    public DateTimeOffset CreateAt { get; private set; }
    public string CustomerAddress { get; private set; }
    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items;

    public static Order Create(string customerAddress, ICollection<CreateOrderItemModel> items)
    {
        var order = new Order
        {
            CustomerAddress = customerAddress,
            CreateAt = DateTimeOffset.Now,
            Status = OrderStatus.Created,
        };

        order._items.AddRange(items.Select(i => new OrderItem
        {
            OrderId = order.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Price = i.Price,
            Quantity = i.Quantity,
        }));

        order.RaiseDomainEvent(
            new OrderCreated(
                AggregateId: order.Id,
                Items: order.Items.Select(i => new OrderCreatedItem(
                    ProductId: i.ProductId,
                    ProductName: i.ProductName,
                    Price: i.Price,
                    Quantity: i.Quantity
                )).ToList())
        );

        return order;
    }

    public void cancelOrder()
    {
        this.Status = OrderStatus.Cancelled;
        this.RaiseDomainEvent(
            new CancelOrder(
                OrderId: this.Id,
                Items: this.Items.Select(i => new OrderCreatedItem(
                    ProductId: i.ProductId,
                    ProductName: i.ProductName,
                    Price: i.Price,
                    Quantity: i.Quantity
                )).ToList()));
    }
}

public record CreateOrderItemModel(Guid ProductId, int Quantity, decimal Price, string ProductName);

public enum OrderStatus
{
    Created,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}