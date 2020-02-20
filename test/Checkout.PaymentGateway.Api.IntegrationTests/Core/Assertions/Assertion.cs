using System;
using Microsoft.AspNetCore.Http;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Core.Assertions
{

    public class Assertion
    {
        private readonly HttpResponse _response;

        public Assertion(HttpResponse response)
        {
            _response = response;
        }

        public Assertion HttpResponse(Action<HttpResponseAssertion> assertions)
        {
            assertions(new HttpResponseAssertion(_response));
            return this;
        }
    }

    public class Assertion<T>
    {
        private readonly T _object;
        private readonly HttpResponse _response;

        public Assertion(T current, HttpResponse response)
        {
            _response = response;
            _object = current;
        }

        public Assertion<T> Result(Action<ResultAssertion<T>> assertions)
        {
            assertions(new ResultAssertion<T>(_object));
            return this;
        }

        public Assertion<T> HttpResponse(Action<HttpResponseAssertion> assertions)
        {
            assertions(new HttpResponseAssertion(_response));
            return this;
        }
    }
}