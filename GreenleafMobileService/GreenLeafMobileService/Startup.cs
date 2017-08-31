using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GreenLeafMobileService.Startup))]

namespace GreenLeafMobileService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}