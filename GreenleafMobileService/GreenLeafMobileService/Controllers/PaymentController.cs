using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;

namespace GreenLeafMobileService.Controllers {
    //[AuthorizeLevel ( AuthorizationLevel.User )]
    public class PaymentController : TableController<Payment> {
        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<Payment>(context, Request);//, Services );
        }

        // GET tables/Payment
        public IQueryable<Payment> GetAllPayment () {
            return Query();
        }

        // GET tables/Payment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Payment> GetPayment ( string id ) {
            return Lookup( id );
        }

        // PATCH tables/Payment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Payment> PatchPayment ( string id, Delta<Payment> patch ) {
            return UpdateAsync( id, patch );
        }

        // POST tables/Payment
        public async Task<IHttpActionResult> PostPayment ( Payment item ) {
            var current = await InsertAsync( item );
            return CreatedAtRoute( "Tables", new {id = current.Id}, current );
        }

        // DELETE tables/Payment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePayment ( string id ) {
            return DeleteAsync( id );
        }
    }
}