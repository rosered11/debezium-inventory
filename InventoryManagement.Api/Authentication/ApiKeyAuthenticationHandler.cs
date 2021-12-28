using InventoryManagement.Domain.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Authentication
{
    public static class AuthenConstant
    {
        public const string Worker = "Worker";
        public const string DefaultScheme = "APIKey";
        public const string ApiKeyHeaderName = "APISECRET";
    }
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        
        public string Scheme => AuthenConstant.DefaultScheme;
        public string AuthenticationType = AuthenConstant.DefaultScheme;
    }

    public class ApiKey
    {
        public ApiKey(string owner, string key, IReadOnlyCollection<string> roles)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        }

        public string Owner { get; }
        public string Key { get; }
        public IReadOnlyCollection<string> Roles { get; }
    }

    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options)
        {
            return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(AuthenConstant.DefaultScheme, options);
        }
    }

    public class GetApiKeyFromConfig 
    {
        private readonly IDictionary<string, ApiKey> _apiKeys;

        public GetApiKeyFromConfig(IConfiguration configuration)
        {
            var existingApiKeys = new List<ApiKey>
            {
                new ApiKey(AuthenConstant.Worker, configuration.GetValue<string>("Authentication:Worker:ApiKey"),
                    new List<string>
                    {
                        AuthenConstant.Worker
                    }
                )
            };

            _apiKeys = existingApiKeys.ToDictionary(x => x.Key, x => x);
        }

        public Task<ApiKey> Execute(string providedApiKey)
        {
            _apiKeys.TryGetValue(providedApiKey, out var key);
            return Task.FromResult(key);
        }
    }

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly GetApiKeyFromConfig _getApiKeyQuery;
        private readonly ILogger<ApiKeyAuthenticationHandler> _loggerInfo;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            GetApiKeyFromConfig getApiKeyQuery,
            ILogger<ApiKeyAuthenticationHandler> loggerInfo
            ) : base(options, logger, encoder, clock)
        {
            _getApiKeyQuery = getApiKeyQuery ?? throw new ArgumentNullException();
            _loggerInfo = loggerInfo;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(AuthenConstant.ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                _loggerInfo.LogWarning($"Header {AuthenConstant.ApiKeyHeaderName} not found in the request.");
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                _loggerInfo.LogWarning($"Header {AuthenConstant.ApiKeyHeaderName} don't has value.");
                return AuthenticateResult.NoResult();
            }

            var existingApiKey = await _getApiKeyQuery.Execute(providedApiKey);

            if (existingApiKey != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, existingApiKey.Owner),
                    new Claim(ClaimConstant.UserId, existingApiKey.Owner)
                };

                claims.AddRange(existingApiKey.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
                var identities = new List<ClaimsIdentity> { identity };
                var principal = new ClaimsPrincipal(identities);
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                _loggerInfo.LogInformation("Authorize via api key success.");
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.NoResult();
        }
    }
}
