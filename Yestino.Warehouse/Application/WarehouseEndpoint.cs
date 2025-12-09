using Carter;

namespace Yestino.Warehouse.Application;

public abstract class WarehouseEndpoint : CarterModule
{
    protected WarehouseEndpoint() : base("warehouse")
    {
        WithTags("Warehouse");
    }
}
