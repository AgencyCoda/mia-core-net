using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using FluentValidation;
using MediatR;
using MiaCore.Exceptions;
using MiaCore.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MiaCore.Extensions
{
    internal static class EndpointRouteBuilderExtensions
    {
        internal static void MapPostRequest<T>(this IEndpointRouteBuilder endpoint, string pattern, bool allowAnonymous = false, List<int> roles = null) where T : IBaseRequest, new()
        {
            var action = generateAction<T>(parsePostRequest<T>, roles);
            var e = endpoint.MapPost(pattern, action);
            if (allowAnonymous)
                e.AllowAnonymous();
            else
            {
                e.RequireAuthorization();
            }
        }

        internal static void MapGetRequest<T>(this IEndpointRouteBuilder endpoint, string pattern, bool allowAnonymous = false, List<int> roles = null) where T : IBaseRequest, new()
        {
            var action = generateAction<T>(parseGetRequest<T>, roles);
            var e = endpoint.MapGet(pattern, action);
            if (allowAnonymous)
                e.AllowAnonymous();
            else
                e.RequireAuthorization();
        }

        private static RequestDelegate generateAction<T>(Func<HttpContext, JsonSerializerOptions, Task<T>> parseFunction, List<int> roles) where T : IBaseRequest, new()
        {
            RequestDelegate action = async (HttpContext context) =>
            {
                using var scope = context.RequestServices.CreateScope();

                var userHelper = scope.ServiceProvider.GetService<UserHelper>();
                await checkRolesAsync(roles, userHelper);

                var options = new JsonSerializerOptions();
                var snakeCasePolicy = new SnakeCaseNamingPolicy();
                options.PropertyNamingPolicy = snakeCasePolicy;//JsonNamingPolicy.CamelCase;
                options.PropertyNameCaseInsensitive = true;
                options.Converters.Add(new JsonStringEnumConverter(snakeCasePolicy));

                var mediator = scope.ServiceProvider.GetService<IMediator>();

                var request = await parseFunction(context, options);

                if (context.Request.RouteValues.Any())
                {
                    string routeJson = JsonConvert.SerializeObject(context.Request.RouteValues);
                    JsonConvert.PopulateObject(routeJson, request);
                }

                validateRequest(scope, request);

                var response = await mediator.Send(request);

                await context.Response.WriteAsJsonAsync(response, options);
            };
            return action;
        }

        private static async Task<T> parsePostRequest<T>(HttpContext context, JsonSerializerOptions options) where T : IBaseRequest, new()
        {
            var request = context.Request.HasJsonContentType() ? await context.Request.ReadFromJsonAsync<T>(options) : new T();
            return request;
        }


        private static Task<T> parseGetRequest<T>(HttpContext context, JsonSerializerOptions options) where T : IBaseRequest, new()
        {
            var enumConverter = new Newtonsoft.Json.Converters.StringEnumConverter();

            string responseString = context.Request.QueryString.Value;
            var dict = HttpUtility.ParseQueryString(responseString);
            string json = System.Text.Json.JsonSerializer.Serialize(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
            T request = System.Text.Json.JsonSerializer.Deserialize<T>(json, options);

            return Task.FromResult(request);
        }

        private static void validateRequest<T>(IServiceScope scope, T request)
        {
            var validators = scope.ServiceProvider.GetServices<IValidator<T>>();
            if (validators.Any())
            {
                var context = new ValidationContext<T>(request);
                var failures = validators
                    .Select(v => v.Validate(context))
                    .SelectMany(result => result.Errors)
                    .Where(f => f != null)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                if (failures.Count != 0)
                {
                    throw new Exceptions.ValidationException(string.Join(';', failures));
                }
            }
        }

        private static async Task checkRolesAsync(List<int> roles, UserHelper userHelper)
        {
            if (roles is null || !roles.Any())
                return;

            var user = await userHelper.GetUserAsync();

            if (!roles.Contains(user.Role))
                throw new UnauthorizedException(ErrorMessages.NoAccessToResource);
        }
    }
}