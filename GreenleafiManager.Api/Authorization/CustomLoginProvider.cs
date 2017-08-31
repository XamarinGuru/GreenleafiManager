using System;
using System.Security.Claims;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Newtonsoft.Json.Linq;
using Owin;

namespace GreenleafiManager.Api.Authorization {
    //public class CustomLoginProvider : LoginProvider {
    //    public const string ProviderName = "custom";

    //    public CustomLoginProvider ( IServiceTokenHandler tokenHandler )
    //        : base( tokenHandler ) {
    //        TokenLifetime = new TimeSpan( 0, 1, 0, 0 );
    //    }

    //    public override string Name => ProviderName;

    //    public override void ConfigureMiddleware ( IAppBuilder appBuilder, ServiceSettingsDictionary settings ) {
    //        // Not Applicable - used for federated identity flows
    //    }

    //    public override ProviderCredentials CreateCredentials ( ClaimsIdentity claimsIdentity ) {
    //        if ( claimsIdentity == null ) {
    //            throw new ArgumentNullException( nameof( claimsIdentity ) );
    //        }

    //        var username = claimsIdentity.FindFirst( ClaimTypes.NameIdentifier ).Value;
    //        var credentials = new CustomLoginProviderCredentials {
    //            UserId = TokenHandler.CreateUserId( Name, username )
    //        };

    //        return credentials;
    //    }

    //    public override ProviderCredentials ParseCredentials ( JObject serialized ) {
    //        if ( serialized == null ) {
    //            throw new ArgumentNullException( nameof( serialized ) );
    //        }

    //        return serialized.ToObject<CustomLoginProviderCredentials>();
    //    }
    //}

    //public class CustomLoginProviderCredentials : ProviderCredentials {
    //    public CustomLoginProviderCredentials ()
    //        : base( CustomLoginProvider.ProviderName ) {
    //    }
    //}
}