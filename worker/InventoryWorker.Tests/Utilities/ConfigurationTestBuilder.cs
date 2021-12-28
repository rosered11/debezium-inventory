using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWorker.Tests.Utilities
{
    public static class ConfigurationTestBuilder
    {
        public static IConfiguration Build(Dictionary<string, string> input)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(input)
                .Build();
        }
    }
}
