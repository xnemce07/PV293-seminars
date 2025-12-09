using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.Common.Domain;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Warehouse.Domain;

public class WarehouseItem : AggregateRoot
{
    public string Name { get; private set; }

    private int _totalQuantity;
    public int TotalQuantity { get; private set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => TotalQuantity - ReservedQuantity;

    public Guid CatalogId;

    public static WarehouseItem Create(Guid catalogId,string name, int initialQuantity)
    {
        var item = new WarehouseItem
        {
            CatalogId = catalogId,
            Name = name,
            _totalQuantity = initialQuantity,
            ReservedQuantity = 0
        };

        return item;
    }

    public void AddQuantity(int quantity)
    {
        this.TotalQuantity += quantity;
        this.RaiseDomainEvent(new WarehouseAvailableStockChanged(this.CatalogId, this.AvailableQuantity));
    }

    public void ChangeReservedStock(int difference)
    {
        if(difference > 0 && difference > this.AvailableQuantity)
        {
            throw new InvalidOperationException("Not enough available stock to reserve the requested quantity.");
        }

        if (difference < 0 && difference > this.ReservedQuantity)
        {
            this.ReservedQuantity = 0;
        } else
        {
            this.ReservedQuantity += difference;
        }


        this.RaiseDomainEvent(new WarehouseAvailableStockChanged(this.CatalogId, this.AvailableQuantity));
    }
}
