using BusifyAPI.Services.AuthServices.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace BusifyAPI.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthService _authenticationService;

        public ApiKeyMiddleware(RequestDelegate next, IAuthService authenticationService)
        {
            _next = next;
            _authenticationService = authenticationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            const string ApiKeyHeaderName = "X-ApiKey";
            const string ApiSecretHeaderName = "X-ApiSecret";

            var apiKey = context.Request.Headers[ApiKeyHeaderName];
            var apiSecret = context.Request.Headers[ApiSecretHeaderName];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret) ||
                !_authenticationService.ValidateApiKeyAndSecret(apiKey, apiSecret))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid or missing API Key/Secret.");
                return;
            }

            await _next(context);
        }
    }

    public static class ApiKeyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
