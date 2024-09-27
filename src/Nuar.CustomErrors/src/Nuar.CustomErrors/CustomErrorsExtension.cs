using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nuar.Extensions.CustomErrors
{
    public class CustomErrorsExtension : IExtension
    {
        public string Name => "customErrors";
        public string Description => "Custom errors handler";
        public string Version => "1.0.0";
        
        public void Add(IServiceCollection services, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.GetForExtension<CustomErrorsOptions>(Name);

            services.AddSingleton(options);

            services.AddScoped<ErrorHandlerMiddleware>();
        }

        public void Use(IApplicationBuilder app, IOptionsProvider optionsProvider)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
