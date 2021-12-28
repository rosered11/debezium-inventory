using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace InventoryWorker
{
    public class ConsumerHandler
    {
        private readonly IList<ConsumeResult<Ignore, string>> _consumersResult;
        private readonly object queueLock = new object();

        public void Setter(ConsumeResult<Ignore, string> consumeResult)
        {
            lock (queueLock)
            {
                _consumersResult.Add(consumeResult);
            }
        }



        public ConsumerHandler()
        {
            _consumersResult = new List<ConsumeResult<Ignore, string>>();
        }

        public void TrackStart(ConsumeResult<Ignore, string> consumeResult)
        {
            _consumersResult.Add(consumeResult);
        }

        public void TrackComplete(ConsumeResult<Ignore, string> consumeResult)
        {
            _consumersResult.Remove(consumeResult);
        }

        public bool IsCommit()
        {
            return _consumersResult.Count == 0;
        }
    }
}
