using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GreenleafiManager.Web.Startup))]
namespace GreenleafiManager.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
