using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using System;

namespace GreenLeafMobileService.Controllers
{
    public class VendorIdController : TableController<VendorId>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GreenLeafMobileContext context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<VendorId>(context, Request);
        }

        // GET tables/VendorId
        public IQueryable<VendorId> GetAllVendorIds()
        {
            try
            {
                return Query();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET tables/VendorId/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<VendorId> GetVendorId(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/VendorId/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<VendorId> PatchVendorId(string id, Delta<VendorId> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/VendorId
        public async Task<IHttpActionResult> PostVendorId(VendorId item)
        {
            try
            {
                item.Deleted = false;
                item.CreatedAt = DateTime.Now;
                VendorId current = await InsertAsync(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE tables/VendorId/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteVendorId(string id)
        {
            return DeleteAsync(id);
        }
    }
}