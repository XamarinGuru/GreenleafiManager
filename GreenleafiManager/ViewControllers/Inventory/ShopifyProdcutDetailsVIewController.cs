// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace GreenleafiManager
{
public partial class ShopifyProdcutDetailsVIewController : UIViewController
{

	public static SavingOverlay savingOverlay;
	public InventoryNS InventoryItem
	{
		get;
		set;
	}
	public static LoadingOverlay loadingOverlay;

public ShopifyProdcutDetailsVIewController(IntPtr handle) : base(handle)
	{
		var bounds = UIScreen.MainScreen.Bounds;

		loadingOverlay = new LoadingOverlay(bounds);


			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, SaveShopifyInfoToLocalItem);
		doneButton.AccessibilityLabel = "doneButton";
		NavigationItem.RightBarButtonItem = doneButton;

	}

	public ShopifyProdcutDetailsVIewController() : base("ShopifyDetailsViewController", null)
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
	void SaveShopifyInfoToLocalItem(object sender, EventArgs args)
	{
		performSave();
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
