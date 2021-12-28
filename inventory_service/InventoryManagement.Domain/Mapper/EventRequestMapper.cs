using Boxed.Mapping;
using InventoryManagement.Domain.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Mapper
{
    public class ReceiveEventReqToInventoryRequestMapper : IMapper<DTO.Requests.ReceiveEvent, DTO.Requests.InventoryRequest>
    {
        private readonly IMapper<Part, Part> _childMapper;

        public ReceiveEventReqToInventoryRequestMapper(IMapper<DTO.Requests.Part, DTO.Requests.Part> childMapper)
        {
            _childMapper = childMapper;
        }

        public void Map(ReceiveEvent source, InventoryRequest destination)
        {
            destination.Parts = _childMapper.MapList(source.Parts);
            destination.WarehouseLocationNo = source.WarehouseLocationNo;
        }
    }

    public class RequestEventReqToInventoryRequestMapper : IMapper<RequestEvent, InventoryRequest>
    {
        private readonly IMapper<Part, Part> _childMapper;

        public RequestEventReqToInventoryRequestMapper(IMapper<DTO.Requests.Part, DTO.Requests.Part> childMapper)
        {
            _childMapper = childMapper;
        }

        public void Map(RequestEvent source, InventoryRequest destination)
        {
            destination.Parts = _childMapper.MapList(source.Parts);
            destination.WarehouseLocationNo = source.WarehouseLocationNo;
        }
    }

    public class ReceiveEventPartReqToInventoryRequestPartMapper : IMapper<DTO.Requests.Part, DTO.Requests.Part>
    {
        public void Map(Part source, Part destination)
        {
            destination.No = source.No;
            destination.Qty = source.Qty;
            destination.Uom = source.Uom;
        }
    }
}
