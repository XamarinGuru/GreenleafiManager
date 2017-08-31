using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Drawing;

using Infragistics;
using Syncfusion.SfBarcode.iOS;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf;

using System.Linq;
using System.Collections.Generic;
using System.Threading;
using CoreGraphics;
using System.Threading.Tasks;
using EventKitUI;
using GreenleafiManager.ViewControllers;
using MediaPlayer;
using ObjCRuntime;
using System.IO;
using QuickLook;
//using SampleBrowser;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
namespace GreenleafiManager
{
	public partial class InventoryDetailsViewController : UIViewController
	{
		private CustomAlertDetailPage Alert;
	private MetalsAlert MetalAlert;
	private LocationsAlert LocationAlert;
	UILabel InvoiceDescriptionLabel { get; set; }
	UITextField InvoiceDescriptionTextField { get; set; }
	UILabel InvoiceSaleLabel { get; set; }
	UITextField InvoiceSalePriceTextField { get; set; }
	UIScrollView _scrollView { get; set; }
	UILabel lblGL, lblSku, lblInfoLine1, lblInfoLine2, lblInfoLine3, lblPrice, lblSecretCode, lblTagPrice, lblMetal;
	UITextField txtTSOC, txtSecretCode, txtTagPrice, txtMetal, txtInfoLine1, txtInfoLine2, txtInfoLine3, txtSku;
	public MasterInventoryViewController ParentMasterInventoryViewController { get; set; }

	public override void ViewDidLayoutSubviews()
	{
		//Alert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
		//MetalAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
		//LocationAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);

		//if (!AzureService.InvoiceService.IsInvoiceSelected)
		//	AddToInvoice.Hidden = true;
	}

	/// <summary>
	/// /////
	/// </summary>
	public UIButton img_UploadImage { get; set; }
	public static UpdatingOverlay updatingOverlay;
	public static SavingOverlay savingOverlay;

	private bool _AlertOpen = false;

	public void SetAlertOpen(bool open)
	{
		_AlertOpen = open;
	}
	private bool costSwapped = false;
	private UIView activeview;             // Controller that activated the keyboard
	private float scrollamount = 0.0f;    // amount to scroll 
	private float bottom = 0.0f;           // bottom point
	private float offset = 10.0f;          // extra offset
	private bool moveViewUp = false;           // which direction are we moving
	private UITextView textView;

	UIAlertController alertWithTextBox;

	//private UIScrollView scrollView;

	public InventoryNS InventoryItem
	{
		get;
		set;
	}

	public void InitializeView()
	{
		int _X = 445;
		int _w = 300;
		//_scrollView = new UIScrollView(new CGRect(0, 63, View.Bounds.Width, View.Bounds.Height));
		View.Add(_scrollView);
		lblGL = new UILabel(new CGRect(_X, 210, 40, 30))
		{
			Font = UIFont.SystemFontOfSize(30f)
		};
		lblGL.Text = "GL";
		_scrollView.Add(lblGL);
		txtTSOC = new UITextField(new CGRect(485, 210, 100, 30))
		{
			Placeholder = "TSOC"
		};
		txtTSOC.ToInventoryItemTextField(true);
		_scrollView.Add(txtTSOC);

		lblSecretCode = new UILabel(new CGRect(_X, lblGL.Frame.Height + lblGL.Frame.Y + 15, 120, 21));
		lblSecretCode.Text = "Secret Code";
		lblSecretCode.ToInventoryLabel();
		//_scrollView.Add(lblSecretCode);

		txtSecretCode = new UITextField(new CGRect(_X, txtTSOC.Frame.Height + txtTSOC.Frame.Y + 5, _w, 30))
		{
			Placeholder = "SecretCode"
		};
		txtSecretCode.ToInventoryItemTextField(true);
		_scrollView.Add(txtSecretCode);

		lblTagPrice = new UILabel(new CGRect(_X, txtSecretCode.Frame.Height + txtSecretCode.Frame.Y + 15, 100, 21));
		lblTagPrice.Text = "Tag Price";
		lblTagPrice.ToInventoryLabel();
		//_scrollView.Add(lblTagPrice);

		txtTagPrice = new UITextField(new CGRect(_X, txtSecretCode.Frame.Height + txtSecretCode.Frame.Y + 5, _w, 30))
		{
			Placeholder = "TagPrice"
		};
		txtTagPrice.ToInventoryItemTextField(true);
		_scrollView.Add(txtTagPrice);

		lblMetal = new UILabel(new CGRect(_X, txtTagPrice.Frame.Height + txtTagPrice.Frame.Y + 15, 100, 21));
		lblMetal.Text = "Metal";
		lblMetal.ToInventoryLabel();
		//_scrollView.Add(lblMetal);

		txtMetal = new UITextField(new CGRect(_X, txtTagPrice.Frame.Height + txtTagPrice.Frame.Y + 5, _w, 30))
		{
			Placeholder = "Metal"
		};
		txtMetal.ToInventoryItemTextField(true);
		_scrollView.Add(txtMetal);

		lblInfoLine1 = new UILabel(new CGRect(_X, txtMetal.Frame.Height + txtMetal.Frame.Y + 15, 100, 21));
		lblInfoLine1.Text = "Info Line1";
		lblInfoLine1.ToInventoryLabel();
		//_scrollView.Add(lblInfoLine1);

		txtInfoLine1 = new UITextField(new CGRect(_X, txtMetal.Frame.Height + txtMetal.Frame.Y + 5, _w, 30))
		{
			Placeholder = "InfoLine1"
		};
		txtInfoLine1.ToInventoryItemTextField(true);
		_scrollView.Add(txtInfoLine1);

		lblInfoLine2 = new UILabel(new CGRect(_X, txtInfoLine1.Frame.Height + txtInfoLine1.Frame.Y + 15, 100, 21));
		lblInfoLine2.Text = "Info Line2";
		lblInfoLine2.ToInventoryLabel();
		//_scrollView.Add(lblInfoLine2);

		txtInfoLine2 = new UITextField(new CGRect(_X, txtInfoLine1.Frame.Height + txtInfoLine1.Frame.Y + 5, _w, 30))
		{
			Placeholder = "InfoLine2"
		};
		txtInfoLine2.ToInventoryItemTextField(true);
		_scrollView.Add(txtInfoLine2);


		lblInfoLine3 = new UILabel(new CGRect(_X, txtInfoLine2.Frame.Height + txtInfoLine2.Frame.Y + 15, 100, 21));
		lblInfoLine3.Text = "Info Line3";
		lblInfoLine3.ToInventoryLabel();
		//_scrollView.Add(lblInfoLine3);

		txtInfoLine3 = new UITextField(new CGRect(_X, txtInfoLine2.Frame.Height + txtInfoLine2.Frame.Y + 5, _w, 30))
		{
			Placeholder = "InfoLine3"
		};
		txtInfoLine3.ToInventoryItemTextField(true);
		_scrollView.Add(txtInfoLine3);

		lblSku = new UILabel(new CGRect(_X, txtInfoLine3.Frame.Height + txtInfoLine3.Frame.Y + 15, 100, 21));
		lblSku.Text = "Sku";
		lblSku.ToInventoryLabel();
		//_scrollView.Add(lblSku);

		txtSku = new UITextField(new CGRect(_X, txtInfoLine3.Frame.Height + txtInfoLine3.Frame.Y + 5, _w, 30))
		{
			Placeholder = "Sku"
		};
		txtSku.ToInventoryItemTextField(true);
		_scrollView.Add(txtSku);
		Console.Write("Y coordinate : " + txtSku.Frame);

	}
	public InventoryDetailsViewController(IntPtr handle) : base(handle)
		{
		//metalCode = " ";
		//var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveItem);
		//saveButton.AccessibilityLabel = "saveButton";
		//NavigationItem.RightBarButtonItem = saveButton;

		//// Keyboard popup
		//NSNotificationCenter.DefaultCenter.AddObserver
		//(UIKeyboard.DidShowNotification, KeyBoardUpNotification);

		//// Keyboard Down
		//NSNotificationCenter.DefaultCenter.AddObserver
		//(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

	}

	public override async void ViewDidLoad()
	{
		base.ViewDidLoad();
		//	InventoryScrollview.Frame=new CGRect(0,63,this.View.Frame.Width,View.Frame.Height);
		var currentFrame = this.View.Bounds;
		var bounds = UIScreen.MainScreen.Bounds;
		_scrollView = new UIScrollView(this.View.Frame);
		_scrollView.Frame = new CGRect(0, 63, this.View.Frame.Width, View.Frame.Height - 63);
		//InventoryScrollview.ContentSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);
		_scrollView.BackgroundColor = UIColor.Red;
		InitializeView();

		//this.View.BackgroundColor = UIColor.Green;
		//var categories = AzureService.InventoryService.Categories;
		//var metalCodesList = AzureService.InventoryService.MetalCodes;
		//var locationsList = AzureService.LocationService.Locations.Select(x => x.Name).ToList();

		InvoiceDescriptionLabel = new UILabel(new CGRect(txtSku.Frame.X, txtSku.Frame.Y + txtSku.Frame.Height + 5, txtSku.Frame.Width, txtSku.Frame.Height));
		InvoiceDescriptionLabel.ToInventoryLabel();
		InvoiceDescriptionLabel.Text = "Invoice Description";
		//if(InventoryItem.Sold)
		_scrollView.AddSubview(InvoiceDescriptionLabel);

		InvoiceDescriptionTextField = new UITextField(new CGRect(InvoiceDescriptionLabel.Frame.X, InvoiceDescriptionLabel.Frame.Y + InvoiceDescriptionLabel.Frame.Height, InvoiceDescriptionLabel.Frame.Width * 2, InvoiceDescriptionLabel.Frame.Height));
		InvoiceDescriptionTextField.ToInventoryItemTextField(true);
		//InvoiceDescriptionTextField.Hidden = !InventoryItem.Sold;
		InvoiceDescriptionTextField.Placeholder = "Invoice Description";
		_scrollView.AddSubview(InvoiceDescriptionTextField);

		InvoiceSaleLabel = new UILabel(new CGRect(InvoiceDescriptionTextField.Frame.X, InvoiceDescriptionTextField.Frame.Y + InvoiceDescriptionTextField.Frame.Height, InvoiceDescriptionLabel.Frame.Width, InvoiceDescriptionTextField.Frame.Height));
		InvoiceSaleLabel.ToInventoryLabel();
		InvoiceSaleLabel.Text = "Sale Price";
		//if (InventoryItem.Sold)
		_scrollView.AddSubview(InvoiceSaleLabel);

		InvoiceSalePriceTextField = new UITextField(new CGRect(InvoiceSaleLabel.Frame.X, InvoiceSaleLabel.Frame.Y + InvoiceSaleLabel.Frame.Height, InvoiceSaleLabel.Frame.Width, InvoiceSaleLabel.Frame.Height));
		InvoiceSalePriceTextField.ToInventoryItemTextField();
		//InvoiceSalePriceTextField.Hidden = !InventoryItem.Sold;
		InvoiceSalePriceTextField.Placeholder = "Invoice Sale Price";
		_scrollView.AddSubview(InvoiceSalePriceTextField);
		_scrollView.ContentSize = new CGSize(this.View.Frame.Width, InvoiceSalePriceTextField.Frame.Y + 50);

		//var metalList = new List<string>();
		//foreach (var item in metalCodesList.ToList())
		//{
		//	metalList.Add(item.Value);
		//}
		//Alert = new CustomAlertDetailPage(View.Bounds.Size.Width, View.Bounds.Height, categories, "", this);
		//MetalAlert = new MetalsAlert(View.Bounds.Size.Width, View.Bounds.Height, metalList, this);
		//LocationAlert = new LocationsAlert(View.Bounds.Size.Width, View.Bounds.Height, locationsList, this);

		//await ConfigureView();
		//View.MultipleTouchEnabled = true;
		//View.UserInteractionEnabled = true;

		//if (!AzureService.InvoiceService.IsInvoiceSelected)
		//{
		//	var btn = new UIBarButtonItem("< Inventory", UIBarButtonItemStyle.Plain, (sender, e) =>
		//		 {
		//			 if (haveChangedBeenMade())
		//			 {
		//				 UIAlertController alert = UIAlertController.Create("Leaving the page?", "If you leave the page before saving, all changes will be lost.", UIAlertControllerStyle.Alert);
		//				 alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
		//				 {
		//				 }));
		//				 alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
		//				 {
		//					 this.NavigationController.PopViewController(true);
		//				 }));
		//				 PresentViewController(alert, true, null);
		//			 }
		//			 else
		//			 {
		//				 this.NavigationController.PopViewController(true);
		//			 }
		//		 });
		//	this.NavigationItem.LeftBarButtonItem = btn;
		//}
	}

	public override void ViewWillAppear(bool animated)
	{
		base.ViewWillAppear(animated);
		//InventoryScrollview.ContentSize = new CGSize(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Width);
		//this._scrollView.InitKeyboardScrollView();
	}
	public override void ViewWillDisappear(bool animated)
	{
		base.ViewWillDisappear(animated);
		//this._scrollView.UnsubscribeKeyboardScrollView();
	}
	public override void ViewDidAppear(bool animated)
	{
		//if (AzureService.InventoryService.NeedLocalRefresh)
		//{
		//	var updatedItem = AzureService.InventoryService.Items.Where(x => x.Id == InventoryItem.Id).FirstOrDefault();
		//	if (updatedItem != null)
		//	{
		//		InventoryItem.GlCost = updatedItem.GlCost.Value;
		//		InventoryItem.TagPrice = updatedItem.TagPrice;
		//		InventoryItem.SetGLShortCode();// = updatedItem.;
		//		InventoryItem.VendorCode = updatedItem.VendorCode;

		//		SetupFields();
		//	}
		//}
		//SetMetalCodeButtonText();
		//SetLocationButtonText();

		//img_UploadImage.SetImage(InventoryItem.ThumbnailImage, UIControlState.Normal);
		//costSwapped = true;
	}
	private async Task ConfigureView()
	{
		//if (InventoryItem == null)
		//{
		//	return;
		//}

		//this.NavigationController.SetNavigationBarHidden(true, false);

		//img_UploadImage = UIButton.FromType(UIButtonType.Custom);
		//img_UploadImage.Frame = new CoreGraphics.CGRect(25, 40, 400, 533);  // changed 
		//img_UploadImage.SetImage(InventoryItem.ThumbnailImage, UIControlState.Normal);//UIImage.FromFile("UploadLocal.png");
		//img_UploadImage.ImageView.Frame = new CoreGraphics.CGRect(25, 40, 400, 533);
		//img_UploadImage.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
		//img_UploadImage.ImageView.Image.Scale(new CoreGraphics.CGSize(400, 300));

		//img_UploadImage.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;

		////Set up event handler for "Click" event ("TouchUpInside in iOS terminology)
		//img_UploadImage.TouchUpInside += (object sender, EventArgs e) =>
		//{
		//	ShowImages();
		//	return;

		//};

		//System.Drawing.RectangleF barcodeRect = new System.Drawing.RectangleF(425, 40, 333, 120);
		//IGBarcodeView barcodeEAN13 = IGBarcodeView.CreateBarcodeFrame(IGBarcodeType.IGBarcodeTypeCode128, barcodeRect);
		//barcodeEAN13.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;
		////UIViewAutoresizing.FlexibleWidth|UIViewAutoresizing.FlexibleHeight|
		////UIViewAutoresizing.FlexibleLeftMargin|UIViewAutoresizing.FlexibleBottomMargin|
		////UIViewAutoresizing.FlexibleRightMargin|UIViewAutoresizing.FlexibleTopMargin;
		//if (InventoryItem.Sku != null)
		//	barcodeEAN13.SetValue(InventoryItem.Sku);
		//barcodeEAN13.ShowText = false;
		//barcodeEAN13.FontName = UIFont.FromName("MarkerFelt-Wide", 0).Name;
		////barcodeEAN13.Center = this.View.Center;

		//this.NavigationController.SetNavigationBarHidden(false, false);
		//_scrollView.AddSubview(img_UploadImage);//ProductImage);

		//_scrollView.Add(barcodeEAN13);

		//ItemCodeTextField.ShouldReturn += TextFieldShouldReturn;
		////Metal.ShouldReturn += TextFieldShouldReturn;
		//Secret.ShouldReturn += TextFieldShouldReturn;
		////Vendor.ShouldReturn += TextFieldShouldReturn;
		////GLCost.ShouldReturn += TextFieldShouldReturn;
		////TagPrice.ShouldReturn += TextFieldShouldReturn;
		//Sku.ShouldReturn += TextFieldShouldReturn;
		//InfoLine1.ShouldReturn += TextFieldShouldReturn;
		//InfoLine2.ShouldReturn += TextFieldShouldReturn;
		//InfoLine3.ShouldReturn += TextFieldShouldReturn;
		//SetupFields();
	}


	//public void SetLocation(string location)
	//{
	//	InventoryItem.Location = location;
	//	var locationButtonText = String.IsNullOrWhiteSpace(InventoryItem.Location) ? "Location" : InventoryItem.Location;
	//	LocationButton.TitleLabel.Text = locationButtonText;
	//	LocationButton.SetTitle(locationButtonText, UIControlState.Normal);
	//}
	//public void SetLocationButtonText()
	//{
	//	var locationButtonText = String.IsNullOrWhiteSpace(InventoryItem.Location) ? "Location" : InventoryItem.Location;
	//	LocationButton.TitleLabel.Text = locationButtonText;
	//	LocationButton.SetTitle(locationButtonText, UIControlState.Normal);
	//}
	//public void SetMetalCodeButtonText()
	//{
	//	var metalButtonText = String.IsNullOrWhiteSpace(InventoryItem.MetalCode) ? "Metal Code" : InventoryItem.MetalCode;
	//	MetalCodeButton.TitleLabel.Text = metalButtonText;
	//	MetalCodeButton.SetTitle(metalButtonText, UIControlState.Normal);
	//}
	//public void SetMetalCode(string metal)
	//{
	//	InventoryItem.MetalCode = metal;
	//	var metalButtonText = String.IsNullOrWhiteSpace(InventoryItem.MetalCode) ? "Metal Code" : InventoryItem.MetalCode;
	//	MetalCodeButton.TitleLabel.Text = metalButtonText;
	//	MetalCodeButton.SetTitle(metalButtonText, UIControlState.Normal);
	//}

	//public void SetupFields()
	//{
	//	GLCostTextField.Text = Reverse(InventoryItem.GlCost.ToString());
	//	ItemCodeTextField.Text = InventoryItem.GlItemCode;
	//	//ItemCodeTextField.Text = ic.Code;
	//	this.View.UserInteractionEnabled = true;
	//	SetMetalCodeButtonText();
	//	Secret.Text = InventoryItem.SecretCode;
	//	TagPrice.Enabled = true;
	//	TagPrice.UserInteractionEnabled = true;
	//	TagPrice.ShouldChangeCharacters += (field, range, replacementString) => false;
	//	TagPrice.Text = Math.Ceiling(InventoryItem.TagPrice).ToString("C");
	//	//var tap = new UITapGestureRecognizer(TappedTagPriceLabel);
	//	//tap.NumberOfTapsRequired = 1;
	//	//TagPrice.AddGestureRecognizer(tap); ;
	//	TagPrice.AddTarget(this, new Selector("tagPrice"), UIControlEvent.EditingDidBegin);

	//	Sku.Text = InventoryItem.Sku;
	//	InvoiceDescriptionTextField.Text = String.IsNullOrWhiteSpace(InventoryItem.InvoiceDescription) ? InventoryItem.DisplayDescription : InventoryItem.InvoiceDescription;
	//	InvoiceSalePriceTextField.Text = InventoryItem.SalePrice == 0 ? Math.Ceiling(InventoryItem.TagPrice).ToString("C") : Math.Ceiling(InventoryItem.SalePrice).ToString("C");

	//	InfoLine1.Text = InventoryItem.Info1;
	//	InfoLine2.Text = InventoryItem.Info2;
	//	InfoLine3.Text = InventoryItem.Info3;

	//	SoldSwitch.On = InventoryItem.Sold;

	//	SetLocationButtonText();

	//	img_UploadImage.SetImage(InventoryItem.ThumbnailImage, UIControlState.Normal);//UIImage.FromFile("UploadLocal.png");

	//	if (InventoryItem.Sold)
	//		SoldSwitch.Enabled = false;
	//}

	//double salePrice;

	//[Export("tagPrice")]
	//public void TappedTagPriceLabel()
	//{
	//	//TagPrice.ResignFirstResponder();

	//	alertWithTextBox = UIAlertController.Create("Sale Price ", "Enter Sale Price", UIAlertControllerStyle.Alert);
	//	alertWithTextBox.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
	//	alertWithTextBox.AddAction(UIAlertAction.Create("Update", UIAlertActionStyle.Default, (UIAlertAction obj) => OnUpdateSalePrice()));
	//	alertWithTextBox.AddTextField((field) =>
	//	{
	//		field.Placeholder = "Price";
	//		field.KeyboardType = UIKeyboardType.NumberPad;
	//		field.ToNumeric();
	//	});
	//	PresentViewController(alertWithTextBox, animated: true, completionHandler: null);
	//}

	//void OnUpdateSalePrice()
	//{
	//	salePrice = Double.Parse(alertWithTextBox?.TextFields[0]?.Text);
	//}

	//public void ShowUpdatingOverlay()
	//{
	//	var bounds = UIScreen.MainScreen.Bounds;
	//	updatingOverlay = new UpdatingOverlay(bounds);
	//	View.Add(updatingOverlay);
	//}

	//public void HideUpdatingOverlay()
	//{
	//	updatingOverlay.Hide();
	//}
	//public void ShowSavingOverlay()
	//{
	//	var bounds = UIScreen.MainScreen.Bounds;
	//	savingOverlay = new SavingOverlay(bounds);
	//	View.Add(savingOverlay);
	//}

	//public void HideSavingOverlay()
	//{
	//	savingOverlay.Hide();
	//}

	//async void SaveItem(object sender, EventArgs args)
	//{
	//	//RMR need to show saving load here
	//	if (!(!String.IsNullOrWhiteSpace(Sku.Text) && Sku.Text.Length == 6))
	//	{
	//		var alert = UIAlertController.Create("Invalid Sku", "SKU must be 6 digits long", UIAlertControllerStyle.Alert);

	//		alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
	//		PresentViewController(alert, animated: true, completionHandler: null);
	//		return;

	//	}
	//	await performSave();
	//}
	//public async Task UpdateGrid()
	//{
	//	int itemIndex = -1;
	//	itemIndex = System.Array.IndexOf(((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data, InventoryItem);
	//	//for (int i = 0; i < ((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data.Count(); i++)
	//	//{
	//	//	if (((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[i]).Id == InventoryItem.Id)
	//	//	{
	//	//		itemIndex = i;
	//	//		break;
	//	//	}
	//	//}
	//	if (itemIndex != -1)
	//	{
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).GlItemCode = ItemCodeTextField.Text;
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).MetalCode = MetalCodeButton.TitleLabel.Text == "Metal Code" ? "" : MetalCodeButton.TitleLabel.Text;
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).SecretCode = Secret.Text;
	//		//InventoryItem.VendorCode = Vendor.Text;

	//		//InventoryItem.GlCost = Double.Parse(GLCost.Text);
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).TagPrice = Double.Parse(TagPrice.Text.Replace("$", ""));
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).SalePrice = salePrice; //Double.Parse(TagPrice.Text.Replace("$", ""));
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Sold = InventoryItem.Sold;//IsSold = InventoryItem.IsSold; //Double.Parse(TagPrice.Text.Replace("$", ""));

	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Sku = Sku.Text;

	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Info1 = InfoLine1.Text;
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Info2 = InfoLine2.Text;
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Info3 = InfoLine3.Text;

	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Sold = SoldSwitch.On;
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Location = LocationButton.TitleLabel.Text == "Location" ? "" : LocationButton.TitleLabel.Text;

	//	}
	//	else {
	//		await AzureService.DefaultService.PushAsync();

	//		//var items = AzureService.InventoryService.GetLocalNSItems()
	//		await AzureService.InventoryService.UpdateItemsFromLocalDB();
	//		var items = AzureService.InventoryService.ItemsNS;

	//		((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data = items.ToArray();
	//		//((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh
	//		await ((MasterInventoryViewController)ParentMasterInventoryViewController).UpdateGrid();
	//	}
	//}
	//private async Task<bool> performSave()
	//{
	//	ShowSavingOverlay();
	//	if (this.NavigationItem.LeftBarButtonItem != null)
	//		this.NavigationItem.LeftBarButtonItem.Enabled = false;
	//	else
	//		this.NavigationItem.HidesBackButton = true;


	//	this.NavigationItem.RightBarButtonItem.Enabled = false;

	//	InventoryItem.GlItemCode = ItemCodeTextField.Text;
	//	InventoryItem.MetalCode = MetalCodeButton.TitleLabel.Text == "Metal Code" ? "" : MetalCodeButton.TitleLabel.Text;
	//	InventoryItem.SecretCode = Secret.Text;
	//	//InventoryItem.VendorCode = Vendor.Text;

	//	//InventoryItem.GlCost = Double.Parse(GLCost.Text);
	//	InventoryItem.TagPrice = Double.Parse(TagPrice.Text.Replace("$", ""));
	//	InventoryItem.SalePrice = salePrice;


	//	InventoryItem.Sku = Sku.Text;
	//	InventoryItem.InvoiceDescription = InvoiceDescriptionTextField.Text;

	//	InventoryItem.Info1 = InfoLine1.Text;
	//	InventoryItem.Info2 = InfoLine2.Text;
	//	InventoryItem.Info3 = InfoLine3.Text;

	//	InventoryItem.Sold = SoldSwitch.On;
	//	InventoryItem.Location = LocationButton.TitleLabel.Text == "Location" ? "" : LocationButton.TitleLabel.Text;

	//	await AzureService.InventoryService.InsertOrSave(InventoryItem.ConvertToInventory(), productImageList);
	//	SetMetalCodeButtonText();
	//	SetLocationButtonText();

	//	AzureService.InventoryService.NeedLocalRefresh = true;
	//	if (ParentMasterInventoryViewController != null)
	//	{
	//		await UpdateGrid();
	//		AzureService.InventoryService.UpdateItemNSFromLocalItems(InventoryItem.Id);

	//	}
	//	else {
	//		if (AzureService.InvoiceService.ItemsList.Any(x => x.Id == InventoryItem.Id))
	//			AzureService.InvoiceService.ItemsList.FirstOrDefault(x => x.Id == InventoryItem.Id).InvoiceDescription = InventoryItem.InvoiceDescription;
	//		this.NavigationController.PopViewController(true);
	//	}
	//	if (this.NavigationItem.LeftBarButtonItem != null)
	//		this.NavigationItem.LeftBarButtonItem.Enabled = true;
	//	else
	//		this.NavigationItem.HidesBackButton = false;

	//	this.NavigationItem.RightBarButtonItem.Enabled = true;
	//	HideSavingOverlay();

	//	return true;
	//}
	//partial void UIButton538_TouchUpInside(UIButton sender)
	//{
	//	if (String.IsNullOrWhiteSpace(InventoryItem.Sku))
	//	{
	//		var alert = UIAlertController.Create("Invalid Sku", "An image cannot be added until a sku has been input", UIAlertControllerStyle.Alert);

	//		alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
	//		PresentViewController(alert, animated: true, completionHandler: null);
	//		return;

	//	}

	//	Camera.TakePicture(this, (obj) =>
	//		{
	//			var image = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;

	//			updateImage(image);

	//			SetupFields();
	//		});
	//}

	//List<ProductImage> productImageList = new List<ProductImage>();
	//public void updateImage(UIImage image)
	//{
	//	var productImage = AzureService.InventoryService.AddOrUpdateImage(image, InventoryItem.ConvertToInventory());
	//	productImageList.Add(productImage);
	//	img_UploadImage.SetImage(InventoryItem.ThumbnailImage, UIControlState.Normal);

	//	InventoryItem.Images.Add(productImage);
	//	AzureService.InventoryService.NeedLocalRefresh = true;
	//	AzureService.InventoryService.NeedImageRefresh = true;
	//	UpdateGridImage(productImage);
	//	//var result = Task.Run(() => UpdateGridImage(productImage));
	//}
	//public async Task UpdateGridImage(ProductImage productImage)
	//{
	//	int itemIndex = -1;
	//	itemIndex = System.Array.IndexOf(((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data, InventoryItem);

	//	if (itemIndex != -1)
	//	{
	//		((InventoryNS)((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data[itemIndex]).Images.Add(productImage);
	//	}
	//	else
	//	{
	//		await AzureService.DefaultService.PushAsync();

	//		//var items = AzureService.InventoryService.GetLocalNSItems()
	//		await AzureService.InventoryService.UpdateItemsFromLocalDB(); ;
	//		var items = AzureService.InventoryService.ItemsNS;

	//		((MasterInventoryViewController)ParentMasterInventoryViewController)._dsh.Data = items.ToArray();
	//	}

	//}
	//partial void AddToInvoice_TouchUpInside(UIButton sender)
	//{
	//	//            var alert = UIAlertController.Create("Not implemented", "This Feature is not available in this verison of iManager", UIAlertControllerStyle.Alert);
	//	//
	//	//            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
	//	//            PresentViewController(alert, animated: true, completionHandler: null);
	//	//			return;
	//	var item = AzureService.InventoryService.Items.Where(x => x.Id == InventoryItem.Id).FirstOrDefault();
	//	if (item.IsSold)
	//	{
	//		var alert = new UIAlertView("Sold Items Cannot Be Added", "Item already sold", null, "Ok");
	//		alert.Show();
	//		return;
	//	}
	//	//item.SalePrice = salePrice;
	//	item.Price = salePrice;
	//	item.IsSold = true;
	//	item.AddNewFromInvoice = true;
	//	InventoryItem.Sold = true;//IsSold = true;

	//	AzureService.InvoiceService.ItemsList.Insert(0, item);
	//	//InvoiceCreateViewController mivc = this.Storyboard.InstantiateViewController("InvoiceCreateViewController") as InvoiceCreateViewController;
	//	if (AzureService.InvoiceService.IsInvoiceSelected == true)
	//	{
	//		this.NavigationController.PopViewController(true);
	//		this.NavigationController.PopViewController(true);
	//	}
	//	else {
	//		var alert = new UIAlertView("No Active Invoice", "To add and item to an invoice you must first create an invoice and then select Add Item, or Add by Sku", null, "Ok");
	//		alert.Show();
	//		return;
	//	}
	//	UpdateGrid();
	//}

	//private bool TextFieldShouldReturn(UITextField textfield)
	//{
	//	nint nextTag = textfield.Tag + 1;
	//	UIResponder nextResponder = this.View.ViewWithTag(nextTag);
	//	if (nextResponder != null)
	//	{
	//		nextResponder.BecomeFirstResponder();
	//	}
	//	else {
	//		// Not found, so remove keyboard.
	//		textfield.ResignFirstResponder();
	//		ResetTheScroller();
	//	}
	//	return false;
	//}
	//private void KeyBoardUpNotification(NSNotification notification)
	//{
	//	if (!_AlertOpen)
	//	{
	//		SetScrollHeightForEditing(-210);
	//		AddKeyboardTapOutGester();
	//	}
	//	else {
	//		SetScrollHeightForEditing(-145);

	//	}
	//}
	//private void KeyBoardDownNotification(NSNotification notification)
	//{
	//	//if(moveViewUp){ScrollTheView(false);}
	//	ResetTheScroller();
	//}
	//private void ResetTheScroller()
	//{
	//	UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
	//	UIView.SetAnimationDuration(0.3);

	//	CoreGraphics.CGRect frame = View.Frame;
	//	frame.Y = 0;
	//	scrollamount = 0;

	//	View.Frame = frame;
	//	UIView.CommitAnimations();
	//}
	//private void SetScrollHeightForEditing(int yValue)
	//{
	//	UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
	//	UIView.SetAnimationDuration(0.3);

	//	CoreGraphics.CGRect frame = View.Frame;
	//	frame.Y = yValue;
	//	scrollamount = 0;

	//	View.Frame = frame;
	//	UIView.CommitAnimations();
	//}
	//private void ScrollTheView(bool move)
	//{

	//	// scroll the view up or down
	//	UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
	//	UIView.SetAnimationDuration(0.3);

	//	CoreGraphics.CGRect frame = View.Frame;
	//	frame.Y = 0;
	//	if (move)
	//	{
	//		frame.Y -= scrollamount;
	//	}
	//	else {
	//		frame.Y += scrollamount;
	//		scrollamount = 0;
	//	}

	//	View.Frame = frame;
	//	UIView.CommitAnimations();
	//}

	//public void HideKeyboard()
	//{
	//	// Find what opened the keyboard
	//	foreach (UIView view in this.View.Subviews)
	//	{
	//		if (view.IsFirstResponder)
	//		{
	//			view.ResignFirstResponder();
	//		}

	//	}
	//	RemoveAllViewGestures();
	//}
	//private void AddKeyboardTapOutGester()
	//{
	//	UITapGestureRecognizer tapGesture = new UITapGestureRecognizer();
	//	tapGesture.AddTarget(() => HideKeyboard());
	//	tapGesture.ShouldReceiveTouch += (recongizer, touch) => !(touch.View is CustomAlert || touch.View is NewCodeAlert || touch.View is CustomAlertDetailPage);

	//	this.View.AddGestureRecognizer(tapGesture);
	//}
	//private void RemoveAllViewGestures()
	//{
	//	this.View.GestureRecognizers = null;
	//}

	//partial void GLCostDidEndEditing(UITextField sender)
	//{

	//	InventoryItem.GlCost = String.IsNullOrWhiteSpace(GLCostTextField.Text) ? 0.00 : Double.Parse(GLCostTextField.Text);
	//	UpdateGLShortCode();
	//	InventoryItem.SetTagPrice();
	//	TagPrice.Text = Math.Ceiling(InventoryItem.TagPrice).ToString("C");

	//	reservseGLCostTextField();
	//	costSwapped = true;

	//	Secret.BecomeFirstResponder();
	//}

	//private void reservseGLCostTextField()
	//{
	//	GLCostTextField.Text = Reverse(GLCostTextField.Text);
	//}
	//public static string Reverse(string s)
	//{
	//	char[] charArray = s.ToCharArray();
	//	Array.Reverse(charArray);
	//	return new string(charArray);
	//}
	//public void UpdateGLShortCode()
	//{
	//	InventoryItem.SetGLShortCode();
	//	//GLShortCodeTextField.Text = InventoryItem.GlShortCode;
	//}



	//partial void TouchDownGLCost(UITextField sender)
	//{
	//	if (costSwapped)
	//	{
	//		reservseGLCostTextField();
	//		costSwapped = false;
	//	}
	//}
	////public void UpdateCodeButtonText()
	////{
	////	var metalButtonText = String.IsNullOrWhiteSpace(ItemCodeTextField.Text) ? "IC" : ItemCodeTextField.Text;
	////	MetalCodeButton.TitleLabel.Text = metalButtonText;
	////	MetalCodeButton.SetTitle(metalButtonText, UIControlState.Normal);
	////}
	//public void UpdateCode(string code)
	//{
	//	ItemCodeTextField.Text = code;
	//	InventoryItem.GlItemCode = code;
	//	UpdateGLShortCode();
	//}
	//partial void ItemCodeButton_TouchUpInside(UIButton sender)
	//{
	//	_AlertOpen = true;
	//	View.AddSubview(Alert);
	//}

	//[Export("ShowImages")]
	//public void ShowImages()
	//{
	//	ItemPhotosViewController itemPhotosViewController = this.Storyboard.InstantiateViewController("ItemPhotosViewController") as ItemPhotosViewController;
	//	itemPhotosViewController.Photos = InventoryItem.GetPhotoInfoList();
	//	itemPhotosViewController.ParentInventoryDetailViewController = this;
	//	this.NavigationController.PushViewController(itemPhotosViewController, true);

	//}

	//partial void GLCostDidEndOnEx(UITextField sender)
	//{
	//	InventoryItem.GlCost = String.IsNullOrWhiteSpace(GLCostTextField.Text) ? 0.00 : Double.Parse(GLCostTextField.Text);
	//	UpdateGLShortCode();
	//	InventoryItem.SetTagPrice();
	//	TagPrice.Text = Math.Ceiling(InventoryItem.TagPrice).ToString("C");

	//	reservseGLCostTextField();
	//	costSwapped = true;

	//	Secret.BecomeFirstResponder();
	//}

	//partial void MetalCodeButton_TouchUpInside(UIButton sender)
	//{
	//	View.AddSubview(MetalAlert);
	//}
	//partial void LocationButton_TouchUpInside(UIButton sender)
	//{
	//	View.AddSubview(LocationAlert);
	//}

	////Printing
	//SFBarcode barcode;
	//CGRect frameRect = new CGRect();

	//partial void PrintTag_TouchUpInside(UIButton sender)
	//{

	//	//Add this back in once save is working RMR			var saved = performSave().Result;
	//	LoadBarcode();
	//	//PrintTagWtihRender();
	//}
	//void LoadBarcode()
	//{
	//	PdfDocument pdfdoc = new PdfDocument();
	//	pdfdoc.PageSettings = new PdfPageSettings(new Syncfusion.Drawing.SizeF(200, 78f));
	//	PdfPage page = pdfdoc.Pages.Add();
	//	PdfCode128CBarcode barcodepdf = new PdfCode128CBarcode(InventoryItem.Sku);
	//	barcodepdf.TextDisplayLocation = TextLocation.None;
	//	barcodepdf.BarHeight = 13f;
	//	barcodepdf.NarrowBarWidth = 0.48f;

	//	float startingX = 115f;
	//	float lineHeight = 7f;
	//	float nextLineY = 0f;
	//	barcodepdf.Draw(page, new Syncfusion.Drawing.PointF(startingX, nextLineY));

	//	PdfGraphics graphics = page.Graphics;

	//	//Create a solid brush

	//	PdfBrush brush = new PdfSolidBrush(Color.FromArgb(255, 0, 0, 0));

	//	//Set the font

	//	PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8, PdfFontStyle.Bold);

	//	//Draw the text

	//	graphics.DrawString(InventoryItem.GlShortCode, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += 13f));
	//	graphics.DrawString(InventoryItem.SecretCode == null ? "" : InventoryItem.SecretCode, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
	//	graphics.DrawString((NSString)InventoryItem.TagPrice.ToString("C"), font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));

	//	//add space for fold
	//	nextLineY += 2;

	//	graphics.DrawString(InventoryItem.MetalCode, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
	//	graphics.DrawString(InventoryItem.Info1 == null ? "" : InventoryItem.Info1, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
	//	graphics.DrawString(InventoryItem.Info2 == null ? "" : InventoryItem.Info2, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
	//	graphics.DrawString(InventoryItem.Info3 == null ? "" : InventoryItem.Info3, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
	//	graphics.DrawString(InventoryItem.Sku == null ? "" : InventoryItem.Sku, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));

	//	//graphics.DrawRectangle (new PdfPen(Color.Black), new Syncfusion.Drawing.RectangleF (0f, 0f, 134f, 67f));
	//	MemoryStream stream = new MemoryStream();
	//	pdfdoc.Save(stream);
	//	pdfdoc.Close(true);

	//	Save("DataGrid.pdf", "application/pdf", stream);
	//	//barcode.Draw (frameRect);
	//	//this.AddSubview (barcode);

	//}

	//private void Save(string filename, string contentType, MemoryStream stream)
	//{
	//	string exception = string.Empty;
	//	string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
	//	string filePath = Path.Combine(path, filename);
	//	try
	//	{
	//		FileStream fileStream = File.Open(filePath, FileMode.Create);
	//		stream.Position = 0;
	//		stream.CopyTo(fileStream);
	//		fileStream.Flush();
	//		fileStream.Close();
	//	}
	//	catch (Exception e)
	//	{
	//		exception = e.ToString();
	//	}
	//	if (contentType == "application/html" || exception != string.Empty)
	//		return;
	//	UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
	//	while (currentController.PresentedViewController != null)
	//		currentController = currentController.PresentedViewController;
	//	UIView currentView = currentController.View;

	//	QLPreviewController qlPreview = new QLPreviewController();
	//	QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);
	//	qlPreview.DataSource = new PreviewControllerDS(item);

	//	currentController.PresentViewController((UIViewController)qlPreview, true, (Action)null);
	//}
	//public void PrintTagWtihRender()
	//{
	//	// Get a reference to the singleton iOS printing concierge
	//	UIPrintInteractionController printController = UIPrintInteractionController.SharedPrintController;
	//	printController.Delegate = new TagPrintDelegate();//This delegate always selects the smallest paper size

	//	// Instruct the printing concierge to use our custom UIPrintPageRenderer subclass when printing this job
	//	TapPrintPageRenderer tpr = new TapPrintPageRenderer();
	//	tpr.SetupData(InventoryItem);
	//	printController.PrintPageRenderer = tpr;

	//	// Ask for a print job object and configure its settings to tailor the print request
	//	UIPrintInfo info = UIPrintInfo.PrintInfo;

	//	// B&W or color, normal quality output for mixed text, graphics, and images
	//	info.OutputType = UIPrintInfoOutputType.General;

	//	// Select the job named this in the printer queue to cancel our print request.
	//	info.JobName = "TagPrint";

	//	// Instruct the printing concierge to use our custom print job settings.
	//	printController.PrintInfo = info;

	//	// Present the standard iOS Print Panel that allows you to pick the target Printer, number of pages, double-sided, etc.
	//	printController.Present(true, PrintingCompleted);
	//}


	//void PrintingCompleted(UIPrintInteractionController controller, bool completed, NSError error)
	//{
	//	if (completed != true || error != null)
	//	{
	//		Console.WriteLine(String.Format("Failed to print Completed: {0} \n NSError: {1}", completed.ToString(), error != null ? error.ToString() : "none"));
	//	}
	//}
	//private bool haveChangedBeenMade()
	//{
	//	if (InventoryItem.GlItemCode != ItemCodeTextField.Text ||
	//		InventoryItem.MetalCode != MetalCodeButton.TitleLabel.Text ||
	//		InventoryItem.SecretCode != Secret.Text ||
	//		InventoryItem.TagPrice != Double.Parse(TagPrice.Text.Replace("$", "")) ||
	//		InventoryItem.Sku != Sku.Text ||
	//		InventoryItem.Info1 != InfoLine1.Text ||
	//		InventoryItem.Info2 != InfoLine2.Text ||
	//		InventoryItem.Info3 != InfoLine3.Text ||
	//		InventoryItem.Sold != SoldSwitch.On ||
	//		InventoryItem.Location != LocationButton.TitleLabel.Text)
	//	{
	//		return true;
	//	}
	//	return false;
	//}

}
}