using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Wolverine;
using Yestino.Warehouse.Application;

namespace Yestino.Warehouse.Features.Commands.AddStockToWarehouse;

public class AddStockToWarehouseEndpoint : WarehouseEndpoint
{

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/warehouse",
            async ([FromBody] AddStockToWarehouseCommand command, IMessageBus bus, CancellationToken cancellationToken) =>
                await bus.InvokeAsync<Guid>(command, cancellationToken));
    }
}
