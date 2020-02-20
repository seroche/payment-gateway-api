using Checkout.PaymentGateway.Api.Features.Payments;

namespace Checkout.PaymentGateway.Api.IntegrationTests
{
    public static class Data
    {
        public static string ValidApiKey
            => "ap_123";
        public static string ValidApiKey2
            => "ap_456";

        public static Request.Command ValidRequestPaymentCommand
            => new Request.Command
            {
                Amount = 100,
                Currency = "HKD",
                Description = "Coco",
                Card = new Request.Command.CardDetails
                {
                    Cvv = "123",
                    Number = "4111 1111 1111 1111",
                    HolderName = "Sebastien R",
                    ExpiryMonth = 10,
                    ExpiryYear = 2021
                }
            };
    }
}
