using Boxed.Mapping;
using InventoryManagement.Domain.DTO.Requests;
using InventoryManagement.Domain.Mapper;
using System.Collections.Generic;
using Xunit;

namespace InventoryManagement.Tests.Mapper.EventRequestMapperTest
{
    public class MapTest
    {
        [Fact]
        public void WhenMappingReceiveEventToInventoryRequest_ShouldReturnIsCorrect()
        {
            IMapper<Domain.DTO.Requests.Part, Domain.DTO.Requests.Part> childMapper = new ReceiveEventPartReqToInventoryRequestPartMapper();
            IMapper<Domain.DTO.Requests.ReceiveEvent, Domain.DTO.Requests.InventoryRequest> service = new ReceiveEventReqToInventoryRequestMapper(childMapper);
            var input = new ReceiveEvent
            {
                WarehouseLocationNo = "wh",
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "n1",
                        Qty = 5,
                        Uom = "uom"
                    }
                }
            };

            var result = service.Map(input);

            Assert.Single(result.Parts);
            Assert.Collection(result.Parts, x => {
                Assert.Equal("n1", x.No);
                Assert.Equal(5, x.Qty);
                Assert.Equal("uom", x.Uom);
            });
            Assert.Equal("wh", result.WarehouseLocationNo);
        }

        [Fact]
        public void WhenMappingRequestEventToInventoryRequest_ShouldReturnIsCorrect()
        {
            IMapper<Domain.DTO.Requests.Part, Domain.DTO.Requests.Part> childMapper = new ReceiveEventPartReqToInventoryRequestPartMapper();
            IMapper<Domain.DTO.Requests.RequestEvent, Domain.DTO.Requests.InventoryRequest> service = new RequestEventReqToInventoryRequestMapper(childMapper);
            var input = new RequestEvent
            {
                WarehouseLocationNo = "wh",
                Parts = new List<Part>
                {
                    new Part
                    {
                        No = "n1",
                        Qty = 5,
                        Uom = "uom"
                    }
                }
            };

            var result = service.Map(input);

            Assert.Single(result.Parts);
            Assert.Collection(result.Parts, x => {
                Assert.Equal("n1", x.No);
                Assert.Equal(5, x.Qty);
                Assert.Equal("uom", x.Uom);
            });
            Assert.Equal("wh", result.WarehouseLocationNo);
        }
    }
}
