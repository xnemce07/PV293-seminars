using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.Common.Domain;

namespace Yestino.OrderingContracts.DomainEvents;

public record CancelOrder(Guid OrderId, ICollection<OrderCreatedItem> Items) : DomainEvent(OrderId);
