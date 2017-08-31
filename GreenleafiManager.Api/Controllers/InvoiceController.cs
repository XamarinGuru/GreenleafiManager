using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using GreenleafiManager.Api.DataObjects;
using GreenleafiManager.Api.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;

namespace GreenleafiManager.Api.Controllers {
    //[AuthorizeLevel ( AuthorizationLevel.User )]
    public class InvoiceController : TableController<Invoice> {
        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Invoice>(context, Request);//, Services );
        }

        // GET tables/Invoice
        public IQueryable<Invoice> GetAllInvoice () {
            return Query();
        }

        // GET tables/Invoice/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Invoice> GetInvoice ( string id ) {
            return Lookup( id );
        }

        // PATCH tables/Invoice/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Invoice> PatchInvoice ( string id, Delta<Invoice> patch ) {
            return UpdateAsync( id, patch );
        }

        // POST tables/Invoice
        public async Task<IHttpActionResult> PostInvoice ( Invoice item ) {
            var current = await InsertAsync( item );
            return CreatedAtRoute( "Tables", new {id = current.Id}, current );
        }

        // DELETE tables/Invoice/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteInvoice ( string id ) {
            return DeleteAsync( id );
        }
    }
}