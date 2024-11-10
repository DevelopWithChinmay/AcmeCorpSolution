namespace AcmeCorpApi.Middlewares
{
    public class AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow Swagger UI and Swagger JSON endpoints without API key
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/api-docs") ||
                context.Request.Path.Equals("/"))
            {
                await next(context); // Skip API key validation for Swagger endpoints
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            if (providedApiKey != configuration["ApiKey"])
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access Denied");
                return;
            }

            await next(context);
        }

    }
}
