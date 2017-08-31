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
    public class StoneCodeController : TableController<StoneCode>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GreenLeafMobileContext context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<StoneCode>(context, Request);
        }

        // GET tables/StoneCode
        public IQueryable<StoneCode> GetAllStoneCodes()
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

        // GET tables/StoneCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<StoneCode> GetStoneCode(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/StoneCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<StoneCode> PatchStoneCode(string id, Delta<StoneCode> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/StoneCode
        public async Task<IHttpActionResult> PostStoneCode(StoneCode item)
        {
            try
            {
                item.Deleted = false;
                item.CreatedAt = DateTime.Now;
                StoneCode current = await InsertAsync(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE tables/StoneCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteStoneCode(string id)
        {
            return DeleteAsync(id);
        }
    }
}