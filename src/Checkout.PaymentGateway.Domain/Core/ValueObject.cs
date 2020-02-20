using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkout.PaymentGateway.Domain.Core
{
    /// <summary>
    /// Base value object class.
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        #region Equality
        public bool Equals(ValueObject other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;

            using var thisValues = GetAtomicValues().GetEnumerator();
            using var otherValues = other.GetAtomicValues().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (thisValues.Current is null ^ otherValues.Current is null)
                    return false;
                if (!(thisValues.Current is null) && !thisValues.Current.Equals(otherValues.Current))
                    return false;
            }

            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public override bool Equals(object? obj)
            => obj is { } && obj is ValueObject o && Equals(o);

        public override int GetHashCode()
            => GetAtomicValues()
                .Select(x => !(x is null) ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y); 
        #endregion

        public override string ToString()
            => string.Join(", ", GetAtomicValues().Where(x => !(x is null)));

        /// <summary>
        /// Returns atomic values used to compare ValueObjects.
        /// </summary>
        protected abstract IEnumerable<object> GetAtomicValues();
    }
}