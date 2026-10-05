[assembly: HostingStartup(typeof(Docker_AspNetCore_MVC_Condominio.Web.Areas.Identity.IdentityHostingStartup))]
namespace Docker_AspNetCore_MVC_Condominio.Web.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => {
        });
    }
}