using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Nuar.Swagger;

namespace Nuar.Swagger
{
    public class SwaggerExtension : IExtension
    {
        public string Name => "swagger";
        public string Description => "Swagger OpenAPI documentation";
        public string Version => "1.0.0";

        public void Add(IServiceCollection services, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.GetForExtension<SwaggerOptions>(Name);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(options.Version, new OpenApiInfo
                {
                    Title = options.Title,
                    Version = options.Version
                });

                if (options.IncludeSecurity)
                {
                    var securitySchema = new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer {token}')",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    };
                    c.AddSecurityDefinition("Bearer", securitySchema);
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        { securitySchema, new[] { "Bearer" } }
                    });
                }
            });
        }

        public void Use(IApplicationBuilder app, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.GetForExtension<SwaggerOptions>(Name);

            app.UseSwagger();

            if (options.ReDocEnabled)
            {
                app.UseReDoc(c =>
                {
                    c.DocumentTitle = options.Title;
                    c.SpecUrl($"/{options.RoutePrefix}/swagger.json");
                });
            }
            else
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/{options.RoutePrefix}/swagger.json", $"{options.Title} {options.Version}");
                    c.RoutePrefix = options.RoutePrefix;
                    c.DocExpansion(DocExpansion.None);
                });
            }
        }
    }
}
