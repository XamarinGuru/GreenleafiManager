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

namespace GreenleafiManager
{
	public class InventoryService : AzureService
	{
		static InventoryService instance = new InventoryService();

		private MobileServiceClient client;
		private IMobileServiceSyncTable<Item> ItemTable;
		//private IMobileServiceSyncTable<InventoryImage> InventoryImageTable;
		private IMobileServiceSyncTable<GLCode> GLCodeTable;
		private IMobileServiceSyncTable<StoneCode> StoneCodeTable;
		private IMobileServiceSyncTable<MetalCode> MetalCodeTable;
		private IMobileServiceSyncTable<ItemCode> ItemCodeTable;
		private IMobileServiceSyncTable<VendorId> VendorIdTable;


		public static void RefreshFromServerDB()
		{

			instance = new InventoryService();

		}
		private InventoryService()
		{
			CurrentPlatform.Init();
			SQLitePCL.CurrentPlatform.Init();
			//SqLz
			//RMR 2016-07-18 SetupInventoryTable();
			// Initialize the Mobile Service client with the Mobile App URL, Gateway URL and key
			client = new MobileServiceClient(ApplicationURL);//, new NativeMessageHandler());

			// Create an MSTable instance to allow us to work with the InventoryItem table
			GLCodeTable = client.GetSyncTable<GLCode>();
			StoneCodeTable = client.GetSyncTable<StoneCode>();
			MetalCodeTable = client.GetSyncTable<MetalCode>();
			ItemCodeTable = client.GetSyncTable<ItemCode>();
			VendorIdTable = client.GetSyncTable<VendorId>();
			ItemTable = client.GetSyncTable<Item>();
			//InventoryImageTab le = client.GetSyncTable <InventoryImage> ();
		}
		public bool NeedServerRefresh { get; set; }
		public bool NeedLocalRefresh { get; set; }
		public bool NeedImageRefresh { get; set; }

		public static InventoryService DefaultService
		{
			get
			{
				if (instance == null)
					instance = new InventoryService();
				return instance;
			}
		}
		public string GetNextSku()
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				return "";
			}
			else
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(ApplicationURL);

					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
					client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
					var response = client.GetAsync("tables/Reports/GetNextSKU").Result;

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
		public async Task<List<InventoryNS>> GetNSItems()
		{
			List<InventoryNS> nsitems = new List<InventoryNS>();
			await SyncAsync();
			await RefreshDataAsync();
			//RefreshData ();
			await SyncAllCodes();
			await UpdateVendorIds();
			await UpdateGLCodes();
			await UpdateItemCodes();

			await UpdateMetalCodes();
			//await SyncAllCodes();
			await SyncAsync();
			await UpdateStoneCodes();

			//			if (Items == null || Items.Count == 0) {
			//				await WipeLocalDBAndUpdateFromShopify ();
			//			}
			foreach (var i in Items)
			{
				nsitems.Add(new InventoryNS(i));
			}
			return nsitems;
		}

		public async Task<List<InventoryNS>> GetLocalNSItems()
		{
			if (ItemsNS == null)
			{
				ItemsNS = new List<InventoryNS>();
				if (Items == null)
				{
					await InitializeStoreAsync();
					await SyncAsync();
				}
				if (Items != null)
					foreach (var i in Items)
					{
						ItemsNS.Add(new InventoryNS(i));
					}
			}
			return ItemsNS;
		}
		public async Task<List<InventoryNS>> GetLocalNSItems(bool sold)
		{
			if (ItemsNS == null)
			{
				ItemsNS = new List<InventoryNS>();
				if (Items == null)
				{
					await InitializeStoreAsync();
					await SyncAsync();
				}
				if (Items != null)
					foreach (var i in Items)
					{
						ItemsNS.Add(new InventoryNS(i));
					}
			}
			return ItemsNS.Where(x => x.IsSold == sold).ToList();
		}

		public List<Item> Items { get; private set; }
		public List<InventoryNS> ItemsNS { get; private set; }
		public List<ProductData> ItemsProductDatas
		{
			get
			{
				var localProducts = new List<ProductData>();
				foreach (var item in Items)
				{
					localProducts.Add(item.GetProductDataForShopifyPost());
				}
				return localProducts;
			}
		}
		//		public List<InventoryImage> _Images;
		//
		//		public List<InventoryImage> Images { 
		//			get { 
		//				if (_Images == null)
		//					setUpImages ();
		//				return _Images;
		//			}
		//		}

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
			await SyncGLCodes();
			_GLCodes = await GLCodeTable.ToListAsync();
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
			await SyncMetalCodes();
			_MetalCodes = await MetalCodeTable.ToListAsync();
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
			await SyncStoneCodes();
			_StoneCodes = await StoneCodeTable.ToListAsync();
		}

		private List<ItemCode> _ItemCodes;

		public List<ItemCode> ItemCodes
		{
			get
			{
				return _ItemCodes;
			}
		}
		public async Task UpdateItemCodes()
		{
			await SyncItemCodes();
			_ItemCodes = await ItemCodeTable.ToListAsync();
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
					"Alexandrite",
					"Opal",
					"Gold Nugget",
					"Quartz",
					"Amethyst",
					"Peridot",
					"Aquamarine",
					"Tourmaline",
					"Watch",
					"Other"
				};
			}
		}
		public List<string> Locations
		{
			get
			{
				return new List<String> {
					"Maui",
					"L.A.",
					"Alaska"
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
			await SyncVendorIds();
			_VendorIds = await VendorIdTable.ToListAsync();
		}

		//		private async void setUpImages()
		//		{
		//			_Images = await InventoryImageTable.ToListAsync ();
		//		}
		//		public InventoryImage GetImage(string inventoryId)
		//		{
		//			if (Images.Count == 0)
		//				return null;
		//			return Images.Where (i => i.InventoryId == inventoryId).First ();
		//		}
		public async Task<bool> InitializeStoreAsync()
		{
			try
			{
				var store = new MobileServiceSQLiteStore(LocalDbPath);
				store.DefineTable<Item>();
				store.DefineTable<GLCode>();
				store.DefineTable<StoneCode>();
				store.DefineTable<MetalCode>();
				store.DefineTable<ItemCode>();
				store.DefineTable<VendorId>();
				//store.DefineTable<Payment>();
				// Uses the default conflict handler, which fails on conflict
				// To use a different conflict handler, pass a parameter to InitializeAsync. For more details, see http://go.microsoft.com/fwlink/?LinkId=521416
				await client.SyncContext.InitializeAsync(store);
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
				Console.Write (ex.Message);
				return false;
#endif


			}
			return true;
		}


		public async Task SyncAsync()
		{
			try
			{
				if (LocationService.DefaultService.Locations == null || LocationService.DefaultService.Locations.Count == 0)
				{
					await LocationService.DefaultService.SyncAsync();
				}

				await client.SyncContext.PushAsync();
				await UpdateMetalCodes();
				await UpdateItemCodes();
				await UpdateGLCodes();
				await UpdateVendorIds();
				await GetLocalNSItems();
				await ItemTable.PullAsync("allItems", ItemTable.CreateQuery()); // query ID is used for incremental sync
				Items = await ItemTable.ToListAsync();

			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{

			}
		}

		public async Task SyncSingle(Item item)
		{
			try
			{
				if (LocationService.DefaultService.Locations == null || LocationService.DefaultService.Locations.Count == 0)
				{
					await LocationService.DefaultService.SyncAsync();
				}
				ShopifyAdapter sa = new ShopifyAdapter();
				sa.CheckAndDownloadImagesForProduct(item.GetProductDataForShopifyPost());

			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task SyncAllCodes()
		{
			try
			{
				await client.SyncContext.PushAsync();
				await ItemCodeTable.PullAsync("allItemCode", ItemCodeTable.CreateQuery()); // query ID is used for incremental sync
				await StoneCodeTable.PullAsync("allStoneCode", StoneCodeTable.CreateQuery()); // query ID is used for incremental sync
				await MetalCodeTable.PullAsync("allMetalCode", MetalCodeTable.CreateQuery()); // query ID is used for incremental sync
				await GLCodeTable.PullAsync("allGLCode", GLCodeTable.CreateQuery()); // query ID is used for incremental sync
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
		public async Task SyncGLCodes()
		{
			try
			{
				await client.SyncContext.PushAsync();
				await GLCodeTable.PullAsync("allGLCode", GLCodeTable.CreateQuery()); // query ID is used for incremental sync
			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync allGLCode Failed: {0}", e.Message);
			}
		}
		public async Task SyncItemCodes()
		{
			try
			{
				await client.SyncContext.PushAsync();
				await ItemCodeTable.PullAsync(null, ItemCodeTable.CreateQuery()); // query ID is used for incremental sync
			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync allItemCode Failed: {0}", e.Message);
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


		public async Task SyncStoneCodes()
		{
			try
			{
				await client.SyncContext.PushAsync();
				await StoneCodeTable.PullAsync("allStoneCode", StoneCodeTable.CreateQuery()); // query ID is used for incremental sync
			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync allStoneCode Failed: {0}", e.Message);
			}
		}
		public async Task SyncMetalCodes()
		{
			try
			{
				await client.SyncContext.PushAsync();
				await MetalCodeTable.PullAsync("allMetalCode", MetalCodeTable.CreateQuery()); // query ID is used for incremental sync

			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sycn allMetalCode Failed: {0}", e.Message);
			}
		}
		public async Task SyncVendorIds()
		{
			try
			{
				await client.SyncContext.PushAsync();
				await VendorIdTable.PullAsync("allVendorIds", VendorIdTable.CreateQuery()); // query ID is used for incremental sync

			}

			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sycn allVendorId Failed: {0}", e.Message);
			}
		}

		public async Task<List<Item>> RefreshDataAsync()
		//public List<Inventory> RefreshData ()
		{
			try
			{
				// update the local store
				// all operations on InventoryItemTable use the local database, call SyncAsync to send changes

				await SyncAsync();

				// This code refreshes the entries in the list view by querying the local InventoryItems table.
				// The query excludes completed InventoryItems

				Items = await ItemTable.ToListAsync();

				//Via SHopify
				//				var itemsPD = GetShopifyProdcutDatas ();
				//				Items = new List<Inventory>();
				//
				//				foreach(var pd in itemsPD)
				//				{
				//					Items.Add(new Inventory(pd));
				//				}                    

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
				return null;
			}

			return Items;
		}

		public async Task InsertInventoryItemAsync(Item InventoryItem)
		{
			try
			{
				await ItemTable.InsertAsync(InventoryItem); // Insert a new InventoryItem into the local database. 
															//await SyncAsync(); // send changes to the mobile service

				Items.Add(InventoryItem);

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		//public async Task InsertOrSave (Inventory item)
		public async Task InsertOrSave(Item item, List<ProductImage> productimagesList = null)
		{//RMR here for updates and inserts
			try
			{
				//var sa = new ShopifyAdapter ();

				if (Items.Any(x => x.Id == item.Id))
				{
					await ItemTable.UpdateAsync(item); // update InventoryItem item in the local database
					var foundItem = Items.Where(x => x.Id == item.Id).First();
					//foundItem.GlCode = item.GlCode;
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
					//await UpdateShopifyProduct(foundItem.GetProductDataForShopifyPost());
				}
				else {//new
					  //item.GlCode = "1";
					item.VendorCode = "GL";
					item.Description = "";
					//item.LocationId = null;
					item.SearchString = "";
					await ItemTable.InsertAsync(item); // Insert a new InventoryItem into the local database. 
					item.VendorCode = "GL";
					Items.Add(item);
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


				await SyncAsync(); // send changes to the mobile service


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
				using (var conn = new SQLite.SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LocalDbPath)))
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

			var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LocalDbPath));
			var foundItem = await db.FindAsync<Item>(x => x.Id == id);

			if (foundItem != null)
			{
				await db.DeleteAsync(foundItem);
			}
		}

		public async Task PushDataToServer()
		{
			try
			{
				await client.SyncContext.PushAsync();
				//await ItemTable.PullAsync("allItems", GLCodeTable.CreateQuery()); // query ID is used for incremental sync
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"Sync allGLCode Failed: {0}", e.Message);
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
				var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LocalDbPath));
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
				using (var conn = new SQLite.SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LocalDbPath)))
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

			var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LocalDbPath));
			var foundItem = await db.FindAsync<GlobalVariables>(x => x.Id == 1);

			return foundItem == null ? DateTime.Parse("1/1/1901") : foundItem.LastUpdated;
		}
		public async Task SetLastUpdatedDateTime(DateTime updateDateTime)
		{
			if (!SQLiteDBGlobalsIsSetup)
				SetupGlobalsTable();
			try
			{
				var db = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), LocalDbPath));
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
	}
}

