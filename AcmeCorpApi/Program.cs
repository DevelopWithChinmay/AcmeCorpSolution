
using AcmeCorpApi.Middlewares;
using AcmeCorpApi.Extensions;
using AcmeCorpBusiness.Entities;
using Microsoft.EntityFrameworkCore;
using AcmeCorpBusiness.Helpers;

namespace AcmeCorpApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MappingHelper.Configure();
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                // Ensure Kestrel binds to all IPs inside Docker container
                options.ListenAnyIP(80);
            });

            var services = builder.Services;
            services.AddAcmeCorpServices(builder.Configuration);

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddApiKeySettingForSwagger();
            services.AddEndpointsApiExplorer();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin()  // Allow all origins (or restrict based on your use case)
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AcmeCorpDbContext>();
                dbContext.Database.Migrate();
            }
            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}