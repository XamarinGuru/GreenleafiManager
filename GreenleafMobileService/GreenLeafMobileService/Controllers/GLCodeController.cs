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
    public class GLCodeController : TableController<GLCode>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            GreenLeafMobileContext context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<GLCode>(context, Request);
        }

        // GET tables/Customer
        public IQueryable<GLCode> GetAllCustomers()
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

        // GET tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<GLCode> GetCustomer(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<GLCode> PatchCustomer(string id, Delta<GLCode> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Customer
        public async Task<IHttpActionResult> PostCustomer(GLCode item)
        {
            try
            {
                item.Deleted = false;
                item.CreatedAt = DateTime.Now;
                GLCode current = await InsertAsync(item);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCustomer(string id)
        {
            return DeleteAsync(id);
        }
    }
}