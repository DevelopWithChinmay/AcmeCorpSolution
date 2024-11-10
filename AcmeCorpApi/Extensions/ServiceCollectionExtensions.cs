using AcmeCorpBusiness.Domain.Customers;
using AcmeCorpBusiness.Domain.Orders;
using AcmeCorpBusiness.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace AcmeCorpApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string DefaultConnection = nameof(DefaultConnection);
        public static void AddAcmeCorpServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AcmeCorpDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString(DefaultConnection)));

            services.AddScoped<CustomerReadHandler>();
            services.AddScoped<CustomerWriteHandler>();
            services.AddScoped<OrderReadHandler>();
            services.AddScoped<OrderWriteHandler>();
        }

        public static void AddApiKeySettingForSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("X-API-KEY", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "API key needed to access the endpoints",
                    Scheme = "ApiKey"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "X-API-KEY"  // This should match the security definition key
                            }
                        },
                        Array.Empty<string>()  // Empty array means no specific scope is required
                    }
                });

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Acme API",
                    Version = "v1",
                    Description = "API for managing customers and orders for Acme Corp"
                });
            });
        }
        
    }
}
