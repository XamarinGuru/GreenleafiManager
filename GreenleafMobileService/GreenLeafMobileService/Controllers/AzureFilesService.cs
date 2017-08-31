using GreenLeafMobileService.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ShopifyClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GreenLeafMobileService.Controllers
{
    public static class AzureFilesService
    {
        public static string GetFileURI(string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("greenleafitempictures");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            return blockBlob.Uri.ToString();
        }
        private static void SetupBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("greenleafitempictures");
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }
        public async static Task SaveBlobToStorage(string fileName, byte[] data)
        {
            //SetupBlobContainer();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("greenleafitempictures");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
        }
        public async static Task DeleteBlobToStorage(string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("greenleafitempictures");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.DeleteIfExistsAsync();

        }
        public static void DownloadBlobPDFToSteam(string fileName, Stream outputStream)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("greenleafitempictures");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.DownloadToStream(outputStream);
        }
    }

    public class SaveImageToShopifyService
    {
        private readonly ShopifyAdapter _shopifyAdapter;
        private readonly GreenLeafMobileContext _db;
        public SaveImageToShopifyService(ShopifyAdapter shopifyAdapter, GreenLeafMobileContext db)
        {
            this._shopifyAdapter = shopifyAdapter;
            this._db = db;
        }
        public async Task<string> CreateImageOnShopify(string blobFileName, string productId, byte[] file, string base64)
        {
            var image = await _shopifyAdapter.CreateInventorieImage(productId, base64, blobFileName);
            
            await AzureFilesService.SaveBlobToStorage(image.id + "_" + blobFileName, file);
            var imageOnOurdb = 
            new DataObjects.ItemsPictures()
            {
                OriginalId = image.product_id,
                ShopifyImageId = image.id,
                position = image.position
            };
            _db.ItemPictures.Add(imageOnOurdb);

            await _db.SaveChangesAsync();
            return imageOnOurdb.PictureName;
        }
        public async Task DeleteImageOnShopify(string blobFileName)
        {
            var pictureId = blobFileName.Split('_')[0];
            var itemid = blobFileName.Split('_')[1].Split('.')[0];
            
            var todelete = _db.ItemPictures.FirstOrDefault(x => x.OriginalId == itemid && x.ShopifyImageId == pictureId);
            if (todelete != null)
            {
                _db.ItemPictures.Remove(todelete);
                await _db.SaveChangesAsync();
            }

            await AzureFilesService.DeleteBlobToStorage(blobFileName);

            await _shopifyAdapter.DeleteInventorieImage(itemid, pictureId);
        }
        public async Task UpdateImageOnShopify(string blobFileName, string position)
        {
            var pictureId = blobFileName.Split('_')[0];
            var itemid = blobFileName.Split('_')[1].Split('.')[0];
            var result = await _shopifyAdapter.UpdateInventorieImage(itemid, pictureId, position);

            var toUpdate = _db.ItemPictures.FirstOrDefault(x => x.OriginalId == itemid && x.ShopifyImageId == pictureId);
            if (toUpdate != null)
            {
                toUpdate.position = result.position;
                _db.ItemPictures.Attach(toUpdate);
                _db.Entry(toUpdate).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
        }
    }
}