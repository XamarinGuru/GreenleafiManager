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
    public class MetalCodeController : TableController<MetalCode>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GreenLeafMobileContext context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<MetalCode>(context, Request);
        }

        // GET tables/MetalCode
        public IQueryable<MetalCode> GetAllMetalCodes()
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

        // GET tables/MetalCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<MetalCode> GetMetalCode(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/MetalCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<MetalCode> PatchMetalCode(string id, Delta<MetalCode> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/MetalCode
        public async Task<IHttpActionResult> PostMetalCode(MetalCode item)
        {
            try
            {
                item.Deleted = false;
                item.CreatedAt = DateTime.Now;
                MetalCode current = await InsertAsync(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE tables/MetalCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMetalCode(string id)
        {
            return DeleteAsync(id);
        }
    }
}