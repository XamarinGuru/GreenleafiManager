using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GreenleafiManager.Api.Authorization;
using GreenleafiManager.Api.Helpers;
using GreenleafiManager.Api.Models;
using GreenleafiManager.Api.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;

namespace GreenleafiManager.Api.Controllers {
    [RoutePrefix ( "api/account" )]
    public class AccountController : ApiController {
        private readonly MobileServiceContext _context = new MobileServiceContext();
        private readonly EmailService _emailService = new EmailService();
        //private readonly IServiceTokenHandler _handler;
        //private readonly ApiServices _services;

        //public AccountController ( IServiceTokenHandler handler, ApiServices services ) {
        //    _handler = handler;
        //    _services = services;
        //}

        //[HttpPost]
        //[Route ( "login" )]
        //[ResponseType ( typeof ( string ) )]
        //[AuthorizeLevel ( AuthorizationLevel.Anonymous )]
        //public HttpResponseMessage Login ( HttpRequestMessage request, LoginViewModel model ) {
        //    var user = _context.Users.SingleOrDefault( a => a.UserName == model.UserName );

        //    if ( user != null ) {
        //        if ( user.Password == model.Password ) {
        //            var claimsIdentity = new ClaimsIdentity();

        //            claimsIdentity.AddClaim( new Claim( ClaimTypes.NameIdentifier, model.UserName ) );

        //            var loginResult = new CustomLoginProvider( _handler )
        //                .CreateLoginResult( claimsIdentity, _services.Settings.MasterKey );

        //            var customLoginResult = new CustomLoginResult {
        //                UserId = user.Id,
        //                MobileServiceAuthenticationToken = loginResult.AuthenticationToken
        //            };
        //            return Request.CreateResponse( HttpStatusCode.OK, customLoginResult );
        //        }
        //    }
        //    return Request.CreateResponse( HttpStatusCode.Unauthorized,
        //        "Invalid username or password" );
        //}

        //[HttpPost]
        //[Route ( "remindPassword" )]
        //[AuthorizeLevel ( AuthorizationLevel.Anonymous )]
        //public async Task<HttpResponseMessage> RemindPassword ( HttpRequestMessage request, string userNameOrEmail ) {
        //    var user = _context.Users.SingleOrDefault( u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail );
        //    if ( user != null ) {
        //        await _emailService.SendAsync( new IdentityMessage {
        //            Body = $"Your password is {user.Password}",
        //            Destination = user.Email,
        //            Subject = "Remind password"
        //        } );
        //    }

        //    return request.CreateResponse( HttpStatusCode.OK );
        //}

        //[HttpPost]
        //[Route ( "changePassword" )]
        //[AuthorizeLevel ( AuthorizationLevel.User )]
        //public HttpResponseMessage ChangePassword ( HttpRequestMessage request, ChangePasswordViewModel model ) {
        //    var user = _context.Users.SingleOrDefault( u => u.Id == model.UserId && u.Password == model.OldPassword );
        //    if ( user == null ) {
        //        return request.CreateResponse( HttpStatusCode.BadRequest );
        //    }

        //    user.Password = model.NewPassword;

        //    _context.Update( user );
        //    _context.SaveChanges();

        //    return request.CreateResponse( HttpStatusCode.OK );
        //}

        //[HttpPost]
        //[Route ( "changeEmail" )]
        //[AuthorizeLevel ( AuthorizationLevel.User )]
        //public HttpResponseMessage ChangeEmail ( HttpRequestMessage request, ChangeEmailViewModel model ) {
        //    var user = _context.Users.SingleOrDefault( u => u.Id == model.UserId );
        //    if ( user == null ) {
        //        return request.CreateResponse( HttpStatusCode.BadRequest );
        //    }

        //    user.Email = model.NewEmail;

        //    _context.Update( user );
        //    _context.SaveChanges();

        //    return request.CreateResponse( HttpStatusCode.OK );
        //}
    }
}