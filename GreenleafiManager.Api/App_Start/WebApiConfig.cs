using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Web.Http;
using Autofac;
using AutoMapper;
using GreenleafiManager.Api.Authorization;
using GreenleafiManager.Api.Migrations;
using GreenleafiManager.Api.Models;
using Newtonsoft.Json;

namespace GreenleafiManager.Api {
    public static class WebApiConfig {
        public static void Register () {
            //// Use this class to set configuration options for your mobile service
            //var options = new ConfigOptions();

            //options.LoginProviders.Add( typeof (CustomLoginProvider) );

            //// Use this class to set WebAPI configuration options
            //var config = ServiceConfig.Initialize( new ConfigBuilder( options, ( configuration, builder ) => {
            //    builder.Register( ctx => MappingConfig.RegisterMappings() );
            //    builder.Register( ctx => ctx.Resolve<MapperConfiguration>().CreateMapper() ).As<IMapper>();
            //} ) );

            ////Set IsHosted = true to enable authorization
            ////For every authorized request you need to pass header X-ZUMO-AUTH with token generated after login
            //config.SetIsHosted( false );

            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            Database.SetInitializer(new MobileServiceInitializer());
            var migrator = new DbMigrator(new Configuration());
            migrator.Update();
        }
    }

    public class MobileServiceInitializer : DropCreateDatabaseIfModelChanges<MobileServiceContext> {
    }
}