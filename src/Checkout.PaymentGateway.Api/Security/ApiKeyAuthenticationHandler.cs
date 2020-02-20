using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Checkout.PaymentGateway.Api.Security
{
    /// <summary>
    /// Simplistic Authentication handler validating ApiKeys passed through the standard Authorization header
    /// </summary>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
    {
        private readonly IApiKeyService _service;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyService service)
            : base(options, logger, encoder, clock)
            => _service = service;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var value))
                return AuthenticateResult.Fail("Authentication header cannot be found.");

            if (!AuthenticationHeaderValue.TryParse(value, out var header))
                return AuthenticateResult.Fail("Authentication header is not valid.");

            var res = await _service.Authenticate(header.Parameter);
            if (!res.IsSuccess)
                return AuthenticateResult.Fail($"ApiKey is either unknown or invalid [{res.Error?.Code ?? "N/A"}].");

            var principal = new ClaimsPrincipal(new ClaimsIdentity(res.GetValue(), ApiKeyAuthenticationSchemeOptions.Scheme));

            return AuthenticateResult.Success(new AuthenticationTicket(principal, ApiKeyAuthenticationSchemeOptions.Scheme));
        }
    }
}
