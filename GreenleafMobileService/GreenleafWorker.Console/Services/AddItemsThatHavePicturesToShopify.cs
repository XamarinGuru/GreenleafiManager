using GreenLeafMobileService.Controllers;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Helper;
using GreenLeafMobileService.Models;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using ShopifyClient.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GreenleafWorker.Console.Services
{
    public class AddItemsThatHavePicturesToShopify
    {
        private readonly ShopifyAdapter _shopifyAdapter;
        private readonly GreenLeafMobileContext _db;

        public AddItemsThatHavePicturesToShopify(ShopifyAdapter shopifyAdapter, GreenLeafMobileContext db)
        {
            _shopifyAdapter = shopifyAdapter;
            _db = db;
        }

        public async Task Execute()
        {
            try
            {
                //get all items from our db that have pictures in blob.
                var localDbItems = await _db.Items.ToListAsync();
               // var localDbItems = await _db.Items.Where(x => x.Sku == "114395").ToListAsync();
                var shopifyItems = await _shopifyAdapter.GetInventories();
                var itemPic = await _db.ItemPictures.ToListAsync();
                for (int i = 0; i < localDbItems.Count; i++)
                {
                    if (!shopifyItems.Any(x => x.OriginalId.ToString() == localDbItems[i].OriginalId))
                    {
                        if (itemPic.Any(x => x.OriginalId == localDbItems[i].OriginalId))
                        {
                            //send this items to shopify
                            var newOriginalId = await SaveItemToShopify(localDbItems[i]);
                            var itemPics = itemPic
                                .Where(x => x.OriginalId == localDbItems[i].OriginalId)
                                .ToList();
                            for (int y = 0; y < itemPics.Count; y++)
                            {
                                //send the products pictures to shopify
                                var newPicId =
                                    await SaveItemPictureToShopify(localDbItems[i], newOriginalId, itemPics[y]);
                                //update the picture file on azure blob
                                await SaveBlobToStorage(itemPics[y], newOriginalId, newPicId);
                                await DeleteOldBlobPicture(itemPics[y].PictureName);
                                //update the item picture table with the new data
                                itemPics[y].OriginalId = newOriginalId;
                                itemPics[y].ShopifyImageId = newPicId;
                                _db.ItemPictures.Attach(itemPics[y]);
                                _db.Entry(itemPics[y]).State = EntityState.Modified;
                            }
                            //update the item table with the new originalId from Shopify
                            localDbItems[i].OriginalId = newOriginalId;
                            _db.Items.Attach(localDbItems[i]);
                            _db.Entry(localDbItems[i]).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<string> SaveItemToShopify(Item item)
        {
            try
            {
                var _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
                // update the item show in storage
                var shopifyInventory = GreenLeafMobileService.Helper.Converter.Convert(item);
                shopifyInventory.Tags = GetShopifyTags(item);
                shopifyInventory.Variants = new List<Variant> { Converter.ConvertToVariant(item) };
                var createdInventory = await _shopifyAdapter.CreateInventory(shopifyInventory);
               // createdInventory.ParseInventoryTags();
                var itemToInsert = Converter.Convert(createdInventory);
                return itemToInsert.OriginalId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetShopifyTags(Item item)
        {
            try
            {
                using (var context = new GreenLeafMobileContext())
                {

                    string sTags = context.ItemCodes.Where(x => x.Code == item.GlItemCode).Select(y => y.ShopifyTags).FirstOrDefault();
                    return sTags;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private async Task<string> SaveItemPictureToShopify(Item item, string newOriginalId, ItemsPictures itemPicture)
        {
            try
            {
                var webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData($"{itemPicture.PictureURL}");
                string base64 = System.Convert.ToBase64String(imageBytes);
                var image = await _shopifyAdapter.CreateInventorieImage(newOriginalId, base64, $"{newOriginalId}.png");
                return image.id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SaveBlobToStorage(ItemsPictures pic, string newOriginalId, string newPicId)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData($"{pic.PictureURL}");
            await AzureFilesService.SaveBlobToStorage($"{newPicId}_{newOriginalId}.png", imageBytes);
        }
        private async Task DeleteOldBlobPicture(string filename)
        {
            await AzureFilesService.DeleteBlobToStorage(filename);
        }
        //private async Task CreateImageOnShopify(string blobFileName, string productId, byte[] file, string base64)
        //{
        //    var image = await _shopifyAdapter.CreateInventorieImage(productId, base64, blobFileName);

        //await AzureFilesService.SaveBlobToStorage(image.id + "_" + blobFileName, file);
        //var imageOnOurdb =
        //new GreenLeafMobileService.DataObjects.ItemsPictures()
        //{
        //    OriginalId = image.product_id,
        //    ShopifyImageId = image.id,
        //    position = image.position
        //};
        //_db.ItemPictures.Add(imageOnOurdb);

        //await _db.SaveChangesAsync();
        //return imageOnOurdb.PictureName;
        //}
    }
}
