using GreenLeafMobileService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GreenLeafMobileService.Controllers
{
    public class ItemsCheckController : ApiController
    {
        private readonly GreenLeafMobileContext _db;
        public ItemsCheckController()
        {
            _db = new GreenLeafMobileContext();
        }

        [HttpGet]
        [Route("api/itemcheck/{sku}")]
        public async Task<HttpResponseMessage> Put(string sku)
        {
            if(string.IsNullOrEmpty(sku))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Sku required");

            var update = _db.Items.FirstOrDefault(x => x.Sku == sku);
            if(update == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Item does not exist");

            update.LastCheckedDate = DateTime.UtcNow;
            _db.Items.Attach(update);
            _db.Entry(update).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}