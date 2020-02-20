using Checkout.PaymentGateway.Domain.Core;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Security
{
    public interface IApiKeyService
    {
        public Task<Result<ICollection<Claim>>> Authenticate(string bearer);
    }
}