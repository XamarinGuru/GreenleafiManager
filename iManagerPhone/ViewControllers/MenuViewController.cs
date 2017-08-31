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
using System.Net.Http;
using System.Net.Http.Headers;

namespace GreenleafiManagerPhone
{
	public partial class MenuViewController : UIViewController
	{
		public static LoadingOverlay loadingOverlay;
		public static LoadingOverlay loadingItemOverlay;

		LoadingViewController lvc; 
		InventoryDetailViewController idvc;

		const string Identifier = "com.rcubedstudios.greenleaf";
		public NSUrlSessionDownloadTask downloadTask;
		public NSUrlSession session;
		UIAlertController alertWithTextBox;

		UIButton btnNewItem, btnFindBySku, btnScanSku, btnUpdateCodes;
		public MenuViewController(IntPtr handle) : base(handle)
		{
			//var updateButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, UpdateData);
			//updateButton.AccessibilityLabel = "updateButton";
			//NavigationItem.RightBarButtonItem = updateButton;
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();


			this.NavigationController.SetNavigationBarHidden(false, false);
			InitializeComponent();
			ShowLoading();
			idvc = this.Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
			lvc = this.Storyboard.InstantiateViewController("LoadingViewController") as LoadingViewController;

			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;


			if (!AzureService.DefaultService.InitialLoadComplete)
			{
				await AzureService.DefaultService.InitializeStoreAsync();//RMR this must be done first
				if (AzureService.InventoryService.MetalCodes == null || AzureService.InventoryService.MetalCodes.Count == 0)
				{
					//Update from local now that they are setup

					await AzureService.DefaultService.SetupConfirmationWithSyncForAllTables();

					if (AzureService.InventoryService.MetalCodes == null || AzureService.InventoryService.MetalCodes.Count == 0)
					{

						await UpdateDataTask();
						//If it is still empty then warn and do a full reset;
						var okCancelAlertController = UIAlertController.Create("No local code data found", "Starting data update.", UIAlertControllerStyle.Alert);
						//Add Actions
						okCancelAlertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
						PresentViewController(okCancelAlertController, animated: true, completionHandler: null);

					}
				}

			}
			else if (AzureService.InventoryService.MetalCodes == null || AzureService.InventoryService.MetalCodes.Count == 0)
			{
				var okCancelAlertController = UIAlertController.Create("No local code data found", "Starting data update.", UIAlertControllerStyle.Alert);
				//Add Actions
				okCancelAlertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
				PresentViewController(okCancelAlertController, animated: true, completionHandler: null);
				await UpdateDataTask();
			}
			HideLoading();
		}



		public override Task DismissViewControllerAsync(bool animated)
		{
			return base.DismissViewControllerAsync(animated);
		}

		void InitializeComponent()
		{
			nfloat initialX = (nfloat)((View.Frame.Width / 2) - (DefaultMenuLayoutSettings.DefaultLabelWidth / 2));
			nfloat initialY = 200f;


			btnNewItem = new UIButton(new CGRect(initialX, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnNewItem.SetTitle("New Item", UIControlState.Normal);
			btnNewItem.ToGreenLeafButton();
			View.Add(btnNewItem);
			btnNewItem.TouchUpInside += (sender, e) =>
			{
				lvc.IsNew = true;
				lvc.sku = "";

				this.NavigationController.PushViewController(lvc, true);

			};

			initialY += DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;



			btnFindBySku = new UIButton(new CGRect(btnNewItem.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnFindBySku.SetTitle("Find By SKU", UIControlState.Normal);
			btnFindBySku.ToGreenLeafButton();
			View.Add(btnFindBySku);
			btnFindBySku.TouchUpInside += FindItemBySku_TouchUpInside;

			initialY += DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;

			btnScanSku = new UIButton(new CGRect(btnFindBySku.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnScanSku.SetTitle("Scan SKU", UIControlState.Normal);
			btnScanSku.ToGreenLeafButton();
			View.Add(btnScanSku);
			btnScanSku.TouchUpInside += (sender, e) =>
			{
				Scan();
			};
	
            initialY += DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;

			btnUpdateCodes = new UIButton(new CGRect(btnScanSku.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
			btnUpdateCodes.SetTitle("Update Codes", UIControlState.Normal);
			btnUpdateCodes.ToGreenLeafButton();
			View.Add(btnUpdateCodes);
			btnUpdateCodes.TouchUpInside += async (sender, e) =>
			{
                ShowLoading();
                await AzureService.DefaultService.PushAsync();
				await UpdateDataTask();
			    HideLoading();
			};

			initialY += DefaultMenuLayoutSettings.DefaultVerticalSpacing + DefaultMenuLayoutSettings.DefaultLabelHeight;

            UITextField versionField = new UITextField(new CGRect(btnScanSku.Frame.X, initialY, DefaultMenuLayoutSettings.DefaultLabelWidth, DefaultMenuLayoutSettings.DefaultLabelHeight));
            versionField.Text = "Version 1.3.2";
            versionField.TextAlignment = UITextAlignment.Center;
			View.Add(versionField);
		}

		async Task UpdateDataTask()
		{
            await AzureService.DefaultService.PurgeAllTables();
			await AzureService.LocationService.UpdateLocationsFromAzure();
			await AzureService.InventoryService.PullAllCodes();
		}

		void GetItemAndOpenDetails(string sku = null)
		{
			
			if (alertWithTextBox != null || sku != null)
			{
				if (sku == null)
					sku = alertWithTextBox.TextFields[0].Text.Trim();
				

				lvc.IsNew = false;
				lvc.sku = sku;
				this.NavigationController.PushViewController(lvc, true);
			}


		}
		//Scanning
		public async void Scan()
		{
			//var itemsWithImages = AzureService.InventoryService.Items.Where(x => x.Images != null && x.Images.Count > 0).ToList();
			var scanner = new ZXing.Mobile.MobileBarcodeScanner();
			var result = await scanner.Scan();

			if (result != null)
			{
				GetItemAndOpenDetails(result.Text);
			}

		}
		private void FindItemBySku_TouchUpInside(object sender, EventArgs e)
		{
			alertWithTextBox = UIAlertController.Create("Edit Item", "Enter SKU Number", UIAlertControllerStyle.Alert);
			alertWithTextBox.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
			alertWithTextBox.AddAction(UIAlertAction.Create("Open Item", UIAlertActionStyle.Default, (UIAlertAction obj) => GetItemAndOpenDetails()));
			alertWithTextBox.AddTextField((field) =>
			{
				field.Placeholder = "SKU";
				field.ToNumeric();
				field.EditingChanged += (sender3, e3) => {
					if (field.Text.Length == 6)
					{
						DismissViewController(true, null);
						GetItemAndOpenDetails();
					}
				};
			});
			PresentViewController(alertWithTextBox, animated: true, completionHandler: null);
		}
		private void ShowLoading()
		{
			this.NavigationItem.HidesBackButton = true;
			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
			View.Add(loadingOverlay);
		}
		private void ShowLoadingItem()
		{
			this.NavigationItem.HidesBackButton = true;
			var bounds = UIScreen.MainScreen.Bounds;
			loadingItemOverlay = new LoadingOverlay(bounds);
			View.Add(loadingItemOverlay);
		}
		private void HideLoading()
		{
			loadingOverlay.Hide();
			this.NavigationItem.HidesBackButton = false;
		}
		private void HideLoadingItem()
		{
			loadingOverlay.RemoveFromSuperview();
			this.NavigationItem.HidesBackButton = false;
		}

	}
}
