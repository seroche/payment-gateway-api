using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Core.Assertions
{
    public class ResultAssertion<T>
    {
        public T Object { get; }

        public ResultAssertion(T current)
            => Object = Equals(default(T), current)
                ? throw new InvalidOperationException($"Assertions cannot be executed on a null '{typeof(T).Name}'.")
                : current;

        #region Self
        public ResultAssertion<T> IsEqual(T expected)
            => IsEqual(expected, res => res);

        public ResultAssertion<T> IsNotEqual(T expected)
            => IsNotEqual(expected, res => res);

        public ResultAssertion<T> IsNotEmpty()
            => IsNotEmpty(res => res);

        public ResultAssertion<T> IsNotNull()
            => IsNotNull(res => res);

        public ResultAssertion<T> IsIn(IEnumerable<T> expected)
            => IsIn(expected, res => res);
        #endregion

        #region Selector
        public ResultAssertion<T> IsEqual<TAssert>(TAssert expected, Func<T, TAssert> selector)
        {
            Assert.Equal(expected, selector(Object));
            return this;
        }

        public ResultAssertion<T> IsNotEqual<TAssert>(TAssert expected, Func<T, TAssert> selector)
        {
            Assert.NotEqual(expected, selector(Object));
            return this;
        }

        public ResultAssertion<T> IsTrue(Func<T, bool> predicate)
        {
            Assert.True(predicate(Object));
            return this;
        }

        public ResultAssertion<T> IsFalse(Func<T, bool> predicate)
        {
            Assert.False(predicate(Object));
            return this;
        }
        
        public ResultAssertion<T> IsNotEmpty(Func<T, object> selector)
        {
            var obj = selector(Object);
            if (obj is IEnumerable enumerable) Assert.NotEmpty(enumerable);
            else if (obj is Guid guid) Assert.NotEqual(Guid.Empty, guid);
            else throw new NotImplementedException();
            return this;
        }

        public ResultAssertion<T> IsNull<TAssert>(Func<T, TAssert> selector)
        {
            Assert.Null(selector(Object));
            return this;
        }

        public ResultAssertion<T> IsNotNull<TAssert>(Func<T, TAssert> selector)
        {
            Assert.NotNull(selector(Object));
            return this;
        }

        public ResultAssertion<T> IsIn<TAssert>(IEnumerable<TAssert> expected, Func<T, TAssert> selector)
        {
            Assert.Contains(expected, x => Equals(x, selector(Object)));
            return this;
        }
        #endregion

        public ResultAssertion<T> ForEach<TItem>(Func<T, IEnumerable<TItem>> selector, Action<ResultAssertion<TItem>> assertions)
        {
            IsNotNull(selector);
            foreach (var req in selector(Object))
                assertions(new ResultAssertion<TItem>(req));
            return this;
        }
    }
}