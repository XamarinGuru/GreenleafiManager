using System.Web;

namespace GreenleafiManager.Api {
    public class WebApiApplication : HttpApplication {
        protected void Application_Start () {
            WebApiConfig.Register();
        }
    }
}