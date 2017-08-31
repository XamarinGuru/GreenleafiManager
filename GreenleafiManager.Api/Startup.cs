using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GreenleafiManager.Api.Startup))]

namespace GreenleafiManager.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}