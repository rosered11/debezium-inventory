using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace InventoryManagement.Api
{
    public static class RetryPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IConfiguration configuration)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                    configuration.GetValue<int>("Client:Retry:RetryCount"),
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }
    }
    
    public static class CircuitBreakerPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(IConfiguration configuration)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == System.Net.HttpStatusCode.NotFound)
                .CircuitBreakerAsync(
                    configuration.GetValue<int>("Client:CircuitBreaker:HandledEventsAllowedBeforeBreaking")
                    , TimeSpan.FromSeconds(configuration.GetValue<double>("Client:CircuitBreaker:DurationOfBreakSeconds"))
                );
        }
    }
}
