using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Net.Mail; 
using ShopifyAPIAdapterLibrary;
using GreenleafiManager;
using UIKit;
using Foundation;
using SQLite;


namespace GreenleafiManager
{
	#region API Required Objects
	public class GetOrderReply
	{
		public List<Order> orders { get; set; }
	}
	public class Order
	{
		public string name { get; set; }
		public string id { get; set; }
		public string fulfillment_status { get; set; }
		public string created_at { get; set; }
		public string Published { get; set; }

		public List<LineItem> line_items { get; set; }
		public ShopifyCustomer customer { get; set; }
	}
	public class LineItem
	{
		public string id { get; set; }
		public string price { get; set; }
		public string product_id { get; set; }
		public string quantity { get; set; }
		public string sku { get; set; }
		public string title { get; set; }
		public string name { get; set; }
	}
	public class ShopifyCustomer
	{
		public string id { get; set; }
		public string email { get; set; }
	}
	public class GetProductReply
	{
		public ProductData product { get; set; }
	}
	public class GetProductsReply
	{
		public List<ProductData> products { get; set; }
	}
	public class GetProductImageReply
	{
		public ProductImage image { get; set; }
	}
	public class GetProductImagesReply
	{
		public List<ProductImage> images { get; set; }
	}
	public class ProductVariant
	{
		public string id { get; set; }
		public string price { get; set; }
		public string product_id { get; set; }
		public string sku { get; set; }
		public string title { get; set; }
		public string name { get; set; }
		public string created_at { get; set; }
		public string updated_at {get; set;	}
	}
	public class ProductImage 
	{
		public DateTime created_at {
			get;
			set;
		}
		[PrimaryKey, Unique]
		public string id {
			get;
			set;
		}
		public string position {
			get;
			set;
		}
		public string product_id {
			get;
			set;
		}
		public string src {
			get;
			set;
		}
		public string filename {
			get;
			set;
		}
		public DateTime updated_at {
			get;
			set;
		}
		private byte[] _ImageBytes;
		//[JsonIgnore,Ignore]
		public byte[] ImageBytes{ 
			get{
				if(_ImageBytes == null){	
					var documentsDirectory = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
					string jpgFilename = System.IO.Path.Combine (documentsDirectory, filename);

					var localImage = System.IO.File.Exists (jpgFilename) ? UIImage.FromFile (jpgFilename) : null;

					if (localImage != null) {
						_ImageBytes = Extensions.ToNSData (localImage);
					}				
				}
				return _ImageBytes;
			}
			set{ _ImageBytes = value; }
		}

		[JsonIgnore,Ignore]
		public string ImageId { get; set; }

		[JsonIgnore, Ignore]
		public string OriginalId { get; set; }
		[JsonIgnore, Ignore]
		public string PictureName { get; set; }

		[JsonIgnore, Ignore]
		public string LocalPath
		{
			get
			{
				var documentsDirectory = Environment.GetFolderPath
					(Environment.SpecialFolder.Personal);

				var path = System.IO.Path.Combine(documentsDirectory, filename); // update it for file number
				return path;

			} 
		}
		public string GetShopifyProductImagePostJSON()
		{
			return String.Format ("{{ \"image\": " +
				"{{ \"prodcut_id\": \"{0}\", " +
				"\"position\": \"{1}\", " +
				"\"attachment\": \"{2}\", " +
				"\"filename\": \"{3}\"" +
				"}} }}",
				product_id, position, Extensions.ImageToBase64(ImageBytes),filename);
		}
		public string GetShopifyProductUpdateImagePositionPutJSON()
		{
			return String.Format ("{{ \"image\": " +
				"{{ \"prodcut_id\": \"{0}\", " +
				"\"position\": \"{1}\", " +
				"\"filename\": \"{3}\", " +
				"\"id\": \"{2}\"" +
				"}} }}",
				product_id, position, id,filename);
		}

		public bool IsImageInLocalFolder(string filename)
		{
			var documentsDirectory = Environment.GetFolderPath
				(Environment.SpecialFolder.Personal);

			var jpgFilename = System.IO.Path.Combine(documentsDirectory, filename); // update it for file number
			if (System.IO.File.Exists(jpgFilename))
				return true;
			return false;

		}
		public void DownloadImageFromShopify()
		{
			if (!IsImageInLocalFolder(this.filename))
			{
				var webClient = new WebClient();
				byte[] imageBytes = webClient.DownloadData(this.src);
				this.ImageBytes = imageBytes;
				var uiImage = Extensions.ToImage(imageBytes);
				var imgData = uiImage.AsJPEG();

				var documentsDirectory = Environment.GetFolderPath
					(Environment.SpecialFolder.Personal);
				var jpgFilename = System.IO.Path.Combine(documentsDirectory, this.filename); // update it for file number

				NSError err = null;
				if (imgData.Save(jpgFilename, false, out err))
				{
					Console.WriteLine("saved as " + jpgFilename);
				}
				else {
					Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
				}
				webClient.Dispose();
				imageBytes = null;
				uiImage.Dispose();
				imgData.Dispose();

			}
		}
	}

	public class ProductData
	{
		public Item InventoryItem { get; set; }
		public List<ProductImage> Images{ get; set; }
		public bool IsNew { get; set; }
		public string id { get; set; }
		public string Title { get; set; }
		public string body_html { get; set; }
		public string Vendor { get; set; }
		public string ProductType { get; set; }
		public string Published { get; set; }
		public string SKU { 
			get { 
				if (variants != null && variants.Count > 0) {
					return variants.First ().sku;
				} 
				else {
					return "";
				}
			}
			set{
				if (variants != null && variants.Count > 0) {
					variants.First ().sku = value;
				} 
			}
		}
		public String CreatedAt { 
			get { 
				if (variants != null && variants.Count > 0) {
					return variants.First ().created_at;
				} 
				else {
					return null;
				}
			}
			set{
				if (variants != null && variants.Count > 0) {
					variants.First ().created_at = value;;
				} 
			}
		}
		public String UpdatedAt { 
			get { 
				if (variants != null && variants.Count > 0) {
					return variants.First ().updated_at;
				} 
				else {
					return null;
				}
			}
			set{
				if (variants != null && variants.Count > 0) {
					variants.First ().updated_at = value;;
				} 
			}
		}

		public string Price { get; set; }
		public string ShopifyId { get; set; }
		public string ShopifyVariantId {
			get { 
				if (variants != null && variants.Count > 0) {
					return variants.First ().id;
				} else {
					return "";
				}
			}
			set {
				if (variants != null && variants.Count > 0) {
					variants.First ().id = value;
				} 
			}
		}
		public string tags { get; set; }

		public List<ProductVariant> variants { get; set; }

		public string GetShopifyProductPostJSON()
		{
			return String.Format ("{{ \"product\": " +
				"{{ \"title\": \"{0}\", " +
				"\"body_html\": \"{1}\", " +
				"\"vendor\": \"{2}\", " +
				"\"product_type\": \"{3}\"," +
				"\"tags\": \"{4}\"," +
				"\"variants\": [" +
				"{{" +
				"\"price\": \"{5}\"," +
				"\"sku\": \"{6}\"," +
				"\"barcode\": \"{6}\"" +
				"}}]" +
				"}} }}",
				Title, body_html, Vendor, ProductType, tags, Price, SKU);
		}
		public string GetShopifyProductPutJSON()
		{
			return String.Format ("{{ \"product\": " +
				"{{ \"title\": \"{0}\", " +
				"\"id\": \"{7}\", " +
				"\"body_html\": \"{1}\", " +
				"\"vendor\": \"{2}\", " +
				"\"product_type\": \"{3}\"," +
				"\"tags\": \"{4}\"," +
				"\"variants\": [" +
				"{{" +
				"\"id\": \"{8}\"," +
				"\"price\": \"{5}\"," +
				"\"sku\": \"{6}\"," +
				"\"barcode\": \"{6}\"" +
				"}}]" +
				"}} }}",
				Title, body_html, Vendor, ProductType, tags, Price, SKU,id,ShopifyVariantId );
		}
		//		public string GetPostImageShopifyJSON()
		//		{
		//			return String.Format ("{{ \"image\": " +
		//				"{{ \"attachment\": \"{1}\", " +
		//				"\"filename\": \"{2}\" " +
		//				"}} }}",
		//				Convert.ToBase64String(Images.ImageBytes), Images.src);
		//		}

		public string GetUpdateFulfillmentShopifPutJSON(string fulfillment)
		{
			return String.Format("{{\"product\": {{ " +
				"\"fulfillment_status\": \"{0}\"}} }} ", fulfillment);
		}
		public string GetUpdateScopeShopifPutJSON(string scope)
		{
			return String.Format("{{\"product\": {{ " +
				"\"published_scope\": \"{0}\"}} }} ", scope);
		}
		public void SetDefaultImageFilenames()
		{
			if (Images == null)
				return;
			for (int i =0; i < Images.Count(); i++) {

				if (String.IsNullOrWhiteSpace (Images[i].filename)) {

					Images[i].filename = String.Format("{0}{1}.jpg",SKU, i == 0 ? "" : "_" + i ); 

				}
			}
		}

	}

	#endregion

	public class ShopifyAdapter
	{
		#region constants
		private const string ShopifyAPIKey = "73ec78f8fdbcd5583cf4bb72a7c64887";
		private const string ShopifyPassword = "cf67c08d6aa5f0729af7d611d11c8cf1";
		private const string ShopifySecret = "67bb495078076c1c640030caf383f723";
		private const string ShopifyStoreURL = "greenleafdiamondsstore";

		ShopifyAPIClient _ShopifyAPIClient;
		ShopifyAPIClient shopifyAPIClient
		{
			get
			{
				if(_ShopifyAPIClient == null)
				{
					_ShopifyAPIClient = new ShopifyAPIClient(shopifyAuthorizationState);
				}
				return _ShopifyAPIClient;
			}
		}

		ShopifyAuthorizationState _ShopifyAuthorizationState;
		ShopifyAuthorizationState shopifyAuthorizationState {
			get
			{
				if (_ShopifyAuthorizationState == null)
				{
					SetupAuthorization();
				}
				return _ShopifyAuthorizationState;
			}  }

		#endregion

		#region Shopify Calls
		public void SetupAuthorization()
		{
			_ShopifyAuthorizationState = new ShopifyAuthorizationState();
			_ShopifyAuthorizationState.ShopName = ShopifyStoreURL;
			_ShopifyAuthorizationState.AccessToken = ShopifyPassword;
		}
		public async Task<ProductData> PostShopifyProduct(ProductData product)
		{
			try {
				string data = product.GetShopifyProductPostJSON();
				//byte[] dataStream = Encoding.UTF8.GetBytes(data);

				string json = (await shopifyAPIClient.Post(String.Format("/admin/products.json"), data)).ToString();
				ProductData pd = JsonConvert.DeserializeObject<GetProductReply>(json).product;
				pd.id = pd.id;
				//pd.ShopifyVariantId = pd.variants.First().id;
				pd.Price = product.Price;
				return pd;

			}

			catch (Exception e)
			{
				#if DEBUG
				throw e;

				#else
				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));
				#endif


				return null;
			}


		}
		public async Task<List<ProductData>> GetShopifyProducts()
		{
			try
			{
				List<ProductData> spProducts = new List<ProductData>();

				string jsonCount = (await shopifyAPIClient.Get("/admin/products/count.json")).ToString();
				string countString = jsonCount.Substring(jsonCount.IndexOf(":")+ 1,  jsonCount.Length - jsonCount.IndexOf(":") - 2);
				int count = int.Parse(countString.Trim());
				int page = 0;

				while(page * 50 < count)
				{
					if (page+1 % 39 == 0) {
						await Task.Delay (20 * 1000);
					}

					string jsonRequestURL = String.Format("/admin/products.json?page={0}",page);
					string json = (await shopifyAPIClient.Get(jsonRequestURL)).ToString();
					GetProductsReply pdReply = JsonConvert.DeserializeObject<GetProductsReply>(json);
					spProducts.AddRange(pdReply.products);		
					page++;
				}
				return spProducts;

			}
			catch (Exception e)
			{
				#if DEBUG
				throw e;
				#else
				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));

				return null;// String.Format("Shopify Get Failed - Message: {0}\n Inner Message: {1}", e.Message, e.InnerException == null ? "none" : e.InnerException.Message);
				#endif



			}

		}

		public void CheckAndDownloadImagesForProduct(ProductData shopifyItem)
		{
			shopifyItem.SetDefaultImageFilenames();
			foreach (var img in shopifyItem.Images)
			{
				img.DownloadImageFromShopify();
			}
			shopifyItem = null; 
		}
		public void CheckAndDownloadImagesForProducts(List<ProductData> shopifyItems)
		{
			List<ProductData> pdsWithImages = shopifyItems.Where(x => x.Images != null && x.Images.Count > 0).ToList();
			foreach (var si in pdsWithImages )
			{
				CheckAndDownloadImagesForProduct(si);
			}
			pdsWithImages = null; 
		}
		public async Task CheckAndDownloadAllProductImages()
		{
			var pds = await GetShopifySkinnyProducts();
			CheckAndDownloadImagesForProducts(pds);
			pds = null; 
		}
		public async Task<List<ProductData>> GetShopifySkinnyProducts()
		{
			try
			{
				List<ProductData> spProducts = new List<ProductData>();

				string jsonCount = (await shopifyAPIClient.Get("/admin/products/count.json")).ToString();
				string countString = jsonCount.Substring(jsonCount.IndexOf(":")+ 1,  jsonCount.Length - jsonCount.IndexOf(":") - 2);
				int count = int.Parse(countString.Trim());
				int page = 0;

				while(page * 50 < count)
				{
					if (page + 1 % 39 == 0) {
						await Task.Delay (20 * 1000);
					}
					string jsonRequestURL = String.Format("/admin/products.json?page={0},fields=id,variants,images",page);
					string json = (await shopifyAPIClient.Get(jsonRequestURL)).ToString();
					GetProductsReply pdReply = JsonConvert.DeserializeObject<GetProductsReply>(json);
					spProducts.AddRange(pdReply.products);
					page++;
				}
				return spProducts;
			}
			catch (Exception e)
			{
				#if DEBUG
				throw e;
				#endif

				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));

				return null;// String.Format("Shopify Get Failed - Message: {0}\n Inner Message: {1}", e.Message, e.InnerException == null ? "none" : e.InnerException.Message);
			}

		}
		public async Task<List<ProductData>> GetShopifySkinnyProductsSinceDate(DateTime date)
		{
			try
			{

				try
				{

					List<ProductData> spProducts = new List<ProductData>();

					string jsonCount = (await shopifyAPIClient.Get(String.Format("/admin/products/count.json?updated_at_min={0}", (date).ToString("yyyy-MM-dd-hh:mm")))).ToString();
					string countString = jsonCount.Substring(jsonCount.IndexOf(":")+ 1,  jsonCount.Length - jsonCount.IndexOf(":") - 2);
					int count = int.Parse(countString.Trim());
					int page = 0;

					while(page * 50 < count)
					{
						if (page+1 % 39 == 0) {
							await Task.Delay (20 * 1000);
						}
						string jsonRequestURL = String.Format("/admin/products.json?updated_at_min={1},page={0},fields=id,variants",page,(date).ToString("yyyy-MM-dd"));
						string json = (await shopifyAPIClient.Get(jsonRequestURL)).ToString();
						GetProductsReply pdReply = JsonConvert.DeserializeObject<GetProductsReply>(json);
						spProducts.AddRange(pdReply.products);
						page++;
					}
					return spProducts;
				}
				catch (Exception e)
				{
					#if DEBUG
					throw e;
					#endif

					Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));

					return null;
				}

				//				string json = shopifyAPIClient.Get("/admin/products.json?fields=id,variants").ToString();
				//				GetProductsReply pdReply = JsonConvert.DeserializeObject<GetProductsReply>(json);
				//				return pdReply.products;

			}
			catch (Exception e)
			{
				#if DEBUG
				throw e;
				#endif

				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));

				return null;// String.Format("Shopify Get Failed - Message: {0}\n Inner Message: {1}", e.Message, e.InnerException == null ? "none" : e.InnerException.Message);
			}

		}
		public async Task<ProductData> GetShopifySkinnyProduct(string id)
		{
			try
			{
				var json = (await shopifyAPIClient.Get(String.Format("/admin/products/{0}.json?fields=id,variants",id))).ToString();
				GetProductReply pdReply = JsonConvert.DeserializeObject<GetProductReply>(json);
				return pdReply.product;

			}
			catch (Exception e)
			{
				#if DEBUG
				throw e;
				#endif

				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));

				return null;// String.Format("Shopify Get Failed - Message: {0}\n Inner Message: {1}", e.Message, e.InnerException == null ? "none" : e.InnerException.Message);
			}

		}
		public async Task<List<ProductData>> GetShopifyProductsSinceXDays(int days)
		{
			try
			{
				string json = (await shopifyAPIClient.Get(String.Format("/admin/products.json?published_at_min={0}", (DateTime.Now - new TimeSpan(days, 0, 0, 0)).ToString("yyyy-MM-dd")))).ToString();
				GetProductsReply pdReply = JsonConvert.DeserializeObject<GetProductsReply>(json);
				return pdReply.products;
			}
			catch (Exception e)
			{
				#if DEBUG
				throw e;
				#endif

				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));

				return null;
			}
		}

		public async Task<ProductData> GetShopifyProduct(string id)
		{
			try
			{

				string json = (await shopifyAPIClient.Get(String.Format("/admin/products/{0}.json", id))).ToString();
				ProductData pd = JsonConvert.DeserializeObject<GetProductReply>(json).product;
				return pd;
			}
			catch (Exception e)
			{
				#if DEBUG
				throw e;
				#endif

				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));

				return null;
			}

		}

		public async Task<ProductImage> PostShopifyProductImage(ProductImage image)
		{
			try {
				string data = image.GetShopifyProductImagePostJSON();
				//byte[] dataStream = Encoding.UTF8.GetBytes(data);

				var json = (await shopifyAPIClient.Post(String.Format("/admin/products/{0}/images.json", image.product_id), data));
				ProductImage pi = JsonConvert.DeserializeObject<GetProductImageReply>(json.ToString()).image;
				image.id = pi.id;
				image.src = pi.src;
				image.position = pi.position;
				image.updated_at = pi.updated_at;
				image.created_at = pi.created_at;
				return pi;
			}

			catch (Exception e)
			{
				#if DEBUG
				throw e;
				#endif

				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));
				return null;
			}
		}
		public async Task<ProductImage> UpdateShopifyProductImage(ProductImage image)
		{
			try {
				string data = image.GetShopifyProductUpdateImagePositionPutJSON();
				//byte[] dataStream = Encoding.UTF8.GetBytes(data);

				string json = (await shopifyAPIClient.Put(String.Format("/admin/products/{0}/images/{1}.json", image.product_id,image.id), data)).ToString();
				ProductImage pi = JsonConvert.DeserializeObject<GetProductImageReply>(json).image;			
				image.position = pi.position;
				image.updated_at = pi.updated_at;
				return pi;
			}

			catch (Exception e)
			{
				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));
				return null;
			}
		}
		public void DeleteShopifyProduct(string shopifyId)
		{
			shopifyAPIClient.Delete (String.Format("/admin/products/{0}.json",shopifyId));
		}
		public async Task<ProductData> UpdateShopifyProduct(ProductData updatedPD)
		{
			var pdWithVarInfo = await GetShopifySkinnyProduct (updatedPD.id);

			updatedPD.ShopifyVariantId = pdWithVarInfo.ShopifyVariantId;
			object json = await shopifyAPIClient.Put(String.Format("/admin/products/{0}.json",updatedPD.id),updatedPD.GetShopifyProductPutJSON());
			if (json == null)
				return null;
			ProductData pd = JsonConvert.DeserializeObject<GetProductReply>(json.ToString()).product;
			return pd;
		}
		//        public bool UpdateShopifyProductFulfillment(string id, string fulfillment)
		//        {
		//            try
		//            {
		//                string data = new ProductData().GetUpdateFulfillmentShopifPutJSON(fulfillment);
		//                //byte[] dataStream = Encoding.UTF8.GetBytes(data);
		//                string json = shopifyAPIClient.Put(String.Format("/admin/products/#{0}.json", id), data).ToString();               
		//
		//             
		//            }
		//            catch (Exception e)
		//            {
		//				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));
		//
		//                return false;
		//            }
		//            return true;
		//        }
		//
		//        public bool UpdateShopifyProductScope(string id, string scope)
		//        {
		//            try
		//            {
		//
		//                string data = new ProductData().GetUpdateScopeShopifPutJSON(scope);
		//                //byte[] dataStream = Encoding.UTF8.GetBytes(data);
		//                string json = shopifyAPIClient.Put(String.Format("/admin/products/{0}.json", id), data).ToString();
		//
		//            }
		//            catch (Exception e)
		//            {
		//				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));
		//
		//                return false;
		//            }
		//            return true;
		//        }
		//
		//
		//        public List<Order> GetOpenOrders(string productName = null)
		//        {
		//            try
		//            {
		//                string json = shopifyAPIClient.Get(String.Format("/admin/orders.json?financial_status=paid&status=open?fields=id,name,fulfillment_status,created_at,line_items,customer")).ToString();
		//                GetOrderReply orderReply = JsonConvert.DeserializeObject<GetOrderReply>(json);
		//                return orderReply.orders;
		//            }
		//           
		//            catch (Exception e)
		//            {
		//				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));
		//
		//                return null;
		//            }
		//
		//        }
		//
		//        public List<Order> GetOpenOrdersWithoutFulfilment(string productName = null)
		//        {
		//            try
		//            {
		//                string json = shopifyAPIClient.Get(String.Format("admin/orders.json?fulfillment_status=unfulfilled&fields=id,name,fulfillment_status,created_at,line_items")).ToString();
		//                GetOrderReply orderReply = JsonConvert.DeserializeObject<GetOrderReply>(json);
		//                return orderReply.orders;
		//            }
		//            
		//            catch (Exception e)
		//            {
		//				Console.WriteLine(String.Format("\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None"));
		//
		//                return null;
		//            }
		//
		//        }
		//
		//        public List<string> GetEmailsFromOpenOrdersWithoutFulfillment(string prodcutName = null)
		//        {
		//            var openOrderIds = GetOpenOrdersWithoutFulfilment(prodcutName).Select(x => x.id).ToList();
		//
		//            return GetEmailsFromOrders(openOrderIds);
		//        }
		//
		//        public List<string> GetEmailsFromOrders(List<string> orderIds)
		//        {
		//
		//            List<string> emails = new List<string>();
		//            foreach (string id in orderIds)
		//            {
		//                emails.Add(GetEmailFromOrder(id));
		//            }
		//
		//            return emails;
		//        }
		//
		//
		//        public string GetEmailFromOrder(string orderId)
		//        {
		//
		//            try
		//            {
		//                string json = shopifyAPIClient.Get(String.Format("admin/orders.json?fields=customer")).ToString();
		//                Order orderReply = JsonConvert.DeserializeObject<Order>(json);
		//                return orderReply.customer.email;
		//            }
		//            catch (Exception e)
		//            {
		//				Console.WriteLine(String.Format("\nId:{2}\nException: {0}\nInner Exception: {1}", e.Message, e.InnerException != null ? e.InnerException.Message : "None",orderId));
		//
		//                return null;
		//            }
		//        }
		#endregion






		#region utility
		public NetworkCredential GetShopifyCredential(string url)
		{
			return new NetworkCredential(ShopifyAPIKey, ShopifyPassword);
		}
		#endregion

	}


}