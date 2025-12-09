using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.Common.Domain;

namespace Yestino.OrderingContracts.DomainEvents;

public record OrderCancelled(Guid OrderId, ICollection<OrderCancelledItem> Items) : DomainEvent(OrderId);


public record OrderCancelledItem(Guid ProductId, int Quantity);