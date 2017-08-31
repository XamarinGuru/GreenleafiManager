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
using GreenleafiManagerPhone.ViewControllers;
using MediaPlayer;
using ObjCRuntime;
using System.IO;
using QuickLook;
//using SampleBrowser;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GreenleafiManagerPhone
{
	public partial class InventoryDetailViewController : UIViewController
	{
		public bool IsNew
		{
			get;
			set;
		}
		private CustomAlertDetailPage Alert;
		private MetalsAlert MetalAlert;
		private LocationsAlert LocationAlert;
		UIKit.UILabel InvoiceDescriptionLabel { get; set; }
		UIKit.UITextField InvoiceDescriptionTextField { get; set; }
		UIKit.UILabel InvoiceSaleLabel { get; set; }
		UIKit.UITextField InvoiceSalePriceTextField { get; set; }
		UIButton imagesButton;

		public override void ViewDidLayoutSubviews()
		{
			Alert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
			MetalAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
			LocationAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);

			//if (!AzureService.InvoiceService.IsInvoiceSelected)
			//	AddToInvoice.Hidden = true;
		}
		/// <summary>
		/// /////
		/// </summary>
		//public UIButton img_UploadImage { get; set; }
		public static UpdatingOverlay updatingOverlay;
		public static SavingOverlay savingOverlay;
		public static LoadingOverlay loadingOverlay;


		private bool _AlertOpen = false;

		public void SetAlertOpen(bool open)
		{
			_AlertOpen = open;
		}
		private bool costSwapped = false;// Controller that activated the keyboard
		private float scrollamount = 0.0f;  

		UIAlertController alertWithTextBox;

		//private UIScrollView scrollView;

		public InventoryNS InventoryItem
		{
			get;
			set;
		}
		public InventoryDetailViewController(IntPtr handle) : base(handle)
		{
			//metalCode = " ";
			var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveItem);
			saveButton.AccessibilityLabel = "saveButton";
			NavigationItem.RightBarButtonItem = saveButton;

			// Keyboard popup
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.DidShowNotification, KeyBoardUpNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			var categories = AzureService.InventoryService.Categories;
			var metalCodesList = AzureService.InventoryService.MetalCodes;
			var locationsList = AzureService.LocationService.Locations.Select(x => x.Name).ToList();



			var metalList = new List<string>();
			foreach (var item in metalCodesList.ToList())
			{
				metalList.Add(item.Value);
			}
			Alert = new CustomAlertDetailPage(View.Bounds.Size.Width, View.Bounds.Height, categories, "", this);
			MetalAlert = new MetalsAlert(View.Bounds.Size.Width, View.Bounds.Height, metalList, this);
			LocationAlert = new LocationsAlert(View.Bounds.Size.Width, View.Bounds.Height, locationsList, this);

			ConfigureView();
			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;

			var btn = new UIBarButtonItem("  Back", UIBarButtonItemStyle.Plain, (sender, e) => { this.NavigationController.PopViewController(true);});
			this.NavigationItem.LeftBarButtonItem = btn;

		}
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(true);
			ShowLoading();

			HideLoading();

			SetMetalCodeButtonText();
			SetLocationButtonText();

			//img_UploadImage.SetImage(InventoryItem.ThumbnailImage, UIControlState.Normal);
			costSwapped = true;
			SetupFields();

			
		}

		private void ConfigureView()
		{
			if (InventoryItem == null)
			{
				return;
			}



			this.NavigationController.SetNavigationBarHidden(false, false);
			//this.View.AddSubview(img_UploadImage);//ProductImage);

		

			ItemCodeTextField.ShouldReturn += TextFieldShouldReturn;
			Secret.ShouldReturn += TextFieldShouldReturn;
			Sku.ShouldReturn += TextFieldShouldReturn;
			InfoLine1.ShouldReturn += TextFieldShouldReturn;
			InfoLine2.ShouldReturn += TextFieldShouldReturn;
			InfoLine3.ShouldReturn += TextFieldShouldReturn;
			SetupFields();
		}


		public void SetLocation(string location)
		{
			InventoryItem.Location = location;
			var locationButtonText = String.IsNullOrWhiteSpace(InventoryItem.Location) ? "Location" : InventoryItem.Location;
			LocationButton.TitleLabel.Text = locationButtonText;
			LocationButton.SetTitle(locationButtonText, UIControlState.Normal);
		}
		public void SetLocationButtonText()
		{
			var locationButtonText = String.IsNullOrWhiteSpace(InventoryItem.Location) ? "Location" : InventoryItem.Location;
			LocationButton.TitleLabel.Text = locationButtonText;
			LocationButton.SetTitle(locationButtonText, UIControlState.Normal);
		}
		public void SetMetalCodeButtonText()
		{
			var metalButtonText = String.IsNullOrWhiteSpace(InventoryItem.MetalCode) ? "Metal Code" : InventoryItem.MetalCode;
			MetalCodeButton.TitleLabel.Text = metalButtonText;
			MetalCodeButton.SetTitle(metalButtonText, UIControlState.Normal);
		}
		public void SetMetalCode(string metal)
		{
			InventoryItem.MetalCode = metal;
			var metalButtonText = String.IsNullOrWhiteSpace(InventoryItem.MetalCode) ? "Metal Code" : InventoryItem.MetalCode;
			MetalCodeButton.TitleLabel.Text = metalButtonText;
			MetalCodeButton.SetTitle(metalButtonText, UIControlState.Normal);
		}

		public void SetupFields()
		{
			if (!IsNew)
			{
				imagesButton = UIButton.FromType(UIButtonType.Custom);
				imagesButton.Frame = new CoreGraphics.CGRect(5, 5, 200, 30);  // changed 
				imagesButton.TitleLabel.Text = "Images";
				View.Add(imagesButton);
			}

			GLCostTextField.Text = Reverse(InventoryItem.GlCost.ToString());
			ItemCodeTextField.Text = InventoryItem.GlItemCode;
			this.View.UserInteractionEnabled = true;
			SetMetalCodeButtonText();
			Secret.Text = InventoryItem.SecretCode;
			TagPrice.Enabled = true;
			TagPrice.UserInteractionEnabled = false;
			//TagPrice.ShouldChangeCharacters += (field, range, replacementString) => false;
			TagPrice.Text = Math.Ceiling(InventoryItem.TagPrice).ToString("C");
			//TagPrice.AddTarget(this, new Selector("tagPrice"), UIControlEvent.EditingDidBegin);

			Sku.Text = InventoryItem.Sku;
			//InvoiceDescriptionTextField.Text = String.IsNullOrWhiteSpace(InventoryItem.InvoiceDescription) ? InventoryItem.DisplayDescription : InventoryItem.InvoiceDescription;
			//InvoiceSalePriceTextField.Text = InventoryItem.SalePrice == 0 ? Math.Ceiling(InventoryItem.TagPrice).ToString("C") : Math.Ceiling(InventoryItem.SalePrice).ToString("C");
				
			InfoLine1.Text = InventoryItem.Info1;
			InfoLine2.Text = InventoryItem.Info2;
			InfoLine3.Text = InventoryItem.Info3;

			SoldSwitch.On = InventoryItem.Sold;
			ShowInStoreSwitch.On = InventoryItem.ShowInStore;
			LastUpdatedField.Text = InventoryItem.LastCheckedDate == null ? "No Check Date" : String.Format("Last Checked On {0}", InventoryItem.LastCheckedDate);

			SetLocationButtonText();

			//img_UploadImage.SetImage(InventoryItem.ThumbnailImage, UIControlState.Normal);//UIImage.FromFile("UploadLocal.png");

			if (InventoryItem.Sold)
				SoldSwitch.Enabled = false;
		}

		double salePrice;

		partial void ImagesButton_TouchUpInside(UIButton sender)
		{
			if (IsNew)
			{
				var alert = UIAlertController.Create("Cannot Add Images To New Items", "Item must be saved before adding images.", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert, animated: true, completionHandler: null);
				return;
			}
			if (!(!String.IsNullOrWhiteSpace(Sku.Text) && Sku.Text.Length == 6))
			{
				var alert = UIAlertController.Create("Invalid Sku", "A valid SKU must be input before adding images. SKU must be 6 digits long", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert, animated: true, completionHandler: null);
				return;

			}
			if (String.IsNullOrWhiteSpace(InventoryItem.Id))
			{
				var alert = UIAlertController.Create("Save Item First", "Item must be saved before adding images", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert, animated: true, completionHandler: null);
				return;
			}
			LoadignImagesViewController limvc = this.Storyboard.InstantiateViewController("LoadignImagesViewController") as LoadignImagesViewController;


			limvc.InventoryItem = InventoryItem;

			this.NavigationController.PushViewController(limvc, true);
		}

		void OnUpdateSalePrice()
		{
			salePrice = Double.Parse(alertWithTextBox?.TextFields[0]?.Text);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			//UpdateItem();
		}

		public void ShowUpdatingOverlay()
		{
			var bounds = UIScreen.MainScreen.Bounds;
			updatingOverlay = new UpdatingOverlay(bounds);
			View.Add(updatingOverlay);
		}

		public void HideUpdatingOverlay()
		{
			updatingOverlay.Hide();
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
		private void ShowLoading()
		{
			this.NavigationItem.HidesBackButton = true;
			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
			View.Add(loadingOverlay);
		}

		private void HideLoading()
		{
			loadingOverlay.Hide();
			this.NavigationItem.HidesBackButton = false;
		}
		void SaveItem(object sender, EventArgs args)
		{
			//RMR need to show saving load here
			if (!(!String.IsNullOrWhiteSpace(Sku.Text) && Sku.Text.Length == 6))
			{
				var alert = UIAlertController.Create("Invalid Sku", "SKU must be 6 digits long", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert, animated: true, completionHandler: null);
				return;

			}
			performSave();
		}

		partial void ShopifyButton_TouchUpInside(UIButton sender)
		{
            if (!(!String.IsNullOrWhiteSpace(Sku.Text) && Sku.Text.Length == 6))
			{
				var alert = UIAlertController.Create("Invalid Sku", "A valid SKU must be input before editing Shopify Details. SKU must be 6 digits long", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert, animated: true, completionHandler: null);
				return;

			}
            if(String.IsNullOrWhiteSpace(InventoryItem.Id))
            {
				var alert = UIAlertController.Create("Save Item First", "Item must be saved before editing shopify details", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert, animated: true, completionHandler: null);
				return;
            }
			ShopifyProdcutDetailsVIewController spdvc = this.Storyboard.InstantiateViewController("ShopifyProdcutDetailsVIewController") as ShopifyProdcutDetailsVIewController;
			saveFieldsToLocalItem();
			spdvc.InventoryItem = InventoryItem;

			this.NavigationController.PushViewController(spdvc, true);
		}
		private void saveFieldsToLocalItem()
		{
			InventoryItem.GlItemCode = ItemCodeTextField.Text;
			InventoryItem.MetalCode = MetalCodeButton.TitleLabel.Text == "Metal Code" ? "" : MetalCodeButton.TitleLabel.Text;
			InventoryItem.SecretCode = Secret.Text;

			InventoryItem.TagPrice = Double.Parse(TagPrice.Text.Replace("$", ""));
			InventoryItem.Sku = Sku.Text;

			InventoryItem.Info1 = InfoLine1.Text;
			InventoryItem.Info2 = InfoLine2.Text;
			InventoryItem.Info3 = InfoLine3.Text;

			InventoryItem.Sold = SoldSwitch.On;

			InventoryItem.ShowInStore = ShowInStoreSwitch.On;

			InventoryItem.Location = LocationButton.TitleLabel.Text == "Location" ? "" : LocationButton.TitleLabel.Text;

		}
		private bool performSave()
		{
			ShowSavingOverlay();
			if (this.NavigationItem.LeftBarButtonItem != null)
				this.NavigationItem.LeftBarButtonItem.Enabled = false;				
			else
				this.NavigationItem.HidesBackButton = true;

			
			this.NavigationItem.RightBarButtonItem.Enabled = false;

			saveFieldsToLocalItem();

			var item = InventoryItem.ConvertToInventory();
			item.LastCheckedDate = DateTime.UtcNow;
			InventoryItem.LastCheckedDate = DateTime.Now.ToString("d");

			var savedItem = AzureService.InventoryService.InsertOrSave(item);
			if (savedItem == null)
				return false;
			InventoryItem = new InventoryNS(savedItem);
			SetMetalCodeButtonText();
			SetLocationButtonText();

			this.NavigationItem.RightBarButtonItem.Enabled = true;
			if (this.NavigationItem.LeftBarButtonItem != null)
				this.NavigationItem.LeftBarButtonItem.Enabled = true;
			else
				this.NavigationItem.HidesBackButton = false;

			SetupFields();
			HideSavingOverlay();
			IsNew = false;

			return true;
		}
	
		List<ProductImage> productImageList = new List<ProductImage>();

		private bool TextFieldShouldReturn(UITextField textfield)
		{
			nint nextTag = (textfield.Tag + 1) % 5;
			switch (nextTag)
			{
				case 0:
					GLCostTextField.BecomeFirstResponder();
					break;
				case 1:
					Secret.BecomeFirstResponder();
					break;
				case 2:
					InfoLine1.BecomeFirstResponder();
					break;
				case 3:
					InfoLine2.BecomeFirstResponder();
					break;
				case 4:
					InfoLine3.BecomeFirstResponder();
					break;
				default:
					textfield.ResignFirstResponder();
					HideKeyboard();
					break;
			}

			ResetTheScroller();
			return false;
		}
		private void KeyBoardUpNotification(NSNotification notification)
		{
			SetScrollBasedOnField();
		}
		private void SetScrollBasedOnField()
		{
			if ((GLCostTextField != null && GLCostTextField.IsFirstResponder) || 
			    (ItemCodeTextField != null && ItemCodeTextField.IsFirstResponder) || 
			    (Secret != null && Secret.IsFirstResponder))
			{
				SetScrollHeightForEditing(0);
				AddKeyboardTapOutGester();
				return;
			}
			if (!_AlertOpen)
			{

				SetScrollHeightForEditing(-210);
				AddKeyboardTapOutGester();
			}
			else
			{
				SetScrollHeightForEditing(-145);

			}
		}
		private void KeyBoardDownNotification(NSNotification notification)
		{
			//if(moveViewUp){ScrollTheView(false);}
			ResetTheScroller();
		}
		private void ResetTheScroller()
		{
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CoreGraphics.CGRect frame = View.Frame;
			frame.Y = 0;
			scrollamount = 0;

			View.Frame = frame;
			UIView.CommitAnimations();
		}
		private void SetScrollHeightForEditing(int yValue)
		{
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CoreGraphics.CGRect frame = View.Frame;
			frame.Y = yValue;
			scrollamount = 0;

			View.Frame = frame;
			UIView.CommitAnimations();
		}
		private void ScrollTheView(bool move)
		{

			// scroll the view up or down
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			CoreGraphics.CGRect frame = View.Frame;
			frame.Y = 0;
			if (move)
			{
				frame.Y -= scrollamount;
			}
			else {
				frame.Y += scrollamount;
				scrollamount = 0;
			}

			View.Frame = frame;
			UIView.CommitAnimations();
		}

		public void HideKeyboard()
		{
			// Find what opened the keyboard
			foreach (UIView view in this.View.Subviews)
			{
				if (view.IsFirstResponder)
				{
					view.ResignFirstResponder();
				}

			}
			RemoveAllViewGestures();
		}
		private void AddKeyboardTapOutGester()
		{
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer();
			tapGesture.AddTarget(() => HideKeyboard());
			tapGesture.ShouldReceiveTouch += (recongizer, touch) => !(touch.View is CustomAlert || touch.View is NewCodeAlert || touch.View is CustomAlertDetailPage);

			this.View.AddGestureRecognizer(tapGesture);
		}
		private void RemoveAllViewGestures()
		{
			this.View.GestureRecognizers = null;
		}


		partial void GLCost_EditingDidBegin(UIKit.UITextField sender)
		{
			SetScrollBasedOnField();
			
		}
		partial void GLCostDidEndEditing(UITextField sender)
		{

			InventoryItem.GlCost = String.IsNullOrWhiteSpace(GLCostTextField.Text) ? 0.00 : Double.Parse(GLCostTextField.Text);
			UpdateGLShortCode();
			InventoryItem.SetTagPrice();
			TagPrice.Text = Math.Ceiling(InventoryItem.TagPrice).ToString("C");

			reservseGLCostTextField();
			costSwapped = true;
			HideKeyboard();
			Secret.BecomeFirstResponder();


		}

		private void reservseGLCostTextField()
		{
			GLCostTextField.Text = Reverse(GLCostTextField.Text);
		}
		public static string Reverse(string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
		public void UpdateGLShortCode()
		{
			InventoryItem.SetGLShortCode();
			//GLShortCodeTextField.Text = InventoryItem.GlShortCode;
		}



		partial void TouchDownGLCost(UITextField sender)
		{
			if (costSwapped)
			{
				reservseGLCostTextField();
				costSwapped = false;
				GLCostTextField.SelectAll(sender);
			}
		}
		//public void UpdateCodeButtonText()
		//{
		//	var metalButtonText = String.IsNullOrWhiteSpace(ItemCodeTextField.Text) ? "IC" : ItemCodeTextField.Text;
		//	MetalCodeButton.TitleLabel.Text = metalButtonText;
		//	MetalCodeButton.SetTitle(metalButtonText, UIControlState.Normal);
		//}
		public void UpdateCode(string code)
		{
			ItemCodeTextField.Text = code;
			InventoryItem.GlItemCode = code;
			UpdateGLShortCode();
			Secret.BecomeFirstResponder();

		}
		partial void ItemCodeButton_TouchUpInside(UIButton sender)
		{

			HideKeyboard();
			_AlertOpen = true;
			View.AddSubview(Alert);

		}


		partial void MetalCodeButton_TouchUpInside(UIButton sender)
		{

			HideKeyboard();
			View.AddSubview(MetalAlert);
		}
		partial void LocationButton_TouchUpInside(UIButton sender)
		{

			HideKeyboard();
			View.AddSubview(LocationAlert);
		}

		//Printing
		SFBarcode barcode;
		CGRect frameRect = new CGRect();

		partial void PrintTag_TouchUpInside(UIButton sender)
		{
			if (IsNew)
			{
				var alert = UIAlertController.Create("Cannot Print New Items", "Item must be saved before printing tags.", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert, animated: true, completionHandler: null);
				return;
			}
			//Add this back in once save is working RMR			var saved = performSave().Result;
			LoadBarcode();
			//PrintTagWtihRender();
		}
		void LoadBarcode()
		{
			PdfDocument pdfdoc = new PdfDocument();
			pdfdoc.PageSettings = new PdfPageSettings(new Syncfusion.Drawing.SizeF(200, 78f));
			PdfPage page = pdfdoc.Pages.Add();
			PdfCode128CBarcode barcodepdf = new PdfCode128CBarcode(InventoryItem.Sku == null ? "" : InventoryItem.Sku);
			barcodepdf.TextDisplayLocation = TextLocation.None;
			barcodepdf.BarHeight = 13f;
			barcodepdf.NarrowBarWidth = 0.48f;

			float startingX = 115f;
			float lineHeight = 7f;
			float nextLineY = 0f;
			barcodepdf.Draw(page, new Syncfusion.Drawing.PointF(startingX, nextLineY));

			PdfGraphics graphics = page.Graphics;

			//Create a solid brush

			PdfBrush brush = new PdfSolidBrush(Color.FromArgb(255, 0, 0, 0));

			//Set the font

			PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8, PdfFontStyle.Bold);

			//Draw the text

			graphics.DrawString(InventoryItem.GlShortCode, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += 13f));
			graphics.DrawString(InventoryItem.SecretCode == null ? "" : InventoryItem.SecretCode, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
			graphics.DrawString((NSString)InventoryItem.TagPrice.ToString("C"), font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));

			//add space for fold
			nextLineY += 2;

			graphics.DrawString(InventoryItem.MetalCode, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
			graphics.DrawString(InventoryItem.Info1 == null ? "" : InventoryItem.Info1, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
			graphics.DrawString(InventoryItem.Info2 == null ? "" : InventoryItem.Info2, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
			graphics.DrawString(InventoryItem.Info3 == null ? "" : InventoryItem.Info3, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));
			graphics.DrawString(InventoryItem.Sku == null ? "" : InventoryItem.Sku, font, brush, new Syncfusion.Drawing.PointF(startingX, nextLineY += lineHeight));

			//graphics.DrawRectangle (new PdfPen(Color.Black), new Syncfusion.Drawing.RectangleF (0f, 0f, 134f, 67f));
			MemoryStream stream = new MemoryStream();
			pdfdoc.Save(stream);
			pdfdoc.Close(true);

			Save("DataGrid.pdf", "application/pdf", stream);
			//barcode.Draw (frameRect);
			//this.AddSubview (barcode);

		}

		private void Save(string filename, string contentType, MemoryStream stream)
		{
			string exception = string.Empty;
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string filePath = Path.Combine(path, filename);
			try
			{
				FileStream fileStream = File.Open(filePath, FileMode.Create);
				stream.Position = 0;
				stream.CopyTo(fileStream);
				fileStream.Flush();
				fileStream.Close();
			}
			catch (Exception e)
			{
				exception = e.ToString();
			}
			if (contentType == "application/html" || exception != string.Empty)
				return;
			UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			while (currentController.PresentedViewController != null)
				currentController = currentController.PresentedViewController;
			UIView currentView = currentController.View;

			QLPreviewController qlPreview = new QLPreviewController();
			QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);
			qlPreview.DataSource = new PreviewControllerDS(item);

			currentController.PresentViewController((UIViewController)qlPreview, true, (Action)null);
		}
		public void PrintTagWtihRender()
		{
			// Get a reference to the singleton iOS printing concierge
			UIPrintInteractionController printController = UIPrintInteractionController.SharedPrintController;
			printController.Delegate = new TagPrintDelegate();//This delegate always selects the smallest paper size

			// Instruct the printing concierge to use our custom UIPrintPageRenderer subclass when printing this job
			TapPrintPageRenderer tpr = new TapPrintPageRenderer();
			tpr.SetupData(InventoryItem);
			printController.PrintPageRenderer = tpr;

			// Ask for a print job object and configure its settings to tailor the print request
			UIPrintInfo info = UIPrintInfo.PrintInfo;

			// B&W or color, normal quality output for mixed text, graphics, and images
			info.OutputType = UIPrintInfoOutputType.General;

			// Select the job named this in the printer queue to cancel our print request.
			info.JobName = "TagPrint";

			// Instruct the printing concierge to use our custom print job settings.
			printController.PrintInfo = info;

			// Present the standard iOS Print Panel that allows you to pick the target Printer, number of pages, double-sided, etc.
			printController.Present(true, PrintingCompleted);
		}


		void PrintingCompleted(UIPrintInteractionController controller, bool completed, NSError error)
		{
			if (completed != true || error != null)
			{
				Console.WriteLine(String.Format("Failed to print Completed: {0} \n NSError: {1}", completed.ToString(), error != null ? error.ToString() : "none"));
			}
		}
		private bool haveChangedBeenMade()
		{
			if (InventoryItem.GlItemCode != ItemCodeTextField.Text ||
				InventoryItem.MetalCode != MetalCodeButton.TitleLabel.Text ||
				InventoryItem.SecretCode != Secret.Text ||
			    InventoryItem.TagPrice != Double.Parse(TagPrice.Text.Replace("$", "")) ||
				InventoryItem.Sku != Sku.Text ||
				InventoryItem.Info1 != InfoLine1.Text ||
				InventoryItem.Info2 != InfoLine2.Text ||
				InventoryItem.Info3 != InfoLine3.Text ||
				InventoryItem.Sold != SoldSwitch.On ||
			    InventoryItem.ShowInStore != ShowInStoreSwitch.On ||
				InventoryItem.Location != LocationButton.TitleLabel.Text)
			{
				return true;
			}
			return false;
		}

	}

	public class PreviewControllerDS : QLPreviewControllerDataSource
	{
		private QLPreviewItem _item;

		public PreviewControllerDS(QLPreviewItem item)
		{
			_item = item;
		}

		public override nint PreviewItemCount(QLPreviewController controller)
		{
			return (nint)1;
		}

		public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
		{
			return _item;
		}
	}

	public class QLPreviewItemBundle : QLPreviewItem
	{
		string _fileName, _filePath;
		public QLPreviewItemBundle(string fileName, string filePath)
		{
			_fileName = fileName;
			_filePath = filePath;
		}

		public override string ItemTitle
		{
			get
			{
				return _fileName;
			}
		}
		public override NSUrl ItemUrl
		{
			get
			{
				var documents = NSBundle.MainBundle.BundlePath;
				var lib = Path.Combine(documents, _filePath);
				var url = NSUrl.FromFilename(lib);
				return url;
			}
		}
	}

	public partial class TagPrintDelegate : UIPrintInteractionControllerDelegate
	{
		public override UIPrintPaper ChoosePaper(UIPrintInteractionController printInteractionController, UIPrintPaper[] paperList)
		{
			if (paperList.Count() > 0)
			{
				nfloat min = paperList.Min(entry => entry.PaperSize.Width);
				var lowestValues = paperList.Where(entry => entry.PaperSize.Width == min);
				return lowestValues.First();
			}
			return null;
		}
	}
}