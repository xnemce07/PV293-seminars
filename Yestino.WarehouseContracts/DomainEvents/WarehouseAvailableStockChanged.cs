using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yestino.Common.Domain;

namespace Yestino.WarehouseContracts.DomainEvents;

public record WarehouseAvailableStockChanged(Guid CatalogId, int NewQuantity) : DomainEvent(CatalogId);
