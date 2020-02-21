using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alba;
using Baseline;
using Microsoft.Net.Http.Headers;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Core
{
    public static class SystemUnderTestsExtensions
    {
        public static Task<Response<TResult>> Request<TRequest, TResult>(
            this SystemUnderTest sut,
            HttpMethod method,
            string endpoint,
            TRequest request,
            string apiKey)
            => sut.AuthenticatedScenario(_ =>
            {
                _.Configure = ctx => ctx.HttpMethod(method.Method);

                // Replaces route data
                _.Url(endpoint, request);

                // Adds JSON body
                if (method == HttpMethod.Post) _.Body.JsonInputIs(request);

                _.IgnoreStatusCode();
            }, apiKey)
            .ContinueWith(state =>
            {
                if (state.Exception != null)
                    throw state.Exception.GetBaseException();

                return typeof(TResult) == typeof(Empty) // No Result expected ? 
                    ? new Response<TResult>(state.Result.Context, Activator.CreateInstance<TResult>()) 
                    : new Response<TResult>(state.Result.Context, state.Result.ResponseBody.ReadAsJson<TResult>());
            });

        private static Task<IScenarioResult> AuthenticatedScenario(
                this SystemUnderTest system,
                Action<Scenario> configure,
                string apiKey)
                => system
                    .BeforeEach(ctx => ctx.Request.Headers.Add(HeaderNames.Authorization, $"Bearer {apiKey}"))
                    .Scenario(configure);

        private static SendExpression Url<TRequest>(this Scenario scenario, string url, TRequest routeData)
        {
            var properties = ToDictionary(routeData);
            var newUrl = new string(url);
            var results = Regex
                .Matches(url, @"\{(\w+)\}", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase)
                .Where(x => x.Success && properties.ContainsKey(x.Groups[1].Value))
                .ToDictionary(k => k.Groups[1].Value, v => properties[v.Groups[1].Value]);

            for (var i = results.Count - 1; i >= 0; i--)
            {
                var (tag, value) = results.ElementAt(i);
                newUrl = newUrl.Replace($"{{{tag}}}", value, StringComparison.OrdinalIgnoreCase);
            }
            return scenario.As<IUrlExpression>().Url(newUrl);
        }

        private static IDictionary<string, string?> ToDictionary<T>(T obj)
            => typeof(T)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(
                    k => k.Name,
                    v =>
                    {
                        var val = v.GetValue(obj);
                        return val switch
                        {
                            null => null,
                            DateTime d => d.ToString("yyyy-MM-ddTHH:mm:ss",
                                System.Globalization.CultureInfo.CurrentCulture),
                            _ => val.ToString()
                        };
                    },
                    StringComparer.InvariantCultureIgnoreCase);
    }
}