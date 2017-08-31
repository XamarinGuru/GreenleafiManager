using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using UIKit;

namespace GreenleafiManagerPhone
{
	public partial class LoadingViewController : UIViewController
	{

		InventoryDetailViewController idvc;
		public bool IsNew
		{
			get;
			set;
		}
		public string sku
		{
			get;
			set;
		}
		public bool AutoPop { get; set; }
		public LoadingViewController() : base("LoadingViewController", null)
		{
		}
		public LoadingViewController(IntPtr handle) : base(handle)
		{
			var bounds = UIScreen.MainScreen.Bounds;
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavigationItem.HidesBackButton = true;

			// Perform any additional setup after loading the view, typically from a nib.
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			NavigationItem.HidesBackButton = true;

			if (!AutoPop)
			{
				idvc = this.Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
				if (IsNew)
				{
					CreateNew();
					GoToItemDetails();
					AutoPop = true;
				}
				else
				{
					if (LoadItem())
					{
						GoToItemDetails();
						AutoPop = true;
					}
					else
					{
						AutoPop = false;
						NavigationController.PopViewController(true);
					}
				}
			}
			else
			{
				AutoPop = false;
				NavigationController.PopViewController(true);
			}
		}
		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
		public void GoToItemDetails()
		{
			this.NavigationController.PushViewController(idvc, true);
		}
		private void CreateNew()
		{
			idvc.InventoryItem = new InventoryNS();
			idvc.InventoryItem.Sku = String.IsNullOrWhiteSpace(sku) ? AzureService.InventoryService.GetNextSku() : sku;
		}
		private bool LoadItem()
		{
			try
			{
				if (AzureService.InventoryService != null)
				{
					var item = new Item();

					if (!Reachability.IsHostReachable("http://google.com"))
					{
						NavigationController.PopViewController(true);
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
							sku = Int32.Parse(sku).ToString();
							var response = Client.GetAsync(String.Format("tables/Reports/GetItemBySku?Sku={0}", sku)).Result;

							if (!response.IsSuccessStatusCode)
							{
								var alert = UIAlertController.Create("Could Not Find Item", String.Format("No item with SKU {0} exist. Would you like to make one?", sku), UIAlertControllerStyle.Alert);
					
								alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (UIAlertAction obj) => NavigationController.PopViewController(true)));
								alert.AddAction(UIAlertAction.Create("Create New", UIAlertActionStyle.Default, (UIAlertAction obj) => 
								{
									CreateNew();
									GoToItemDetails();
									AutoPop = true;

								}));

								PresentViewController(alert, animated: true, completionHandler: null);


								return false;
							}
							else
							{
								var json = response.Content.ReadAsStringAsync().Result;
								item = JsonConvert.DeserializeObject<Item>(json);

							}
						}
					}
					if (item != null && item.Sku == sku)
					{
						idvc.IsNew = false;
						idvc.InventoryItem = new InventoryNS(item);

					}
					else
					{
						var alert = UIAlertController.Create("Could Not Find Item", String.Format("No item with SKU {0} exist. Would you like to make one?", sku), UIAlertControllerStyle.Alert);

						alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (UIAlertAction obj) => NavigationController.PopViewController(true)));
						alert.AddAction(UIAlertAction.Create("Create New", UIAlertActionStyle.Default, (UIAlertAction obj) =>
						{
							CreateNew();
							GoToItemDetails();
							AutoPop = true;

						}));

						PresentViewController(alert, animated: true, completionHandler: null);


						return false;
					}
				}
				return true;

			}
			catch (Exception ex)
			{
				return false;
			}
			finally
			{
				//HideLoading();
			}
		}
	}
}

