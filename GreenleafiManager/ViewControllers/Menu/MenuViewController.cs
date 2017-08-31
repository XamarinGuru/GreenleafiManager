using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace GreenleafiManager
{
	public partial class MenuViewController : UIViewController
	{
		public static LoadingOverlay loadingOverlay;

		private AlertViewController ReportSelectionAlert;
		const string Identifier = "com.rcubedstudios.greenleaf";
		public NSUrlSessionDownloadTask downloadTask;
		public NSUrlSession session;
		public bool forcingBackgroundDownload = false;
		public ArrayList taskArray = new ArrayList();

		UIButton btnReport, btnCustomer, btnUser, btnInvoice, btnLocation, btnInvertory, btnSoldItems, btnPurge, btnRefresh, btnDownloadImages, btnResetAll;
		public MenuViewController(IntPtr handle) : base(handle)
		{
		}

		void InitializeComponent()
		{
			nfloat initialX = (nfloat)((View.Frame.Width / 2) - (1.5 * DefaultMenuLayoutSettings.DefaultLabelWidth + DefaultMenuLayoutSettings.DefaultHorizontalSpacing));
			nfloat initialY = 200f;
			btnReport = new UIButton(new CGRect(initialX, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnReport.SetTitle("Report", UIControlState.Normal);
			btnReport.ToGreenLeafButton();
			View.Add(btnReport);
			btnReport.TouchUpInside += (sender, e) =>
			{
				View.AddSubview(ReportSelectionAlert);
			};

			btnCustomer = new UIButton(new CGRect(initialX + DefaultMenuLayoutSettings.DefaultLabelWidth + DefaultMenuLayoutSettings.DefaultHorizontalSpacing, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnCustomer.SetTitle("Customers", UIControlState.Normal);
			btnCustomer.ToGreenLeafButton();
			View.Add(btnCustomer);
			btnCustomer.TouchUpInside += (sender, e) =>
			{
				MasterCustomerViewController mcvc = this.Storyboard.InstantiateViewController("MasterCustomerViewController") as MasterCustomerViewController;
				this.NavigationController.PushViewController(mcvc, true);
			};

			btnInvertory = new UIButton(new CGRect(btnCustomer.Frame.X + DefaultMenuLayoutSettings.DefaultLabelWidth + DefaultMenuLayoutSettings.DefaultHorizontalSpacing, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnInvertory.SetTitle("Inventory", UIControlState.Normal);
			btnInvertory.ToGreenLeafButton();
			View.Add(btnInvertory);
			btnInvertory.TouchUpInside += (sender, e) =>
			{
				MasterInventoryViewController mivc = this.Storyboard.InstantiateViewController("MasterInventoryViewController") as MasterInventoryViewController;
				this.NavigationController.PushViewController(mivc, true);
			};

			initialY += DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;

			btnUser = new UIButton(new CGRect(initialX, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnUser.SetTitle("Users", UIControlState.Normal);
			btnUser.ToGreenLeafButton();
			View.Add(btnUser);
			btnUser.TouchUpInside += (sender, e) =>
			{
				MasterUserViewController muvc = this.Storyboard.InstantiateViewController("MasterUserViewController") as MasterUserViewController;
				this.NavigationController.PushViewController(muvc, true);
			};

			btnLocation = new UIButton(new CGRect(btnCustomer.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnLocation.SetTitle("Locations", UIControlState.Normal);
			btnLocation.ToGreenLeafButton();
			View.Add(btnLocation);
			btnLocation.TouchUpInside += (sender, e) =>
			{
				MasterLocationViewController mlvc = this.Storyboard.InstantiateViewController("MasterLocationViewController") as MasterLocationViewController;
				this.NavigationController.PushViewController(mlvc, true);
			};

			btnInvoice = new UIButton(new CGRect(btnInvertory.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnInvoice.SetTitle("Invoices", UIControlState.Normal);
			btnInvoice.ToGreenLeafButton();
			View.Add(btnInvoice);
			btnInvoice.TouchUpInside += (sender, e) =>
			{
				MasterInvoiceViewController mivc = this.Storyboard.InstantiateViewController("MasterInvoiceViewController") as MasterInvoiceViewController;
				this.NavigationController.PushViewController(mivc, true);
			};

			initialY += DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;


			btnRefresh = new UIButton(new CGRect(btnCustomer.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnRefresh.SetTitle("Refresh", UIControlState.Normal);
			btnRefresh.ToGreenLeafButton();
			View.Add(btnRefresh);
			btnRefresh.TouchUpInside += async (sender, e) =>
		   {

			   ShowLoading();
			   await UpdateDataTask();
			   HideLoading();
		   };


			btnSoldItems = new UIButton(new CGRect(btnRefresh.Frame.X - DefaultMenuLayoutSettings.DefaultLabelWidth - DefaultMenuLayoutSettings.DefaultHorizontalSpacing, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnSoldItems.SetTitle("Sold Items", UIControlState.Normal);
			btnSoldItems.ToGreenLeafButton();
			View.Add(btnSoldItems);
			btnSoldItems.TouchUpInside += (sender, e) =>
			{
				MasterInventoryViewController mivc = this.Storyboard.InstantiateViewController("MasterInventoryViewController") as MasterInventoryViewController;
				mivc.ShowOnlySoldItems = true;
				this.NavigationController.PushViewController(mivc, true);
			};

			btnDownloadImages = new UIButton(new CGRect(btnRefresh.Frame.X + DefaultMenuLayoutSettings.DefaultLabelWidth + DefaultMenuLayoutSettings.DefaultHorizontalSpacing, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnDownloadImages.SetTitle("Download Images", UIControlState.Normal);
			btnDownloadImages.ToGreenLeafButton();
			View.Add(btnDownloadImages);
			btnDownloadImages.TouchUpInside += async (sender, e) =>
			  	{
					  await UpdateImages(true);
				  };

			initialY = btnRefresh.Frame.Y + DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;

			btnPurge = new UIButton(new CGRect(btnUser.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnPurge.SetTitle("Purge", UIControlState.Normal);
			btnPurge.ToGreenLeafButton();
			View.Add(btnPurge);
			btnPurge.TouchUpInside += (sender, e) =>
			{
				UIAlertController alert = UIAlertController.Create("Purge will delete all local data and images. Continue?", "", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
					{
					}));
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, async (actionOK) =>
				  {
					  ShowLoading();
					  await PurgeData();
					  HideLoading();
				  }));
				PresentViewController(alert, true, null);
			};

			btnResetAll = new UIButton(new CGRect(btnLocation.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnResetAll.SetTitle("Reset", UIControlState.Normal);
			btnResetAll.ToGreenLeafButton();
			View.Add(btnResetAll);
			btnResetAll.TouchUpInside += (sender, e) =>
			{
				UIAlertController alert = UIAlertController.Create("Reset will remove all local data and pull all new data from the server. This can take over an hour depending on internet speed."
																				   + " Please make sure you iPad is plugged in before starting.", "", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
				{
				}));
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, async (actionOK) =>
				{
					this.NavigationItem.HidesBackButton = true;
					var bounds = UIScreen.MainScreen.Bounds;
					loadingOverlay = new LoadingOverlay(bounds, "Reseting...");
					View.Add(loadingOverlay);
					await PurgeData();
					await UpdateDataTask();
					loadingOverlay.Hide();
                    this.forcingBackgroundDownload = true;
					await UpdateImages(true);
				}));
				PresentViewController(alert, true, null);
			};


			UITextField versionField = new UITextField(new CGRect(btnResetAll.Frame.X + DefaultMenuLayoutSettings.DefaultLabelWidth, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			versionField.Text = "Version 0.13.1";
			versionField.TextAlignment = UITextAlignment.Center;
			View.Add(versionField);

            initialY = btnPurge.Frame.Y + DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;

			var bgImage = new UIImageView(new CGRect(0, initialY, View.Frame.Width, View.Frame.Height - initialY));
			bgImage.Image = UIImage.FromFile("bghome.png");
			View.Add(bgImage);

		}
		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();
			if (session == null)
				session = InitBackgroundSession();

			this.NavigationController.SetNavigationBarHidden(false, false);
			InitializeComponent();
			ShowLoading();
			ReportSelectionAlert = new AlertViewController(View.Bounds.Size.Width, View.Bounds.Height, "", this);
			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;


			if (!AzureService.DefaultService.InitialLoadComplete)
			{
				await AzureService.DefaultService.InitializeStoreAsync();//RMR this must be done first
				if (AzureService.InventoryService.Items == null || AzureService.InventoryService.Items.Count == 0)
				{
					//Update from local now that they are setup
					await AzureService.CustomerService.UpdateCustomersFromLocalDB();
					await AzureService.UserService.UpdateUsersFromLocalDB();
					await AzureService.LocationService.UpdateLocationsFromLocalDB();
					await AzureService.InventoryService.UpdateItemsFromLocalDB();
					await AzureService.InvoiceService.UpdateInvoicesFromLocalDB();

					if (AzureService.InventoryService.Items == null || AzureService.InventoryService.Items.Count == 0)
					{
						//If it is still empty then warn and do a full reset;
						var okCancelAlertController = UIAlertController.Create("No local items found", "Starting data update.", UIAlertControllerStyle.Alert);
						//Add Actions
						okCancelAlertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
						PresentViewController(okCancelAlertController, animated: true, completionHandler: null);

						await UpdateDataTask();
					}
				}

			}
			else if (AzureService.InventoryService.Items == null || AzureService.InventoryService.Items.Count == 0)
			{
				var okCancelAlertController = UIAlertController.Create("No local items found", "Starting data update.", UIAlertControllerStyle.Alert);
				//Add Actions
				okCancelAlertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
				PresentViewController(okCancelAlertController, animated: true, completionHandler: null);
				await UpdateDataTask();
			}
			HideLoading();
		}

		//Background Task
		public NSUrlSession InitBackgroundSession()
		{
			Console.WriteLine("InitBackgroundSession");
			using (var configuration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(Identifier))
			{
				return NSUrlSession.FromConfiguration(configuration, new UrlSessionDelegate(this), null);
			}
		}

		public override Task DismissViewControllerAsync(bool animated)
		{
			return base.DismissViewControllerAsync(animated);
		}

		public override void ViewDidLayoutSubviews()
		{
			ReportSelectionAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
		}

		async Task PurgeData()
		{
			await AzureService.DefaultService.PurgeAllTables();
		}
		async Task UpdateDataTask()
		{
			await AzureService.LocationService.UpdateLocationsFromAzure();
			await AzureService.UserService.UpdateUsersFromAzure();
			await AzureService.CustomerService.UpdateCustomersFromAzure();
			await AzureService.InventoryService.UpdateItemsFromAzure();
			await AzureService.InvoiceService.UpdateInvoicesFromAzure();
		}

        //Downloading Images in Background Session
        async Task UpdateImages(bool image)
        {
            ShowDownloadingImages();

            using (WebClient client = new WebClient())
            {
                var imageBytes = await client.DownloadStringTaskAsync(new Uri("http://greenleafmobileservice.azurewebsites.net/api/images/"));
                List<ItemsPictures> downloadedUrls = AzureService.InventoryService.GetImagesFromAzure();// JsonConvert.DeserializeObject<List<ImageData>>(imageBytes);

                //for (int i = 0; i < 5; i++)
                foreach (var item in downloadedUrls)
                {
                    //var item = downloadedUrls[i];
                    using (var request = NSUrlRequest.FromUrl(NSUrl.FromString(item?.PictureURL)))
                    {
                        downloadTask = session.CreateDownloadTask(request);
                        downloadTask.Resume();
                        taskArray?.Add(downloadTask?.TaskIdentifier);

                        await UpdateDownloadingImages(taskArray.Count, downloadedUrls.Count());
                    }
                }
            }

            HideDownloadImages();
        }

        //async Task UpdateImages(bool image)
        //{
        //    ShowDownloadingImages();

        //    WebClient client = new WebClient();
        //    var imageBytes = await client.DownloadStringTaskAsync(new Uri("http://greenleafmobileservice.azurewebsites.net/api/images/"));
        //    List<ItemsPictures> downloadedUrls = AzureService.InventoryService.GetImagesFromAzure();// JsonConvert.DeserializeObject<List<ImageData>>(imageBytes);

        //    //await UpdateDownloadingImages(11, downloadedUrls.Count());
        //    //for (int i = 0; i < 5; i++)
        //    foreach (var item in downloadedUrls)
        //    {
        //        //var item = downloadedUrls[i];
        //        var request = NSUrlRequest.FromUrl(NSUrl.FromString(item?.PictureURL));

        //        downloadTask = session.CreateDownloadTask(request);
        //        downloadTask.Resume();
        //        taskArray?.Add(downloadTask?.TaskIdentifier);

        //        await UpdateDownloadingImages(taskArray.Count, downloadedUrls.Count());
        //    }

        //    HideDownloadImages();
        //}


		partial void InventoryButton_TouchUpInside(UIButton sender)
		{
			MasterInventoryViewController mivc = this.Storyboard.InstantiateViewController("MasterInventoryViewController") as MasterInventoryViewController;
			this.NavigationController.PushViewController(mivc, true);
		}

		partial void InvoicesButton_TouchUpInside(UIButton sender)
		{
			MasterInvoiceViewController mivc = this.Storyboard.InstantiateViewController("MasterInvoiceViewController") as MasterInvoiceViewController;
			this.NavigationController.PushViewController(mivc, true);
		}

		partial void LocationsButton_TouchUpInside(UIButton sender)
		{
			MasterLocationViewController mlvc = this.Storyboard.InstantiateViewController("MasterLocationViewController") as MasterLocationViewController;
			this.NavigationController.PushViewController(mlvc, true);
		}

		partial void CustomersButton_TouchUpInside(UIButton sender)
		{
			MasterCustomerViewController mcvc = this.Storyboard.InstantiateViewController("MasterCustomerViewController") as MasterCustomerViewController;
			this.NavigationController.PushViewController(mcvc, true);
		}

		partial void Users_TouchUpInside(UIButton sender)
		{
			MasterUserViewController muvc = this.Storyboard.InstantiateViewController("MasterUserViewController") as MasterUserViewController;
			this.NavigationController.PushViewController(muvc, true);
		}

		partial void Reports_TouchUpInside(UIButton sender)
		{

			View.AddSubview(ReportSelectionAlert);
		}

		public void ShowLoading(string message = null)
		{
			this.NavigationItem.HidesBackButton = true;
			var bounds = UIScreen.MainScreen.Bounds;
			if (message == null)
				loadingOverlay = new LoadingOverlay(bounds);
			else
				loadingOverlay = new LoadingOverlay(bounds, message);
			View.Add(loadingOverlay);
		}

		public void ShowDownloadingImages(string message = null)
		{
			this.NavigationItem.HidesBackButton = true;
			var bounds = UIScreen.MainScreen.Bounds;
			if (message == null)
				loadingOverlay = new LoadingOverlay(bounds, "Queuing images to download in the background");
			else
				loadingOverlay = new LoadingOverlay(bounds, message);
			View.Add(loadingOverlay);
		}
		public async Task UpdateDownloadingImages(int numCurrentImages, int numTotalImages)
		{
			await Task.Run(() =>
			{
				InvokeOnMainThread(() =>
				{
					loadingOverlay.loadingLabel.Text = "Queuing images to download in the background (" + numCurrentImages + "/" + numTotalImages + ")";
				});
			});
		}
		public void HideDownloadImages()
		{
			loadingOverlay.Hide();
			this.NavigationItem.HidesBackButton = false;
		}
		public void HideLoading()
		{
			loadingOverlay.Hide();
			this.NavigationItem.HidesBackButton = false;
		}
	}

	//Handler
	public class UrlSessionDelegate : NSUrlSessionDownloadDelegate
	{
		public MenuViewController controller;

		public UrlSessionDelegate(MenuViewController controller)
		{
			this.controller = controller;
		}

		public override void DidWriteData(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
		{
			if (downloadTask == controller.downloadTask)
			{
				//float progress = totalBytesWritten / (float)totalBytesExpectedToWrite;
				Console.WriteLine(string.Format("DownloadTask: {0} ", downloadTask));
			}
		}

		public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
		{
			try
			{
				Console.WriteLine("Finished");
				Console.WriteLine("File downloaded in : {0}", location);
				NSFileManager fileManager = NSFileManager.DefaultManager;

				var URLs = fileManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
				NSUrl documentsDictionry = URLs[0];

				NSUrl originalURL = downloadTask.OriginalRequest.Url;
				//NSUrl destinationURL = documentsDictionry.Append(originalURL.LastPathComponent, false);

				string shopifyItemId = originalURL.LastPathComponent.Split('_').LastOrDefault();
				if (shopifyItemId == null)
					return;
				string shopifyImageId = originalURL.LastPathComponent.Split('_').FirstOrDefault();
				if (shopifyImageId == null)
					return;

				var inventoryItem = AzureService.InventoryService.GetItemByShopifyId(shopifyItemId.Replace(".png", ""));
				if (inventoryItem == null)
				{
					Console.WriteLine("Item for shopify ID {0} not found.", shopifyItemId.Replace(".png", ""));
				}
				else
				{
					var destinationURL = AzureService.InventoryService.GetImageSaveLocationWithName(inventoryItem, shopifyImageId);

					NSError removeCopy;

					fileManager.Remove(destinationURL, out removeCopy);

					NSError errorCopy;

					bool success = fileManager.Copy(location, destinationURL, out errorCopy);

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
				}
				controller.taskArray.Remove(downloadTask.TaskIdentifier);
				if (controller.taskArray.Count == 5)
				{
					controller.InvokeOnMainThread(() =>
					{
						if (controller.forcingBackgroundDownload)
						{
							controller.HideLoading();
							controller.forcingBackgroundDownload = false;
							MasterInventoryViewController mivc = controller.Storyboard.InstantiateViewController("MasterInventoryViewController") as MasterInventoryViewController;
							controller.NavigationController.PushViewController(mivc, true);
						}
						else
							controller.HideLoading();
					});
				}

			}
			catch (Exception ex)
			{

			}
		}

		public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
		{
			Console.WriteLine("DidComplete");
			if (error == null)
				Console.WriteLine("Task: {0} completed successfully", task);
			else
				Console.WriteLine("Task: {0} completed with error: {1}", task, error.LocalizedDescription);
			controller.downloadTask = null;
		}

		public override void DidResume(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long resumeFileOffset, long expectedTotalBytes)
		{
			Console.WriteLine("DidResume");
		}

		public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
		{
			using (AppDelegate appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate)
			{
				var handler = appDelegate.BackgroundSessionCompletionHandler;
				if (handler != null)
				{
					appDelegate.BackgroundSessionCompletionHandler = null;
					handler();
				}
			}
			controller.InvokeOnMainThread(() =>
			{
				controller.HideLoading();
				//Notification For Image
				var notification = new UILocalNotification();
				// configure the alert
				notification.FireDate = DateTime.Now.ToNSDate();
				notification.AlertAction = "Images Sync";
				notification.AlertBody = "All product images has been sync.";
				notification.SoundName = UILocalNotification.DefaultSoundName;
				UIApplication.SharedApplication.ScheduleLocalNotification(notification);
			});

			Console.WriteLine("All tasks are finished");
		}
	}
}
