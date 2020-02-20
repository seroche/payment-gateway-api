using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Checkout.PaymentGateway.Domain.Core
{
    /// <summary>
    /// Base enumeration class. This is somehow a smarter enum.
    /// </summary>
    public abstract class Enumeration : ValueObject
    {
        /// <summary>
        /// Returns all possible values of an enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>()
            where T : Enumeration
            => GetAll(typeof(T)).OfType<T>();

        /// <summary>
        /// Returns all possible values of an enumeration.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<object> GetAll(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return type
                .GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    BindingFlags.GetProperty |
                    BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null));
        }
    }
}