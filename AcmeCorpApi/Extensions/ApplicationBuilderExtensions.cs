using AcmeCorpApi.Middlewares;

namespace AcmeCorpApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder) =>
            builder.UseMiddleware<AuthenticationMiddleware>();
    }
}
