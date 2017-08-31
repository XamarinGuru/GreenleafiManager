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

namespace GreenleafiManagerPhone
{
	public class InventoryService
	{
		AzureService _Parent;
		//static InventoryService instance = new InventoryService();

		//private IMobileServiceSyncTable<Item> ItemTable;
		//private IMobileServiceSyncTable<InventoryImage> InventoryImageTable;
		private IMobileServiceSyncTable<GLCode> GLCodeTable;
		private IMobileServiceSyncTable<StoneCode> StoneCodeTable;
		private IMobileServiceSyncTable<MetalCode> MetalCodeTable;
		private IMobileServiceSyncTable<ItemCode> ItemCodeTable;
		private IMobileServiceSyncTable<VendorId> VendorIdTable;


		public InventoryService(AzureService parent)
		{
			_Parent = parent;

			// Create an MSTable instance to allow us to work with the InventoryItem table
			GLCodeTable = _Parent.Client.GetSyncTable<GLCode>();
			StoneCodeTable = _Parent.Client.GetSyncTable<StoneCode>();
			MetalCodeTable = _Parent.Client.GetSyncTable<MetalCode>();
			ItemCodeTable = _Parent.Client.GetSyncTable<ItemCode>();
			VendorIdTable = _Parent.Client.GetSyncTable<VendorId>();

		}

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
					Client.BaseAddress = new Uri(AzureService.DefaultService.ApplicationURL);

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
			await MetalCodeTable.PurgeAsync("allMetalCode", "", true, new System.Threading.CancellationToken());
            await ItemCodeTable.PurgeAsync("allItemCodes", "", true, new System.Threading.CancellationToken());
            await StoneCodeTable.PurgeAsync("allStoneCode", "", true, new System.Threading.CancellationToken());
            await GLCodeTable.PurgeAsync("allGLCode", "", true, new System.Threading.CancellationToken());
            await VendorIdTable.PurgeAsync("allVendorIds", "", true, new System.Threading.CancellationToken());
		}
		private async Task PullAsync()
		{
			try
			{
				await _Parent.LocationSetupConfirmationWithSync();

				//await _Parent.Client.SyncContext.PushAsync();
				await PullAllCodes();
				await UpdateVendorIds();

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

		public Item InsertOrSave(Item item)
		{
			try
			{
				if (!Reachability.IsHostReachable("http://google.com"))
				{
					var alert = new UIAlertView("Could Not Save Item", "Item failed to save. Please make sure you are connected to the internet.", null, "Ok");
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
						var httpContent = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
						var response = Client.PostAsync("tables/Reports/SaveOrUpdateItem", httpContent).Result;

						if (!response.IsSuccessStatusCode)
						{
							
							var alert = new UIAlertView("Could Not Find Item",String.Format( "No item with SKU {0} exist.",item.Sku), null, "Ok");
							alert.Show();
							return null;
						}
						else
						{
							var json = response.Content.ReadAsStringAsync().Result;
							if (!String.IsNullOrEmpty(json) && json != "null")
								item = JsonConvert.DeserializeObject<Item>(json);
							else
							{
								var alert = new UIAlertView("Could Not Save Item", String.Format("Item {0} failed to save.", item.Sku), null, "Ok");
								alert.Show();
							}
							return item;

						}
					}
				}


			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
				return null;
			}
			return null;
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

		public List<ProductImage> GetImagesFromAzure(string sku)
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
					var response = Client.GetAsync(String.Format("api/images/{0}", sku)).Result;

					if (!response.IsSuccessStatusCode)
					{
						
						var alert = new UIAlertView("Could Not Find Item", String.Format("No item with SKU {0} exist.", sku), null, "Ok");
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
			if (resultImages != null && resultImages.Count > 0 )
			{
				foreach (var ri in resultImages)
				{
					var webClient = new WebClient();
					byte[] imageBytes = webClient.DownloadData(ri.PictureURL);

					//byte[] encodedDataAsBytes = System.Convert.FromBase64String(imageBytes.ToString());
					//NSData data = NSData.FromArray(imageBytes);

					var pi = new ProductImage();
					pi.position = ri.position;
					pi.ImageBytes = imageBytes;
					pi.OriginalId = ri.OriginalId;
					pi.PictureName = ri.PictureName;


					images.Add(pi);
				}
				return images;
			}
			//else
			//{
			//	//THIS ONE
			//	var alert = new UIAlertView("Could Not Find Item", String.Format("No item with SKU {0} exist.", sku), null, "Ok");
			//	alert.Show();
			//	return null;
			//}
			return null;
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
		ShopifyAdapter sad = new ShopifyAdapter();

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
	}
}

