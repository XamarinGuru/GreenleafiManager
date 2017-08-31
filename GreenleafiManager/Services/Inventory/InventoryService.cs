using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using SQLite;
using Foundation;
using UIKit;
using System.Net;
using ModernHttpClient;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace GreenleafiManager
{
	public class InventoryService
	{
		AzureService _Parent;
		//static InventoryService instance = new InventoryService();

		private IMobileServiceSyncTable<Item> ItemTable;
		//private IMobileServiceSyncTable<InventoryImage> InventoryImageTable;
		private IMobileServiceSyncTable<GLCode> GLCodeTable;
		private IMobileServiceSyncTable<StoneCode> StoneCodeTable;
		private IMobileServiceSyncTable<MetalCode> MetalCodeTable;
		private IMobileServiceSyncTable<ItemCode> ItemCodeTable;
		private IMobileServiceSyncTable<VendorId> VendorIdTable;

		//RMR Service Rework TODO do we need this?
		//public static void RefreshFromServerDB()
		//{

		//	instance = new InventoryService();

		//}
		private List<string> _ItemsThatRequireImageUpdate;
		public List<string> ItemsThatRequireImageUpdate
		{
			get
			{
				if (_ItemsThatRequireImageUpdate == null)
					_ItemsThatRequireImageUpdate = new List<string>();
				return _ItemsThatRequireImageUpdate;
			}
		}
		public void UpdateImagesFromRequiredUpdateList(int pagination = 0)
		{
			if (pagination > 0)
			{
				var skuArray = ItemsThatRequireImageUpdate.Distinct().ToArray();
				foreach (var itemSku in skuArray)
				{
					var itemNS = AzureService.InventoryService.ItemsNS.Where(x => x.Sku == itemSku).FirstOrDefault();
					if (itemNS != null)
						itemNS.LoadImagesFromDrive();
					var invItem = AzureService.InventoryService.Items.Where(x => x.Sku == itemSku).FirstOrDefault();
					if (invItem != null)
						invItem.AddImageData();
					ItemsThatRequireImageUpdate.RemoveAll(x => x == itemSku);
				}
			}
			//else
			//	itemNS.Images = new List<ProductImage>();

		}
		public InventoryService(AzureService parent)
		{
			_Parent = parent;

			// Create an MSTable instance to allow us to work with the InventoryItem table
			GLCodeTable = _Parent.Client.GetSyncTable<GLCode>();
			StoneCodeTable = _Parent.Client.GetSyncTable<StoneCode>();
			MetalCodeTable = _Parent.Client.GetSyncTable<MetalCode>();
			ItemCodeTable = _Parent.Client.GetSyncTable<ItemCode>();
			VendorIdTable = _Parent.Client.GetSyncTable<VendorId>();
			ItemTable = _Parent.Client.GetSyncTable<Item>();
			//InventoryImageTab le = Client.GetSyncTable <InventoryImage> ();
		}
		public bool NeedServerRefresh { get; set; }
		public bool NeedLocalRefresh { get; set; }
		public bool NeedImageRefresh { get; set; }

		public string GetNextSku()
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				return "";
			}
			else
			{
				using (var Client = new HttpClient())
				{
					Client.BaseAddress = new Uri(_Parent.ApplicationURL);

					Client.DefaultRequestHeaders.Accept.Clear();
					Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
					Client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
					var response = Client.GetAsync("tables/Reports/GetNextSKU").Result;

					if (!response.IsSuccessStatusCode)
						return "";
					else {
						var json = response.Content.ReadAsStringAsync().Result;
						string result = JsonConvert.DeserializeObject<string>(json);
						return result;
					}
				}
			}
		}

		public async Task UpdateItemsFromLocalDB(bool ignoreImages = false)
		{
			//RMR only do this if list is null
			if(_ItemsNS == null)
				_ItemsNS = new List<InventoryNS>();
			if (Items == null || Items.Count == 0)
			{
			//	await PullAsync();
				_Items = await ItemTable.ToListAsync();
			}
			//RMR this should be taken care of in intiail load. It isn't but we need to figuore out why
			await AllCodesAndIdsSetupConfirmationWithSync();
			foreach (var i in Items)
			{
				if(!UpdateItemNSFromLocalItems(i.Id))
					ItemsNS.Add(new InventoryNS(i));
			}
		}

		public async Task UpdateItemsFromAzure()
		{
			await PullAsync();
			//RMR only do this if list is null
			if (_ItemsNS == null)
				_ItemsNS = new List<InventoryNS>();
			
			_Items = await ItemTable.ToListAsync();

			if (Items != null)
			{
				foreach (var i in Items)
				{
					if(!UpdateItemNSFromLocalItems(i.Id))
						ItemsNS.Add(new InventoryNS(i));
				}
			}
		}

		public bool UpdateItemNSFromLocalItems(string id)
		{
			if (_Items == null || _Items.Count == 0 || _ItemsNS == null || _ItemsNS.Count == 0)
				return false;

			var inv = _Items.Where(x => x.Id == id).FirstOrDefault();
			var invNS = _ItemsNS.Where(x => x.Id == id).FirstOrDefault();
			if (inv == null || invNS == null)
				return false;

			invNS.SetDescription();
			invNS.Sold = inv.IsSold;
			//GlCode = inv.GlCode;
			invNS.GlCost = inv.GlCost.Value;
			invNS.VendorCode = inv.VendorCode;
			invNS.GlItemCode = inv.GlItemCode;
			invNS.SecretCode = inv.SecretCode;
			invNS.TagPrice = inv.TagPrice;
			invNS.MetalCode = inv.MetalCode;
			invNS.Info1 = inv.Info1;
			invNS.Info2 = inv.Info2;
			invNS.Info3 = inv.Info3;
			invNS.Sku = inv.Sku;
			invNS.InvoiceDescription = inv.InvoiceDescription;
			invNS.Location = inv.LocationName;

			invNS.ShopifyId = inv.ShopifyId;

			invNS.SetGLShortCode();
			invNS.SetSearchString();

			if (inv.Images == null)
				invNS.Images = null;
			else if (inv.Images.Count == 0)
				invNS.Images = new List<ProductImage>();
			else
			{
				for (int i = 0; i < inv.Images?.Count; i++)
				{
					if (invNS.Images?.Count < i + 1)
					{
						invNS.Images.Add(inv.Images[i]);
					}
					//else if(inv.Images[i].
				}
			}

			return true;
		}
		private List<Item> _Items;

		public List<Item> Items
		{
			get
			{
				if (_Items == null)
				{
					_Items = new List<Item>();
				}
				return _Items;
			}
		}
		private List<InventoryNS> _ItemsNS;
		public List<InventoryNS> ItemsNS 
		{
			get
			{
				if (_ItemsNS == null)
				{
					_ItemsNS = new List<InventoryNS>();
				}
				return _ItemsNS;
			}
		}


		private List<GLCode> _GLCodes;

		public List<GLCode> GLCodes
		{
			get
			{
				return _GLCodes;
			}
		}
		public async Task UpdateGLCodes()
		{
			await PullGLCodes();
			_GLCodes = await GLCodeTable.ToListAsync();
		}
		public async Task GLCodesSetupConfirmationWithSync()
		{
			if (_GLCodes == null || _GLCodes.Count == 0)
			{
				await UpdateGLCodes();
			}
		}

		private List<MetalCode> _MetalCodes;

		public List<MetalCode> MetalCodes
		{
			get
			{
				return _MetalCodes;
			}
		}
		public async Task UpdateMetalCodes()
		{
			await PullMetalCodes();
			_MetalCodes = await MetalCodeTable.ToListAsync();
		}
		public async Task MetalCodesSetupConfirmationWithSync()
		{
			if (_MetalCodes == null || _MetalCodes.Count == 0)
			{
				await UpdateMetalCodes();
			}
		}

		private List<StoneCode> _StoneCodes;

		public List<StoneCode> StoneCodes
		{
			get
			{
				return _StoneCodes;
			}
		}
		public async Task UpdateStoneCodes()
		{
			await PullStoneCodes();
			_StoneCodes = await StoneCodeTable.ToListAsync();
		}
		public async Task StoneCodesSetupConfirmationWithSync()
		{
			if (_StoneCodes == null || _StoneCodes.Count == 0)
			{
				await UpdateStoneCodes();
			}
		}

		private List<ItemCode> _ItemCodes;

		public List<ItemCode> ItemCodes
		{
			get
			{
				if (_ItemCodes != null)
					return _ItemCodes;
				else
				{
					//UpdateItemCodes();
					return _ItemCodes;
				}
			}
		}
		public async Task UpdateItemCodes()
		{
			await PullItemCodes();
			_ItemCodes = await ItemCodeTable.ToListAsync();
		}
		public async Task ItemCodesSetupConfirmationWithSync()
		{
			if (_ItemCodes == null || _ItemCodes.Count == 0)
			{
				await UpdateItemCodes();
			}
		}
		public List<string> Categories
		{
			get
			{
				return new List<String> {
					"Diamond",
					"Ruby",
					"Emerald",
					"Sapphire",
					"Tanzanite",
					"Pearl",
					"Gold",
					"Zircon",
					"Blue Zircon",
					"Alexandrite",
					"Opal",
					"Gold Nugget",
					"Quartz",
					"Amethyst",
					"Peridot",
					"Aquamarine",
					"Tourmaline",
					"Citrine",
					"Morganite",
					"Topaz",
					"Blue Topaz",
					"Watch",
					"Other"
				};
			}
		}

		private List<VendorId> _VendorIds;

		public List<VendorId> VendorIds
		{
			get
			{
				return _VendorIds;
			}
		}
		public async Task UpdateVendorIds()
		{
			await PullVendorIds();
			_VendorIds = await VendorIdTable.ToListAsync();
		}
		public async Task VendorIdsSetupConfirmationWithSync()
		{
			if (_VendorIds == null || _VendorIds.Count == 0)
			{
				await UpdateVendorIds();
			}
		}


		public async Task ClearAllData()
		{
			await ItemTable.PurgeAsync("allItems", "", true, new System.Threading.CancellationToken());
			await MetalCodeTable.PurgeAsync("allMetalCode", "", true, new System.Threading.CancellationToken());
			await ItemCodeTable.PurgeAsync("allItemCodes", "", true, new System.Threading.CancellationToken());
			await StoneCodeTable.PurgeAsync("allStoneCode", "", true, new System.Threading.CancellationToken());
			await GLCodeTable.PurgeAsync("allGLCode", "", true, new System.Threading.CancellationToken());
			await VendorIdTable.PurgeAsync("allVendorIds", "", true, new System.Threading.CancellationToken());

			_Items = new List<Item>();
			_ItemsNS = new List<InventoryNS>() ;
			_MetalCodes = new List<MetalCode>();
			_ItemCodes = new List<ItemCode>();
			_StoneCodes = new List<StoneCode>();
			_GLCodes = new List<GLCode>();
			_VendorIds = new List<VendorId>();
		}
		private async Task PullAsync()
		{
			try
			{
				await _Parent.LocationSetupConfirmationWithSync();

				//await _Parent.Client.SyncContext.PushAsync();
				await PullAllCodes();
				await UpdateVendorIds();
				await ItemTable.PullAsync("allItems", ItemTable.CreateQuery()); // query ID is used for incremental sync
				_Items = new List<Item>();
				_Items = await ItemTable.ToListAsync();
			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(@"Sync Failed: {0}", ex.Message);
			}
		}
		public async Task AllCodesAndIdsSetupConfirmationWithSync()
		{
			await VendorIdsSetupConfirmationWithSync();
			await ItemCodesSetupConfirmationWithSync();
			await StoneCodesSetupConfirmationWithSync();
			await MetalCodesSetupConfirmationWithSync();
			await GLCodesSetupConfirmationWithSync();
		}
		public async Task PullAllCodes()
		{
			try
			{
				//await _Parent.Client.SyncContext.PushAsync();
				await PullItemCodes();//await ItemCodeTable.PullAsync("allItemCode", ItemCodeTable.CreateQuery()); // query ID is used for incremental sync
				await PullStoneCodes();//await StoneCodeTable.PullAsync("allStoneCode", StoneCodeTable.CreateQuery()); // query ID is used for incremental sync
				await PullMetalCodes();//await MetalCodeTable.PullAsync("allMetalCode", MetalCodeTable.CreateQuery()); // query ID is used for incremental sync
				await PullGLCodes();//await GLCodeTable.PullAsync("allGLCode", GLCodeTable.CreateQuery()); // query ID is used for incremental sync
			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"SyncAllCodes Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task PullGLCodes()
		{
			try
			{
				//await _Parent.Client.SyncContext.PushAsync();
				await GLCodeTable.PullAsync("allGLCode", GLCodeTable.CreateQuery()); // query ID is used for incremental sync
			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync allGLCode Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(@"Sync allItemCode Failed: {0}", ex.Message);
			}
			finally
			{
				_GLCodes = await GLCodeTable.ToListAsync();
			}
		}
		public async Task PullItemCodes()
		{
			try
			{
				//await _Parent.Client.SyncContext.PushAsync();
				await ItemCodeTable.PullAsync("allItemCodes", ItemCodeTable.CreateQuery()); // query ID is used for incremental sync
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync allItemCode Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(@"Sync allItemCode Failed: {0}", ex.Message);
			}
			finally
			{
				_ItemCodes = await ItemCodeTable.ToListAsync();
			}
		}


		public async Task InsertIntoItemCodes(ItemCode item)
		{
			try
			{
				await ItemCodeTable.InsertAsync(item);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("Inser into ItemsCode exception: " + e.Message);
			}

		}


		public async Task PullStoneCodes()
		{
			try
			{
				//await _Parent.Client.SyncContext.PushAsync();
				await StoneCodeTable.PullAsync("allStoneCode", StoneCodeTable.CreateQuery()); // query ID is used for incremental sync
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync allStoneCode Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(@"Sync allItemCode Failed: {0}", ex.Message);
			}
			finally
			{
				_StoneCodes = await StoneCodeTable.ToListAsync();
			}
		}
		public async Task PullMetalCodes()
		{
			try
			{
				//await _Parent.Client.SyncContext.PushAsync();
				await MetalCodeTable.PullAsync("allMetalCode", MetalCodeTable.CreateQuery()); // query ID is used for incremental sync

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sycn allMetalCode Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(@"Sync allItemCode Failed: {0}", ex.Message);
			}
			finally
			{
				_MetalCodes = await MetalCodeTable.ToListAsync();
			}
		}
		public async Task PullVendorIds()
		{
			try
			{
				//await _Parent.Client.SyncContext.PushAsync();
				await VendorIdTable.PullAsync("allVendorIds", VendorIdTable.CreateQuery()); // query ID is used for incremental sync
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sycn allVendorId Failed: {0}", e.Message);
			}

			catch (Exception ex)
			{
				Console.Error.WriteLine(@"Sync allItemCode Failed: {0}", ex.Message);
			}
			finally
			{
				_VendorIds = await VendorIdTable.ToListAsync();
			}
		}

		public async Task InsertOrSave(Item item, List<ProductImage> productimagesList = null)
		{
			try
			{
				if (Items.Any(x => x.Id == item.Id || x.Sku == item.Sku))
				{
					item.LastCheckedDate = DateTime.UtcNow;
					var foundItem = Items.Where(x => x.Id == item.Id).First();
					if (foundItem == null)
						foundItem = Items.Where(x => x.Sku == item.Sku).First();
					foundItem.GlCost = item.GlCost;
					foundItem.GlItemCode = item.GlItemCode;
					foundItem.Info1 = item.Info1;
					foundItem.Info2 = item.Info2;
					foundItem.Info3 = item.Info3;
					foundItem.MetalCode = item.MetalCode;
					foundItem.SecretCode = item.SecretCode;
					foundItem.Sku = item.Sku;
					foundItem.IsSold = item.IsSold;
					foundItem.TagPrice = item.TagPrice;
					foundItem.VendorCode = item.VendorCode;
					foundItem.LocationId = item.LocationId;
					foundItem.InvoiceDescription = item.InvoiceDescription;
					foundItem.LastCheckedDate = item.LastCheckedDate;
					foundItem.ShowOnWebsite = item.ShowOnWebsite;
					foundItem.ShopifyTitle = item.ShopifyTitle;
					foundItem.ShopifyPrice = item.ShopifyPrice;
					foundItem.ShopifyDescription = item.ShopifyDescription;
					await ItemTable.UpdateAsync(foundItem);
					//await UpdateShopifyProduct(foundItem.GetProductDataForShopifyPost());
				}
				else
				{//new
				 //item.GlCode = "1";
					item.LastCheckedDate = DateTime.UtcNow;
					item.VendorCode = "GL";
					item.Description = "";
					//item.LocationId = null;
					item.SearchString = "";
					await ItemTable.InsertAsync(item); // Insert a new InventoryItem into the local database. 
					item.VendorCode = "GL";
					Items.Add(item);

					//RMR this is required so creating new InventoryNS doesn't fail
					await AllCodesAndIdsSetupConfirmationWithSync();
					ItemsNS.Add(new InventoryNS(item));
					//AddNewProduct(item.GetProductDataForShopifyPost());

				}
				var documentsDirectory = Environment.GetFolderPath
					(Environment.SpecialFolder.Personal);

				if (productimagesList != null)
				{
					foreach (var productImage in productimagesList)
					{
						string jpgFilename = System.IO.Path.Combine(documentsDirectory, productImage.filename);

						productImage.ImageBytes = File.ReadAllBytes(jpgFilename);
						UploadImageToShopifyAndUpdateLocalData(productImage);
					}
				}

				await _Parent.PushAsync();
				//RMR TODO refactor - might not need the pull, just the push now //await PullAsync(); // send changes to the mobile service


			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}

		public async Task MarkItemSoldAsync(Item item, bool sold = true)
		{
			try
			{
				item.IsSold = sold;
				await ItemTable.UpdateAsync(item); // update InventoryItem item in the local database
												   //await SyncAsync(); // send changes to the mobile service
				if (Items.Any(x => x.Id == item.Id))
				{
					var foundItem = Items.Where(x => x.Id == item.Id).First();
					foundItem.IsSold = sold;
				}
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		private async Task DeleteItemById(string id)
		{
			try
			{

				await DeleteLocalItemById(id);

				//RMR Deletition of DB item not setup
				//var sa = new ShopifyAdapter ();
				//sa.DeleteShopifyProduct(id);

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}

		private async Task DeleteLocalItemById(string id)
		{
			try
			{
				var foundItem = Items.Where(x => x.Id == id).First();//.IndexOf(item);

				Items.Remove(foundItem);

				await DeleteItemFromLocalDB(id);

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		public async Task DeleteItem(Item item)
		{
			await DeleteItemById(item.Id);
		}


		//SQLite Actions

		bool SQLiteDBIsSetup = false;
		public void SetupInventoryTable()
		{
			try
			{

				//SQLite
				using (var conn = new SQLite.SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _Parent.LocalDbPath)))
				{
					if (conn.GetTableInfo("Inventory") != null)
						conn.CreateTable<Item>();
					SQLiteDBIsSetup = true;
				}
			}
			catch (SQLite.SQLiteException ex)
			{
#if DEBUG
				throw ex;
#else
				SQLiteDBIsSetup = false;
				Console.WriteLine(ex.Message);
#endif

			}
		}



		public void UpdateItemInList(Item item)
		{
			var foundItem = Items.Where(x => x.Id == item.Id).FirstOrDefault();
			if (foundItem == null)
			{
				Items.Add(item);
			}
			else {
				//foundItem.CreatedAt = item.CreatedAt;
				foundItem.Description = item.Description;
				//foundItem.GlCode = item.GlCode;
				foundItem.GlCost = item.GlCost;
				foundItem.GlItemCode = item.GlItemCode;
				foundItem.Images = item.Images;
				foundItem.Info1 = item.Info1;
				foundItem.Info2 = item.Info2;
				foundItem.Info3 = item.Info3;
				//foundItem.IsNew = false;
				foundItem.MetalCode = item.MetalCode;
				foundItem.SearchString = item.SearchString;
				foundItem.SecretCode = item.SecretCode;
				foundItem.Sku = item.Sku;
				foundItem.IsSold = item.IsSold;
				foundItem.TagPrice = item.TagPrice;
				//foundItem.UpdatedAt = item.UpdatedAt;
				foundItem.VendorCode = item.VendorCode;
			}
		}

		public async Task DeleteItemFromLocalDB(string id)
		{
			if (!SQLiteDBIsSetup)
			{
				SetupInventoryTable();
				return;
			}

			var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _Parent.LocalDbPath));
			var foundItem = await db.FindAsync<Item>(x => x.Id == id);

			if (foundItem != null)
			{
				await db.DeleteAsync(foundItem);
			}
		}


		/// <summary>
		/// //Images
		/// </summary>

		public async Task SyncImages()
		{
			var sa = new ShopifyAdapter();
			await sa.CheckAndDownloadAllProductImages();
		}
		ShopifyAdapter sad = new ShopifyAdapter();
		public async Task<ProductImage> UploadImage(ProductImage image)
		{

			var uploadedImage = await sad.PostShopifyProductImage(image);

			return uploadedImage;
		}
		public ProductImage AddOrUpdateImage(UIImage image, Item inventoryItem)
		{
			using (image = Extensions.RotateImage(image))
			{

				//image = Extensions.RotateImage(image);
				using (var thumb_image = Extensions.MaxResizeImage(image, 400, 400))
				{
					var photo = image.AsJPEG(0.25f);

					var documentsDirectory = Environment.GetFolderPath
						(Environment.SpecialFolder.Personal);

					string jpgFilename = System.IO.Path.Combine(documentsDirectory, inventoryItem.Sku + ".jpg"); //Default filename for first file

					int nextImageIndex = inventoryItem.Images.Count > 1 ? inventoryItem.Images.Count :
						System.IO.File.Exists(jpgFilename) ? 1 : 0;

					jpgFilename = System.IO.Path.Combine(documentsDirectory, String.Format("{0}{1}.jpg", inventoryItem.Sku, nextImageIndex == 0 ? "" : "_" + nextImageIndex)); // update it for file number

					NSData imgData = photo;
					NSError err = null;
					if (imgData.Save(jpgFilename, false, out err))
					{
						Console.WriteLine("saved as " + jpgFilename);
					}
					else {
						Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
					}

					#region Thumbnail
					var thumb_photo = thumb_image.AsJPEG(0.25f);


					var thumbjpgFilename = System.IO.Path.Combine(documentsDirectory, String.Format("{0}{1}_thumb.jpg", inventoryItem.Sku, nextImageIndex == 0 ? "" : "_" + nextImageIndex)); // update it for file number

					NSData imgThumbData = thumb_photo;
					if (imgThumbData.Save(thumbjpgFilename, false, out err))
					{
						Console.WriteLine("saved as " + thumbjpgFilename);
					}
					else {
						Console.WriteLine("NOT saved as " + thumbjpgFilename + " because" + err.LocalizedDescription);
					}

					#endregion

					//img_UploadImage.SetImage((System.IO.File.Exists(jpgFilename) ? UIImage.FromFile (jpgFilename) : UIImage.FromFile ("Images/no-thumb.png")), UIControlState.Normal);

					var productImage = new ProductImage()
					{
						//ImageBytes = Extensions.ToNSData(image),
						filename = String.Format("{0}{1}.jpg", inventoryItem.Sku, nextImageIndex == 0 ? "" : "_" + nextImageIndex),
						product_id = inventoryItem.ShopifyId
					};
					//this.img_UploadImage.ReloadInputViews();

					//Run Async
					//UploadImageToShopifyAndUpdateLocalData(productImage);

					//					Task.Run(()=>{
					//						UploadImageToShopifyAndUpdateLocalData(productImage);
					//					});
					return productImage;

				}
				//var thumb_image = Extensions.MaxResizeImage(image, 200, 200);
			}
		}
		public bool AddOrUpdateImage(NSUrl currentLocationURL, string originalShopifyItemId)
		{
			
			var inventoryItem = Items.Where(x => x.ShopifyId == originalShopifyItemId).FirstOrDefault();
			if (inventoryItem == null)
				return false;
			
			var documentsDirectory = Environment.GetFolderPath
				(Environment.SpecialFolder.Personal);

			string jpgFilename = System.IO.Path.Combine(documentsDirectory, inventoryItem.Sku + ".jpg");

			if(inventoryItem.Images == null)
				inventoryItem.Images = new List<ProductImage>();
			int nextImageIndex = inventoryItem.Images.Count > 1 ? inventoryItem.Images.Count :
																	System.IO.File.Exists(jpgFilename) ? 1 : 0;

			jpgFilename = System.IO.Path.Combine(documentsDirectory, String.Format("{0}{1}.jpg", inventoryItem.Sku, nextImageIndex == 0 ? "" : "_" + nextImageIndex)); 

			NSFileManager fileManager = NSFileManager.DefaultManager;
			var URLs = fileManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
			NSUrl documentsDictionry = URLs[0];
			NSUrl destinationURL = documentsDictionry.Append(jpgFilename, false);
			NSError removeCopy;

			fileManager.Remove(destinationURL, out removeCopy);

			NSError errorCopy;

			bool success = fileManager.Copy(currentLocationURL, destinationURL, out errorCopy);

			if (success)
			{
				Console.WriteLine("Sku {1}:D File Saved in : {0}", destinationURL, inventoryItem.Sku);
				AzureService.InventoryService.ItemsThatRequireImageUpdate.Add(inventoryItem.Sku);
				AzureService.InventoryService.NeedLocalRefresh = true;

			}
			else
			{
				Console.WriteLine("Error during the copy: {0}", errorCopy.LocalizedDescription);

			}
			return success;
		}
		public Item GetItemByShopifyId(string originalShopifyItemId)
		{
			var inventoryItem = Items.Where(x => x.ShopifyId == originalShopifyItemId).FirstOrDefault();
			return inventoryItem; 
		}
		public NSUrl GetImageSaveLocationWithName(Item inventoryItem, string shopifyImageId)
		{
			var documentsDirectory = Environment.GetFolderPath
				(Environment.SpecialFolder.Personal);

			string jpgFilename = System.IO.Path.Combine(documentsDirectory, inventoryItem.Sku + ".jpg");

			if(inventoryItem.Images == null)
				inventoryItem.Images = new List<ProductImage>();
			int nextImageIndex = inventoryItem.Images.Count > 1 ? inventoryItem.Images.Count :
																	System.IO.File.Exists(jpgFilename) ? 1 : 0;

			jpgFilename = String.Format("{0}{1}.jpg", inventoryItem.Sku, nextImageIndex == 0 ? "" : "_" + nextImageIndex); 

			NSFileManager fileManager = NSFileManager.DefaultManager;
			var URLs = fileManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
			NSUrl documentsDictionry = URLs[0];
			NSUrl destinationURL = documentsDictionry.Append(jpgFilename, false);

			return destinationURL;
		}
		public async Task UploadImageToShopifyAndUpdateLocalData(ProductImage productImage)
		{
			var uploadedImage = await UploadImage(productImage);
			productImage.id = uploadedImage.id;
			productImage.created_at = uploadedImage.created_at;
			productImage.updated_at = uploadedImage.updated_at;
			productImage.position = uploadedImage.position;
			productImage.src = uploadedImage.src;

			//await AddImageToLocalDB(productImage);
		}

		/// <summary>
		///Global Variables 
		/// </summary>
		bool SQLiteDBGlobalsIsSetup = false;
		private async Task DropAndRecreateLocalGlobalsTable()
		{
			if (!SQLiteDBGlobalsIsSetup)
			{
				SetupGlobalsTable();
				return;
			}
			try
			{
				var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _Parent.LocalDbPath));
				await db.DropTableAsync<GlobalVariables>();
				SetupGlobalsTable();
				Console.WriteLine("Table Dropped and Recreated");
			}
			catch (SQLite.SQLiteException ex)
			{
#if DEBUG
				throw ex;
#else
				Console.WriteLine(ex.Message);
#endif
			}
		}
		public void SetupGlobalsTable()
		{
			try
			{

				//SQLite
				using (var conn = new SQLite.SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _Parent.LocalDbPath)))
				{
					if (conn.GetTableInfo("GlobalVariables") != null)
						conn.CreateTable<GlobalVariables>();
					SQLiteDBGlobalsIsSetup = true;

				}
			}
			catch (SQLite.SQLiteException ex)
			{
				SQLiteDBGlobalsIsSetup = false;
				Console.WriteLine(ex.Message);
			}
		}

		public async Task<DateTime> GetLastUpdatedDateTime()
		{
			if (!SQLiteDBGlobalsIsSetup)
				SetupGlobalsTable();

			var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _Parent.LocalDbPath));
			var foundItem = await db.FindAsync<GlobalVariables>(x => x.Id == 1);

			return foundItem == null ? DateTime.Parse("1/1/1901") : foundItem.LastUpdated;
		}
		public async Task SetLastUpdatedDateTime(DateTime updateDateTime)
		{
			if (!SQLiteDBGlobalsIsSetup)
				SetupGlobalsTable();
			try
			{
				var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _Parent.LocalDbPath));
				var data = new GlobalVariables() { Id = 1, LastUpdated = updateDateTime };
				var foundItem = await db.FindAsync<GlobalVariables>(x => x.Id == 1);

				if (foundItem != null)
				{
					await db.UpdateAsync(data);
				}
				else
				{
					await db.InsertAsync(data);
				}
				Console.WriteLine("Single lastupdate date inserted or updated");
			}
			catch (SQLite.SQLiteException ex)
			{
#if DEBUG
				throw ex;
#else
				Console.WriteLine(ex.Message);
#endif
			}
		}

	public List<ItemsPictures> GetImagesFromAzure()
		{
			List<ProductImage> images = new List<ProductImage>();
			List<ItemsPictures> resultImages;
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				return null;
			}
			else
			{
				using (var Client = new HttpClient())
				{
					Client.BaseAddress = new Uri(AzureService.DefaultService.ApplicationURL);

					Client.DefaultRequestHeaders.Accept.Clear();
					Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
					Client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
					var response = Client.GetAsync(String.Format("api/images/")).Result;

					if (!response.IsSuccessStatusCode)
					{

						var alert = new UIAlertView("Could Not Find Item", String.Format("No item exist."), null, "Ok");
						alert.Show();
						return null;
					}
					else
					{
						var json = response.Content.ReadAsStringAsync().Result;
						resultImages = JsonConvert.DeserializeObject<List<ItemsPictures>>(json);
					}
				}
			}
			return resultImages;
		}

		public void UpdateImagePositions(InventoryNS item)
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				return;
			}
			else
			{

				using (var Client = new HttpClient())
				{
					Client.BaseAddress = new Uri(AzureService.DefaultService.ApplicationURL);

					Client.DefaultRequestHeaders.Accept.Clear();
					Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
					Client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");

					foreach (var image in item.GridImages)
					{
						var positionUpdate = new PicturePositionPut();
						positionUpdate.pictureName = image.PictureName;
						positionUpdate.position = image.SortOrder;
						var httpContent = new StringContent(JsonConvert.SerializeObject(positionUpdate), Encoding.UTF8, "application/json");
						var response = Client.PostAsync(String.Format("api/images/updateposition"), httpContent).Result;

						if (!response.IsSuccessStatusCode)
						{
							var alert = new UIAlertView("Could Not Find Image", String.Format("No image with name {0} exist.", image.PictureName), null, "Ok");
							alert.Show();
							//return;
						}
						//return;
					}
				}
			}
		}

		public bool DeleteImage(string pictureName)
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				return false;
			}
			else
			{
				using (var Client = new HttpClient())
				{
					Client.BaseAddress = new Uri(AzureService.DefaultService.ApplicationURL);

					Client.DefaultRequestHeaders.Accept.Clear();
					Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
					Client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
					var response = Client.GetAsync(String.Format("api/images/delete/{0}", pictureName)).Result;

					if (!response.IsSuccessStatusCode)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
			}
		}
		public string AddImageToAzure(GridImage image)
		{
			PicturePost pp = new PicturePost();
			pp.itemId = image.OriginalId;
			NSData imageData = image.Image.AsJPEG(0.5f);

			string encodedImage = imageData.GetBase64EncodedData(NSDataBase64EncodingOptions.None).ToString();
			pp.picturebase64 = encodedImage;

			try
			{
				if (!Reachability.IsHostReachable("http://google.com"))
				{
					var alert = new UIAlertView("Could Add Image", "Image add failed. Please make sure you are connected to the internet.", null, "Ok");
					alert.Show();
					return null;
				}
				else
				{
					using (var Client = new HttpClient())
					{


						Client.BaseAddress = new Uri(AzureService.DefaultService.ApplicationURL);

						Client.DefaultRequestHeaders.Accept.Clear();
						Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
						Client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
						var httpContent = new StringContent(JsonConvert.SerializeObject(pp), Encoding.UTF8, "application/json");
						var response = Client.PostAsync("api/images", httpContent).Result;

						if (!response.IsSuccessStatusCode)
						{

							var alert = new UIAlertView("Could Not Add Image", String.Format("No item with id {0} exist.", image.OriginalId), null, "Ok");
							alert.Show();
							return null;
						}
						else
						{
							var json = response.Content.ReadAsStringAsync().Result;
							var pictureName = JsonConvert.DeserializeObject<string>(json);
							return pictureName;

						}
					}
				}


			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
				return null;
			}
		}


	}
}

