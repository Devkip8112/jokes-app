using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MyAspNetApp.Areas.Identity.IdentityHostingStartup))]
namespace MyAspNetApp.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => {
        });
    }
} 