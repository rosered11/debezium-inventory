using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Worker.Client
{
    public interface IInventoryClient
    {
        Task PostReceiveEvent(string payload, string eventId, CancellationToken cancellationToken);
    }
    public class InventoryClient : IInventoryClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string SECRET_KEY = "APISECRET";
        private const string EVENT_TYPE = "EventType";
        private const string EVENT_ID = "EventId";
        private readonly string _eventType;
        private readonly string _secretKey;

        public InventoryClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _eventType = _eventType ?? _configuration["External:Inventory:EventType"];
            _secretKey = _secretKey ?? _configuration["External:Inventory:SecretKey"];
        }

        public async Task PostReceiveEvent(string payload, string eventId, CancellationToken cancellationToken)
        {
            
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            SetHeaders(eventId);

            var response = await _httpClient.PostAsync(_configuration["External:Inventory:ReceiveEvent"], content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        internal void SetHeaders(string eventId)
        {
            // Setup headers
            // Check secret key in header
            if (!_httpClient.DefaultRequestHeaders.TryGetValues(SECRET_KEY, out _))
            {
                _httpClient.DefaultRequestHeaders.Add(SECRET_KEY, _secretKey);
            }

            // Check event type in header
            if (!_httpClient.DefaultRequestHeaders.TryGetValues(EVENT_TYPE, out _))
            {
                _httpClient.DefaultRequestHeaders.Add(EVENT_TYPE, _eventType);
            }

            // Check event id in header
            if (_httpClient.DefaultRequestHeaders.TryGetValues(EVENT_ID, out _))
            {
                _httpClient.DefaultRequestHeaders.Remove(EVENT_ID);
            }
            _httpClient.DefaultRequestHeaders.Add(EVENT_ID, eventId);
        }
    }
}
