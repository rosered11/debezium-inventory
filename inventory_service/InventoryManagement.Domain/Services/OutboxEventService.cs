using Boxed.Mapping;
using InventoryManagement.Domain.Utilities;
using InventoryManagement.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManagement.Domain.Services
{
    public class OutboxEventService
    {
        private readonly IMapper<DTO.Requests.Inventory, OutboxEvent> _outboxMapper;
        private readonly IMapper<DTO.Requests.InventoryRequest, OutboxEvent> _outboxInventoryRequestMapper;
        private readonly IMapper<DTO.Requests.ReceiveEvent, OutboxEvent> _receiveEventToOutboxMapper;
        private readonly IMapper<DTO.Requests.RequestEvent, OutboxEvent> _requestEventToOutboxMapper;

        public OutboxEventService(IMapper<DTO.Requests.Inventory, Infrastructure.Entities.OutboxEvent> outboxMapper
            , IMapper<DTO.Requests.InventoryRequest, Infrastructure.Entities.OutboxEvent> outboxInventoryRequestMapper
            , IMapper<Domain.DTO.Requests.ReceiveEvent, Infrastructure.Entities.OutboxEvent> receiveEventToOutboxMapper
            , IMapper<Domain.DTO.Requests.RequestEvent, Infrastructure.Entities.OutboxEvent> requestEventToOutboxMapper)
        {
            _outboxMapper = outboxMapper;
            _outboxInventoryRequestMapper = outboxInventoryRequestMapper;
            _receiveEventToOutboxMapper = receiveEventToOutboxMapper;
            _requestEventToOutboxMapper = requestEventToOutboxMapper;
        }
        public OutboxEvent CreateOutboxEventPerPart(DTO.Requests.Inventory request, Inventory currentInventory)
        {
            OutboxEvent outboxEvent = _outboxMapper.Map(request);
            outboxEvent.AggregateId = request.PartNo;
            outboxEvent.AggregateType = GetAggregateTypeOfPartFromInputTransactionType(request.TransactionType);
            return CheckAvailableQuantity(outboxEvent, request, currentInventory);
        }

        public OutboxEvent CreateOutboxEventPerOrder(DTO.Requests.InventoryRequest request
            , InputTransactionType inputType
            , string orderNo)
        {
            OutboxEvent outboxEvent = _outboxInventoryRequestMapper.Map(request);
            outboxEvent.AggregateId = orderNo;
            outboxEvent.Type = InventoryTypeUtility.GetInventoryType(inputType);
            outboxEvent.AggregateType = GetAggregateTypeOfOrderFromInputTransactionType(inputType.ToString());
            return outboxEvent;
        }

        public OutboxEvent CreateOutboxEventPerOrder(DTO.Requests.ReceiveEvent request
            , InputTransactionType inputType
            , string orderNo)
        {
            OutboxEvent outboxEvent = _receiveEventToOutboxMapper.Map(request);
            outboxEvent.AggregateId = orderNo;
            outboxEvent.Type = InventoryTypeUtility.GetInventoryType(inputType);
            outboxEvent.AggregateType = GetAggregateTypeOfOrderFromInputTransactionType(inputType.ToString());
            return outboxEvent;
        }

        public OutboxEvent CreateOutboxEventPerOrder(DTO.Requests.RequestEvent request
            , InputTransactionType inputType
            , string orderNo)
        {
            OutboxEvent outboxEvent = _requestEventToOutboxMapper.Map(request);
            outboxEvent.AggregateId = orderNo;
            outboxEvent.Type = InventoryTypeUtility.GetInventoryType(inputType);
            outboxEvent.AggregateType = GetAggregateTypeOfOrderFromInputTransactionType(inputType.ToString());
            return outboxEvent;
        }

        internal OutboxEvent CheckAvailableQuantity(OutboxEvent outboxEvent, DTO.Requests.Inventory request, Inventory currentInventory)
        {
            if (outboxEvent.Type == nameof(InventoryTransactionType.STOCK_BOOKED))
            {
                outboxEvent = HandleInventoryTypeForAvailableQuantity(outboxEvent, request.Quantity, currentInventory?.AvailableQty);
            }
            return outboxEvent;
        }

        internal OutboxEvent CheckAvailableQuantity(OutboxEvent outboxEvent, DTO.Requests.InventoryRequest request, IEnumerable<Inventory> currentInventoryList)
        {
            if(currentInventoryList == null)
            {
                outboxEvent.Type = nameof(InventoryTransactionType.STOCK_BOOKING_FAILED);
            }
            else
            {
                if (outboxEvent.Type == nameof(InventoryTransactionType.STOCK_BOOKED))
                {
                    foreach (var part in request.Parts)
                    {
                        var currentInventory = currentInventoryList.FirstOrDefault(x => x.PartNo == part.No);
                        outboxEvent = HandleInventoryTypeForAvailableQuantity(outboxEvent, part.Qty, currentInventory?.AvailableQty);
                        if (outboxEvent.Type == nameof(InventoryTransactionType.STOCK_BOOKING_FAILED))
                        {
                            break;
                        }
                    }
                }
            }
            return outboxEvent;
        }

        private OutboxEvent HandleInventoryTypeForAvailableQuantity(OutboxEvent outboxEvent, int? requestQuantity, int? currentAvailableQty)
        {
            if (currentAvailableQty == null || requestQuantity > currentAvailableQty)
            {
                outboxEvent.Type = nameof(InventoryTransactionType.STOCK_BOOKING_FAILED);
            }
            return outboxEvent;
        }

        internal string GetAggregateTypeOfPartFromInputTransactionType(string inputTypeString)
        {
            switch(Enum.Parse<InputTransactionType>(inputTypeString))
            {
                case InputTransactionType.RC_SUBMITTED:
                case InputTransactionType.RC_ACCEPTED:
                case InputTransactionType.RC_COMPLETED:
                    return OutboxEventConstant.RECEIVE_PART;
                case InputTransactionType.RQ_SUBMITTED:
                case InputTransactionType.RQ_DELIVERED:
                    return OutboxEventConstant.REQUEST_PART;
            }

            throw new ArgumentException("Cann't get aggregate type from input transaction type.", inputTypeString);
        }

        internal string GetAggregateTypeOfOrderFromInputTransactionType(string inputTypeString)
        {
            switch (Enum.Parse<InputTransactionType>(inputTypeString))
            {
                case InputTransactionType.RC_SUBMITTED:
                case InputTransactionType.RC_ACCEPTED:
                case InputTransactionType.RC_COMPLETED:
                    return OutboxEventConstant.RECEIVE_ORDER;
                case InputTransactionType.RQ_SUBMITTED:
                case InputTransactionType.RQ_DELIVERED:
                    return OutboxEventConstant.REQUEST_ORDER;
            }

            throw new ArgumentException("Cann't get aggregate type from input transaction type.", inputTypeString);
        }
    }
}
