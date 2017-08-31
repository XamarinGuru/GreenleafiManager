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
    public class ItemCodeController : TableController<ItemCode>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GreenLeafMobileContext context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<ItemCode>(context, Request);
        }

        // GET tables/ItemCode
        public IQueryable<ItemCode> GetAllItemCodes()
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

        // GET tables/ItemCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ItemCode> GetItemCode(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ItemCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<ItemCode> PatchItemCode(string id, Delta<ItemCode> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/ItemCode
        public async Task<IHttpActionResult> PostItemCode(ItemCode item)
        {
            try
            {
                item.Deleted = false;
                item.CreatedAt = DateTime.Now;
                ItemCode current = await InsertAsync(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE tables/ItemCode/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteItemCode(string id)
        {
            return DeleteAsync(id);
        }
    }
}