using GreenleafiManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GreenleafiManager.Web.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        // GET: Item
        public async Task<ActionResult> Index(string sku)
        {
            var Items = new List<ItemModel>();
            var pictures = new List<ItemPictureModel>();
            try
            {
                if (!string.IsNullOrEmpty(sku))
                {
                    Items = await new ItemsApiService().GetItems(sku);
                    if (!string.IsNullOrEmpty(Items.First().OriginalId))
                    {

                        for (int i = 0; i < Items.Count; i++)
                            Items[i].addPictures(await new ItemImagesService().GetItemPictures(sku));

                        ViewBag.ErrorMessage = string.Empty;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "This item is not in shopify store!";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Items = new List<ItemModel>();
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(Items);
        }

        // GET: Item/Create
        public async Task<JsonResult> DeletePic()
        {
            try
            {
                JsonResult result = null;
                string PictureName = Request.Form["PictureName"].ToString();
                await new ItemImagesService().DeleteItemPictures(PictureName);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // GET: Item/Create
        public async Task<JsonResult> SavePic()
        {
            try
            {
                JsonResult result = null;
                var file = Request.Files["Picture"];
                byte[] thePictureAsBytes = new byte[file.ContentLength];
                using (BinaryReader theReader = new BinaryReader(file.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(file.ContentLength);
                }
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);

                string ItemOriginalId = Request.Form["ItemOriginalId"].ToString();
                string Sku = Request.Form["Sku"].ToString();
                string Position = Request.Form["Position"].ToString();

                var pictureName = await new ItemImagesService().CreateItemPictures(ItemOriginalId, thePictureDataAsString);
                await new ItemImagesService().UpdateItemPictures(pictureName, Position);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
