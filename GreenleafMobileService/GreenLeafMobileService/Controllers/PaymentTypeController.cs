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
    public class PaymentTypeController : TableController<PaymentType> {
        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<PaymentType>(context, Request);//, Services );
        }

        // GET tables/PaymentType
        public IQueryable<PaymentType> GetAllPaymentType () {
            return Query();
        }

        // GET tables/PaymentType/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<PaymentType> GetPaymentType ( string id ) {
            return Lookup( id );
        }

        // PATCH tables/PaymentType/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<PaymentType> PatchPaymentType ( string id, Delta<PaymentType> patch ) {
            return UpdateAsync( id, patch );
        }

        // POST tables/PaymentType
        public async Task<IHttpActionResult> PostPaymentType ( PaymentType item ) {
            var current = await InsertAsync( item );
            return CreatedAtRoute( "Tables", new {id = current.Id}, current );
        }

        // DELETE tables/PaymentType/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePaymentType ( string id ) {
            return DeleteAsync( id );
        }
    }
}