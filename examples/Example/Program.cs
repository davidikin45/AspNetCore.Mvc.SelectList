using Example.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Example
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var hostingEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var appLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();

                if (hostingEnvironment.IsDevelopment())
                {
                    var ctx = serviceProvider.GetRequiredService<AppDbContext>();
                    await ctx.Database.EnsureCreatedAsync(appLifetime.ApplicationStopping);
                }
            }

            webHost.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            });
     
    }
}
