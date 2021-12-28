using Boxed.Mapping;
using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Infrastructure.Entities;
using System;
using System.Text.Json;

namespace InventoryManagement.Domain.Mapper
{
    public class OutboxEventMapper : IMapper<DTO.Requests.Inventory, Infrastructure.Entities.OutboxEvent>
    {
        public void Map(DTO.Requests.Inventory source, Infrastructure.Entities.OutboxEvent destination)
        {
            destination.Id = Guid.NewGuid();
            destination.Payload = JsonSerializer.Serialize(source);
            destination.Type = Utilities.InventoryTypeUtility.GetInventoryType(Enum.Parse<Utilities.InputTransactionType>(source.TransactionType));
        }
    }

    public class OutboxEventForRequestInventory : IMapper<DTO.Requests.InventoryRequest, Infrastructure.Entities.OutboxEvent>
    {
        public void Map(InventoryRequest source, OutboxEvent destination)
        {
            destination.Id = Guid.NewGuid();
            destination.Payload = JsonSerializer.Serialize(source);
        }
    }

    public class ReceiveEventToOutboxEventMapper : IMapper<DTO.Requests.ReceiveEvent, Infrastructure.Entities.OutboxEvent>
    {
        public void Map(ReceiveEvent source, OutboxEvent destination)
        {
            destination.Id = Guid.NewGuid();
            destination.Payload = JsonSerializer.Serialize(source);
        }
    }

    public class RequestEventToOutboxEventMapper : IMapper<DTO.Requests.RequestEvent, Infrastructure.Entities.OutboxEvent>
    {
        public void Map(RequestEvent source, OutboxEvent destination)
        {
            destination.Id = Guid.NewGuid();
            destination.Payload = JsonSerializer.Serialize(source);
        }
    }
}
