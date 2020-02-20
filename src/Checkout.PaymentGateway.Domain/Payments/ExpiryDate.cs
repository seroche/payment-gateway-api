using System;
using System.Collections.Generic;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain.Payments
{
    public class ExpiryDate : ValueObject
    {
        public int Month { get; private set; }
        public int Year { get; private set; }
        public DateTime Date => new DateTime(Year, Month, 1).AddMonths(1).AddDays(-1);
        
        private ExpiryDate(int month, int year)
        {
            Month = month;
            Year = year;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Month;
            yield return Year;
        }

        /// <summary>
        /// Returns a <see cref="ExpiryDate"/>.
        /// </summary>
        /// <param name="month">The expiry month.</param>
        /// <param name="year">The expiry year.</param>
        /// <returns>A <see cref="Result{ExpiryDate}"/> containing either an expiry date or an error.</returns>
        public static Result<ExpiryDate> Create(int month, int year)
        {
            if (month < 1 || month > 12) 
                return Result.Fail<ExpiryDate>(Errors.InvalidExpiryDate);

            var date = new ExpiryDate(month, year);
            return date.Date < DateTime.Today
                ? Result.Fail<ExpiryDate>(Errors.ExpiredCard)
                : Result.Ok(date);
        }
    }
}
