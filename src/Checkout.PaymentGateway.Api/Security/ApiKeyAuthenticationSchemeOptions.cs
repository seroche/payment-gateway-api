using Microsoft.AspNetCore.Authentication;

namespace Checkout.PaymentGateway.Api.Security
{
    public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "ApiKeyScheme";
    }
}