using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain
{
    public static class Errors
    {
        #region Errors raised within the payment request feature
        public static Error InvalidCurrency => new Error(nameof(InvalidCurrency));
        public static Error InvalidAmount => new Error(nameof(InvalidAmount));
        public static Error InvalidCardNumber => new Error(nameof(InvalidCardNumber));
        public static Error InvalidExpiryDate => new Error(nameof(InvalidExpiryDate));
        public static Error InvalidCvv => new Error(nameof(InvalidCvv));
        public static Error ExpiredCard => new Error(nameof(ExpiredCard));
        #endregion

        #region Errors raised within the payment feature
        public static Error UnknownPayment => new UnknownItemError(nameof(UnknownPayment));
        #endregion

        #region Errors raised within the jobs feature
        public static Error UnknownJob => new UnknownItemError(nameof(UnknownJob));
        #endregion

        #region Security related errors
        public static Error UnauthorizedAccess => new Error(nameof(UnauthorizedAccess));
        public static Error MissingRequiredParameter => new Error(nameof(MissingRequiredParameter));
        public static Error InvalidBearer => new Error(nameof(InvalidBearer));
        public static Error UnknownApiKey => new Error(nameof(UnknownApiKey));
        public static Error UnauthorizedAccessToPayment => new Error(nameof(UnauthorizedAccessToPayment));
        #endregion
    }
}