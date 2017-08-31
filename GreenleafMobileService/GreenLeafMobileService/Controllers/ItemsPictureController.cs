using AutoMapper;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using ShopifyClient.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace GreenLeafMobileService.Controllers
{
    public class ItemsPictureController : ApiController
    {

        private readonly GreenLeafMobileContext _db;
        private readonly ShopifyAdapter _shopifyAdapter;
        public ItemsPictureController()
        {
            try
            {
                _db = new GreenLeafMobileContext();
                _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ItemsPictureController(IMapper mapper)
        {
            _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
            _db = new GreenLeafMobileContext();
        }

        [HttpGet]
        [Route("api/images/")]
        public async Task<HttpResponseMessage> GetAll()
        {
            int count = 0;
            try
            {
                var list = await _db.ItemPictures.ToListAsync();
                var jObject = JsonConvert.SerializeObject(list);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + "Object number = " + count.ToString());
            }
        }
        [HttpGet]
        [Route("api/invoice/GetNextInvoiceNumber")]
        public async Task<HttpResponseMessage> GetNextInvoiceNumber()
        {
            List<Invoice> items = new List<DataObjects.Invoice>();
            int number = 0;
            if (await _db.Invoices.AnyAsync())
            {
                number = Convert.ToInt32(_db.Invoices.OrderByDescending(x => x.Number).First().Number);
            }
            else
            {
                var response1 = Request.CreateResponse(HttpStatusCode.OK);
                response1.Content = new StringContent("1001", Encoding.UTF8, "application/json");
                return response1;
            }
            var nextNumber = number + 1;
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(nextNumber.ToString(), Encoding.UTF8, "application/json");
            return response;
        }
        [HttpGet]
        [Route("api/images/{sku}")]
        public async Task<HttpResponseMessage> GetById(string sku)
        {
            int count = 0;
            try
            {
                var item = _db.Items.Where(x => x.Sku == sku).First();
                if (item == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound + "Sku: " + sku);
                }

                var list = await _db.ItemPictures.Where(x => x.OriginalId == item.OriginalId).ToListAsync();
                var jObject = JsonConvert.SerializeObject(list);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message + "Object number = " + count.ToString());
            }
        }
        [HttpPost]
        [Route("api/images/")]
        public async Task<HttpResponseMessage> Post(PicturePost model)
        {
            byte[] byteArray = System.Convert.FromBase64String(model.picturebase64);
            var service = new SaveImageToShopifyService(_shopifyAdapter, _db);
            var imageId = await service.CreateImageOnShopify(model.itemId + ".png", model.itemId, byteArray, model.picturebase64);
            if (imageId != null)
            {
                var jObject = JsonConvert.SerializeObject(imageId);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                return response;
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
        [HttpPost]
        [Route("api/images/updateposition")]
        public async Task<HttpResponseMessage> Put([FromBody]PicturePositionPut model)
        {
            //DJ - Update Image position on shopify here
            var service = new SaveImageToShopifyService(_shopifyAdapter, _db);
            await service.UpdateImageOnShopify(model.pictureName, model.position);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpGet]
        [Route("api/images/delete/{pictureBlobName}")]
        public async Task<HttpResponseMessage> Delete(string pictureBlobName)
        {
            var service = new SaveImageToShopifyService(_shopifyAdapter, _db);
            await Task.Run(async () => { await service.DeleteImageOnShopify(pictureBlobName); });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
    public class PicturePost
    {
        public string itemId { get; set; }
        public string picturebase64 { get; set; }
    }
    public class PicturePositionPut
    {
        public string pictureName { get; set; }
        public string position { get; set; }
    }
    public class PictureDelete
    {
        public string pictureBlobName { get; set; }
    }
}
