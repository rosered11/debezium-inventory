using InventoryManagement.Domain.Client.Models.MasterClient;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Client
{
    public interface IMasterClient
    {
        Task<PartModel> GetPartsAsync();
        Task<WarehouseLocationModel> GetWarehouseNoAsync();
    }
    public class MasterClient : IMasterClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public MasterClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _config = configuration;
        }

        public async Task<PartModel> GetPartsAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["External:Masterdata:SecretToken"]);
            var response = await _httpClient.GetAsync(_config["External:Masterdata:GetPart"]);
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PartModel>(resultString);
        }

        public async Task<WarehouseLocationModel> GetWarehouseNoAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["External:Masterdata:SecretToken"]);
            var response = await _httpClient.GetAsync(_config["External:Masterdata:GetWarehouse"]);
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WarehouseLocationModel>(resultString);
        }
    }
}
