using Example.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Example
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();

            using (var scope = webHost.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var hostingEnvironment = serviceProvider.GetRequiredService<IHostingEnvironment>();
                var appLifetime = serviceProvider.GetRequiredService<IApplicationLifetime>();

                if (hostingEnvironment.IsDevelopment())
                {
                    var ctx = serviceProvider.GetRequiredService<AppDbContext>();
                    await ctx.Database.EnsureCreatedAsync(appLifetime.ApplicationStopping);
                }
            }

            webHost.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
