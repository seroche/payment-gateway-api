using Checkout.PaymentGateway.Domain.Payments;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Payments
{
    public class ExpiryDateTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        public void Create_ShouldReturnError_WhenMonthIsNotBetween1And12(int month)
        {
            var expiry = ExpiryDate.Create(month, DateTime.UtcNow.Year + 1);

            expiry.IsSuccess.Should().BeFalse();
            expiry.Error.Should().Be(Errors.InvalidExpiryDate);
        }

        public static IEnumerable<object[]> Create_ShouldReturnError_WhenExpiryDateIsInThePast_Data()
        {
            var today = DateTime.Today;
            yield return new object[] { 1, 1787 };
            yield return new object[] { today.Month - 1, today.Year };
        }

        [Theory]
        [MemberData(nameof(Create_ShouldReturnError_WhenExpiryDateIsInThePast_Data))]
        public void Create_ShouldReturnError_WhenExpiryDateIsInThePast(int month, int year)
        {
            var expiry = ExpiryDate.Create(month, year);

            expiry.IsSuccess.Should().Be(false);
            expiry.Error.Should().Be(Errors.ExpiredCard);
        }

        public static IEnumerable<object[]> Create_ShouldReturnExpiryDate_Data()
        {
            var today = DateTime.Today;
            yield return new object[] { today.Month, today.Year };
            yield return new object[] { today.Month, 2099 };
        }

        [Theory]
        [MemberData(nameof(Create_ShouldReturnExpiryDate_Data))]
        public void Create_ShouldReturnExpiryDate(int month, int year)
        {
            var expiry = ExpiryDate.Create(month, year);

            expiry.IsSuccess.Should().BeTrue();

            var value = expiry.GetValue();
            value.Month.Should().Be(month);
            value.Year.Should().Be(year);
        }

        [Fact]
        public void Date_ShouldReturnLastDayOfExpiryMonth()
        {
            var expiry = ExpiryDate.Create(1, 2099).GetValue();

            expiry.Date.Should().Be(new DateTime(2099, 1, 31));
        }
    }
}