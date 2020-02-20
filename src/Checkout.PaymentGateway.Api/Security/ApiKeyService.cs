using System.Collections.Generic;
using Checkout.PaymentGateway.Domain;
using Checkout.PaymentGateway.Domain.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Security
{
    public class ApiKeyService : IApiKeyService
    {
        // Should get this from a db
        private static readonly Dictionary<string, string> ApiKeys = new Dictionary<string, string>
        {
            ["ap_123"] = "62CB1521-7197-4B5A-9A9F-BA39BE675248",
            ["ap_456"] = "48A849B5-E1B2-4FDA-89AF-9AF174F75C80"
        };

        public async Task<Result<ICollection<Claim>>> Authenticate(string bearer)
        {
            await Task.CompletedTask;
            if (bearer is null)
                return Result.Fail<ICollection<Claim>>(Errors.InvalidBearer);
            if (!ApiKeys.ContainsKey(bearer))
                return Result.Fail<ICollection<Claim>>(Errors.UnknownApiKey);

            return Result.Ok<ICollection<Claim>>(new[]
            {
                // Add other claims here
                new Claim(CustomClaimTypes.MerchantId, ApiKeys[bearer])
            });
        }
    }
}
