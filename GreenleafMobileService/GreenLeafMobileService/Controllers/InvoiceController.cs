using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using System.Data.Entity;
using System;

namespace GreenLeafMobileService.Controllers {
    //[AuthorizeLevel ( AuthorizationLevel.User )]
    public class InvoiceController : TableController<Invoice> {
        //private GreenLeafMobileContext context;
        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<Invoice>(context, Request);//, Services );
        }

        // GET tables/Invoice
        public IQueryable<Invoice> GetAllInvoice () {
            try
            {
                return Query();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET tables/Invoice/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Invoice> GetInvoice ( string id ) {
            try
            {
                return Lookup(id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        
        public Task<Invoice> PatchInvoice(string id, Delta<Invoice> patch)
        {
            try
            {
                return UpdateAsync(id, patch);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // POST tables/Invoice
        public async Task<IHttpActionResult> PostInvoice(Invoice item)
        {
            try
            {
                var current = await InsertAsync(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // DELETE tables/Invoice/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteInvoice ( string id ) {
            return DeleteAsync( id );
        }
    }
}