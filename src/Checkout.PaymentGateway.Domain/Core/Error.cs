using System;
using System.Diagnostics.CodeAnalysis;

namespace Checkout.PaymentGateway.Domain.Core
{
    /// <summary>
    /// Error class explaining what when wrong during an action
    /// This error contains a simple code. All code are listed under <see cref="Errors"/>. All these errors should be documented publicly.
    /// </summary>
    public class Error : IEquatable<Error>
    {
        public string Code { get; private set; }
        
        protected Error() { }

        public Error(string code)
        {
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));
            Code = code;
        }

        public override string ToString() => Code;

        public bool Equals(Error? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Code == other.Code;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Error) obj);
        }

        public override int GetHashCode() => Code.GetHashCode();
    }

    /// <summary>
    /// Error raised when something expected happens (exception)
    /// </summary>
    public class UnexpectedError : Error
    {
        private UnexpectedError(string code)
            : base(code) { }

        public UnexpectedError(Exception exception)
            : this(exception?.GetType().Name ?? throw new ArgumentNullException(nameof(exception))) { }
    }

    /// <summary>
    /// Error raised when an item cannot be found
    /// </summary>
    public class UnknownItemError : Error
    {
        public UnknownItemError(string code)
            : base(code) { }
    }
}