using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(APS.WebApp.Startup))]
namespace APS.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
