using Microsoft.EntityFrameworkCore;
using Wolverine;
using Yestino.Common.Infrastructure.Persistence;
using Yestino.Warehouse.Domain;

namespace Yestino.Warehouse.Infrastructure;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options, IMessageBus bus)
    : DbContextBase(options, bus)
{

    public DbSet<WarehouseItem> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("warehouse");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
    }
}