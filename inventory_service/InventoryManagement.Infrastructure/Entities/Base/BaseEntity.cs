using Sieve.Attributes;
using System;

namespace InventoryManagement.Infrastructure.Entities.Base
{
    public class BaseEntity
    {
        [Sieve(CanFilter = true, CanSort = true, Name = "created")]
        public DateTimeOffset? CreatedAt { get; set; }
        [Sieve(CanFilter = true, CanSort = true, Name = "updated")]
        public DateTimeOffset? UpdatedAt { get; set; }
        public string UserCreated { get; set; }
        public string UserUpdated { get; set; }
        public string UserIdCreated { get; set; }
        public string UserIdUpdated { get; set; }
    }
}
