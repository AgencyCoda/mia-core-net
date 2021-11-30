using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MiaCore.Extensions
{
    internal static class EndpointRouteBuilderExtensions
    {
        internal static void MapPostRequest<T>(this IEndpointRouteBuilder endpoint, string pattern, bool allowAnonymous = false) where T : IBaseRequest
        {
            var action = generateAction<T>();
            var e = endpoint.MapPost(pattern, action);
            if (allowAnonymous)
                e.AllowAnonymous();
            else
                e.RequireAuthorization();
        }

        internal static void MapGetRequest<T>(this IEndpointRouteBuilder endpoint, string pattern, bool allowAnonymous = false) where T : IBaseRequest
        {
            var action = generateAction<T>();
            var e = endpoint.MapGet(pattern, action);
            if (allowAnonymous)
                e.AllowAnonymous();
            else
                e.RequireAuthorization();
        }

        private static RequestDelegate generateAction<T>() where T : IBaseRequest
        {
            RequestDelegate action = async (HttpContext context) =>
            {
                using var scope = context.RequestServices.CreateScope();
                var mediator = scope.ServiceProvider.GetService<IMediator>();

                var request = await context.Request.ReadFromJsonAsync<T>();

                var response = await mediator.Send(request);

                await context.Response.WriteAsJsonAsync(response);
            };
            return action;
        }
    }
}