using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;
using Yestino.Ordering.Application;

namespace Yestino.Ordering.Features.Commands.CancelOrder;

public class CancelOrderEndpoint : OrderingEndpoint
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders/cancel",
            async ([FromBody] CancelOrderCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
                await bus.InvokeAsync<Guid>(command, cancellationToken));
    }
}
