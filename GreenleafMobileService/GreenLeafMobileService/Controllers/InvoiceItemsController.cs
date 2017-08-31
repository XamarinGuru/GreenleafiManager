using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;

namespace GreenLeafMobileService.Controllers
{
    public class InvoiceItemsController : TableController<InvoiceItems>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GreenLeafMobileContext context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<InvoiceItems>(context, Request);
        }

        // GET tables/InvoiceItems
        public IQueryable<InvoiceItems> GetAllInvoiceItems()
        {
            return Query(); 
        }

        // GET tables/InvoiceItems/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<InvoiceItems> GetInvoiceItems(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/InvoiceItems/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<InvoiceItems> PatchInvoiceItems(string id, Delta<InvoiceItems> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/InvoiceItems
        public async Task<IHttpActionResult> PostInvoiceItems(InvoiceItems item)
        {
            InvoiceItems current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/InvoiceItems/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteInvoiceItems(string id)
        {
             return DeleteAsync(id);
        }
    }
}
