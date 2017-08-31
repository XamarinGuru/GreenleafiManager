using Foundation;
using System;
using System.Linq;
using System.CodeDom.Compiler;
using UIKit;
using Infragistics;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;
using Syncfusion.SfDataGrid;
using CoreGraphics;
using System.Windows.Input;
using System.Collections;

namespace GreenleafiManagerPhone
{
    public partial class ShopifyProdcutDetailsVIewController : UIViewController
    {
        UIButton btnResetShopifyData; 
		public static SavingOverlay savingOverlay;
		public InventoryNS InventoryItem
		{
			get;
			set;
		}
		public static LoadingOverlay loadingOverlay;

		public ShopifyProdcutDetailsVIewController (IntPtr handle) : base (handle)
        {
			var bounds = UIScreen.MainScreen.Bounds;

			loadingOverlay = new LoadingOverlay(bounds);


			var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveItem);
			saveButton.AccessibilityLabel = "saveButton";
			NavigationItem.RightBarButtonItem = saveButton;

        }

		public ShopifyProdcutDetailsVIewController() : base("ShopifyProdcutDetailsVIewController", null)
		{
			if (InventoryItem == null)
				throw new Exception("Item Required for images");
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//_slideView = new UIView(new CoreGraphics.CGRect(0, 0, 100, 100));

			var btn2 = new UIBarButtonItem("  Cancel", UIBarButtonItemStyle.Plain, (sender, e) => { this.NavigationController.PopViewController(true); });
			this.NavigationItem.LeftBarButtonItem = btn2;
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			ConfigureView();

		}

		public void ConfigureView()
		{
			if (InventoryItem == null)
			{
				return;
			}

			ShopifyDescriptionTextView.Text = InventoryItem.ShopifyDescription;
			ShopifyTitleTextField.Text = InventoryItem.ShopifyTitle;
			ShopifyPriceTextField.Text = InventoryItem.ShopifyPrice.ToString();

			ShopifyDescriptionTextView.Layer.BorderColor = UIColor.Black.CGColor;
			ShopifyDescriptionTextView.Layer.BorderWidth = 1f;
			ShopifyDescriptionTextView.ScrollsToTop = true;

		}
		void SaveItem(object sender, EventArgs args)
		{
			performSave();
		}
        partial void ResetShopifyDataTouchUp(Foundation.NSObject sender)
        {
            InventoryItem.ShopifyTitle = "";
            InventoryItem.ShopifyDescription = "";
            InventoryItem.ShopifyPrice = 0;
            ConfigureView();
        }
		private bool performSave()
		{
			ShowSavingOverlay();
			if (this.NavigationItem.LeftBarButtonItem != null)
				this.NavigationItem.LeftBarButtonItem.Enabled = false;
			else
				this.NavigationItem.HidesBackButton = true;


			this.NavigationItem.RightBarButtonItem.Enabled = false;

			InventoryItem.ShopifyTitle = ShopifyTitleTextField.Text;

			double price = 0; 
			Double.TryParse(ShopifyPriceTextField.Text, out price);
			if (price > 0)
			{
				InventoryItem.ShopifyPrice = price;
			}

			InventoryItem.ShopifyDescription = ShopifyDescriptionTextView.Text;


			var item = InventoryItem.ConvertToInventory();
			item.LastCheckedDate = DateTime.UtcNow;
			InventoryItem.LastCheckedDate = DateTime.Now.ToString("d");

			var savedItem = AzureService.InventoryService.InsertOrSave(item);
			if (savedItem == null)
				return false;
			InventoryItem = new InventoryNS(savedItem);
		

			this.NavigationItem.RightBarButtonItem.Enabled = true;
			if (this.NavigationItem.LeftBarButtonItem != null)
				this.NavigationItem.LeftBarButtonItem.Enabled = true;
			else
				this.NavigationItem.HidesBackButton = false;

			HideSavingOverlay();
			this.NavigationController.PopViewController(true);

			return true;
		}

		private void ShowLoading()
		{
			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
			View.Add(loadingOverlay);
		}

		private void HideLoading()
		{
			loadingOverlay.Hide();
		}

			public void ShowSavingOverlay()
		{
			var bounds = UIScreen.MainScreen.Bounds;
			savingOverlay = new SavingOverlay(bounds);
			View.Add(savingOverlay);
		}

		public void HideSavingOverlay()
		{
			savingOverlay.Hide();
		}
    }
}