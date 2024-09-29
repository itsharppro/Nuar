using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nuar.Host
{
    public static class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, builder) =>
                        {
                            var configPath = args?.FirstOrDefault() ?? "nuar.yml";
                            builder.AddYamlFile(configPath, optional: false);

                            var servicesPath = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "services");
                            if (Directory.Exists(servicesPath))
                            {
                                var ymlFiles = Directory.GetFiles(servicesPath, "*.yml");
                                foreach (var ymlFile in ymlFiles)
                                {
                                    builder.AddYamlFile(ymlFile, optional: true);
                                }
                            }
                        })
                        .ConfigureServices(services => services.AddNuar())  
                        .Configure(app => app.UseNuar());
                });
    }
}
