using System.Web.Http;

namespace GreenLeafMobileService.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //var migrator = new System.Data.Entity.Migrations.DbMigrator(new Migrations.Configuration());
            // migrator.Update();

        }
    }
}