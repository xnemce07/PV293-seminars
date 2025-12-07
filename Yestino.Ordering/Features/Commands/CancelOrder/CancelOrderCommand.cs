using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yestino.Ordering.Features.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId);
