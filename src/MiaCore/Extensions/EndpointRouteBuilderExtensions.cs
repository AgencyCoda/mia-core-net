using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using MiaCore.Features.GeGenerictList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MiaCore.Extensions
{
    internal static class EndpointRouteBuilderExtensions
    {
        internal static void MapPostRequest<T>(this IEndpointRouteBuilder endpoint, string pattern, bool allowAnonymous = false) where T : IBaseRequest
        {
            var action = generateAction<T>(parsePostRequest<T>);
            var e = endpoint.MapPost(pattern, action);
            if (allowAnonymous)
                e.AllowAnonymous();
            else
                e.RequireAuthorization();
        }

        internal static void MapGetRequest<T>(this IEndpointRouteBuilder endpoint, string pattern, bool allowAnonymous = false) where T : IBaseRequest
        {
            var action = generateAction<T>(parseGetRequest<T>);
            var e = endpoint.MapGet(pattern, action);
            if (allowAnonymous)
                e.AllowAnonymous();
            else
                e.RequireAuthorization();
        }

        private static RequestDelegate generateAction<T>(Func<HttpContext, Task<T>> parseFunction) where T : IBaseRequest
        {
            RequestDelegate action = async (HttpContext context) =>
            {
                using var scope = context.RequestServices.CreateScope();
                var mediator = scope.ServiceProvider.GetService<IMediator>();

                var request = await parseFunction(context);

                if (context.Request.RouteValues.Any())
                {
                    string routeJson = JsonConvert.SerializeObject(context.Request.RouteValues);
                    JsonConvert.PopulateObject(routeJson, request);
                }

                var response = await mediator.Send(request);

                await context.Response.WriteAsJsonAsync(response);
            };
            return action;
        }

        private static async Task<T> parsePostRequest<T>(HttpContext context) where T : IBaseRequest
        {
            var request = await context.Request.ReadFromJsonAsync<T>();
            return request;
        }


        private static Task<T> parseGetRequest<T>(HttpContext context) where T : IBaseRequest
        {
            string responseString = context.Request.QueryString.Value;
            var dict = HttpUtility.ParseQueryString(responseString);
            string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
            T request = JsonConvert.DeserializeObject<T>(json);

            return Task.FromResult(request);
        }
    }
}