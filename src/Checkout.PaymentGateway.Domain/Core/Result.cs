using System;

namespace Checkout.PaymentGateway.Domain.Core
{
    /// <summary>
    /// Result is a very important concept in this system.
    /// Most value objects, actions or handlers return a result to indicate if the action was successful or not.
    /// </summary>
    public class Result
    {
        public bool IsSuccess => Error is null;
        public Error? Error { get; private set; } // If successful there is no error

        internal Result() { }
        internal Result(Error error) =>
            Error = error ?? throw new ArgumentNullException(nameof(error));

        #region Static members
        public static Result<TValue> Ok<TValue>(TValue value) 
            => new Result<TValue>(value);

        public static Result<TValue> Fail<TValue>(Error error)
        {
            if (error is null) throw new ArgumentNullException(nameof(error));
            return new Result<TValue>(error);
        }
        #endregion
    }

    /// <summary>
    /// Result is a very important concept in this system.
    /// Most value objects, actions or handlers return a result to indicate if the action was successful or not.
    /// </summary>
    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        internal Result(TValue value)
            => _value = value;
        internal Result(Error error)
            : base(error) { }

        public TValue GetValue() =>
            !IsSuccess
                ? throw new InvalidOperationException("Value cannot be retrieved in case of failure.")
                : _value;
    }
}