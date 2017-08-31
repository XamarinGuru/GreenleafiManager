using System;
using CoreGraphics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreFoundation;

using Infragistics;
using Syncfusion.SfBarcode.iOS;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf;

using System.Drawing;

namespace GreenleafiManager
{
	
	public class TapPrintPageRenderer : UIPrintPageRenderer
	{
		static UIFont SystemFont = UIFont.SystemFontOfSize (UIFont.SystemFontSize);
		private InventoryNS _Item;
		// This property must be overriden when doing custom drawing as we are.
		// Since our custom drawing is really only for the borders and we are
		// relying on a series of UIMarkupTextPrintFormatters to display the recipe
		// content, UIKit can calculate the number of pages based on informtation
		// provided by those formatters.
		//
		// Therefore, setup the formatters, and ask super to count the pages.
		// HACK: Changed overridden member int to nint
		public override nint NumberOfPages {
			get {
				SetupPrintFormatters ();
				return 1;//Always print 1 tag base.NumberOfPages;
			}
		}
		public void SetupData(InventoryNS item)
		{
			_Item = item;
			//GetImageFromView ();
			LoadBarcode();
		}
		// Iterate through the recipes setting each of their html representations into
		// a UIMarkupTextPrintFormatter and add that formatter to the printing job.
		void SetupPrintFormatters ()
		{
			nint page = 0;

				// ios9 calls NumberOfPages -> SetupPrintFormatters not from main thread, but UIMarkupTextPrintFormatter is UIKit class (must be accessed from main thread)
			DispatchQueue.MainQueue.DispatchSync (() => {
				
				var formatter = new UIMarkupTextPrintFormatter ("");
				AddPrintFormatter (formatter, page);

			});
		}

		// To intermix custom drawing with the drawing performed by an associated print formatter,
		// this method is called for each print formatter associated with a given page.
		//
		// We do this to intermix/overlay our custom drawing onto the recipe presentation.
		// We draw the upper portion of the recipe presentation by hand (image, title, desc),
		// and the bottom portion is drawn via the UIMarkupTextPrintFormatter.
		public override void DrawPrintFormatterForPage (UIPrintFormatter printFormatter, nint page)
		{
			
				
			//base.DrawPrintFormatterForPage (printFormatter, page);

			NSString metal = new NSString ("14K-Y/G");
			NSString info1 = new NSString ("SLED");
			NSString info2 = new NSString ("info2");
			NSString info3 = new NSString ("info3");
			NSString sku = new NSString ("114077");
			NSString GLCode = new NSString ("GL 111GP");
			NSString secret = new NSString ("1");
			NSString tagprice = new NSString ("$866.00");

			CGRect rect = new CGRect (140f, 145f, 70f, 70f);

			rect.Y += 10f;
			int linespace = 7;
			nfloat fontsize = 7f;
			var newFont = UIFont.FromName ("Helvetica-Bold", fontsize);

			GLCode.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);
			rect.Y += linespace;
			secret.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);
			rect.Y += linespace;
			tagprice.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);

			rect.Y += linespace + 2;//for fold space

			metal.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);
			rect.Y += linespace;
			info1.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);
			rect.Y += linespace;
			info2.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);
			rect.Y += linespace;
			info3.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);
			rect.Y += linespace;
			sku.DrawString (rect, newFont, UILineBreakMode.Clip, UITextAlignment.Left);
		
//			DispatchQueue.MainQueue.DispatchSync (() => {
//				
//				GetImageFromView ();//BarcodeView);
//			});
//			rect = new CGRect(137f, 135f, 60f, 30f);
//			barcodeImage.ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal).Draw(rect);


			frameRect = new CGRect(137f, 135f, 60f, 30f);
			//LoadBarcode ();
			barcode.Draw(frameRect);// UIViewPrintFormatter

			//				using (CGContext context = UIGraphics.GetCurrentContext ()) {
//					
//				UIColor.Black.SetFill ();
//				context.FillRect (rect);
//					IGBarcodeView BarcodeView;
//
//					BarcodeView = IGBarcodeView.CreateBarcode (IGBarcodeType.IGBarcodeTypeCode128);
//					//BarcodeView.BackgroundColor = UIColor.Clear;
//					BarcodeView.SetValue ("114077");
//					BarcodeView.ShowText = false;
//					//BarcodeView.Center = new CGPoint (126f, 180f);
//					BarcodeView.Draw (rect);
//					BarcodeView.Dispose ();
//				}

		}
		private UIImage barcodeImage;
		SFBarcode barcode;
		CGRect frameRect = new CGRect ();
		float frameMargin = 8.0f;

		void LoadBarcode()
		{
			
			barcode = new SFBarcode();
			barcode.BackgroundColor = UIColor.FromRGB (242/255.0f, 242/255.0f, 242/255.0f);
			barcode.Frame = frameRect;
			barcode.Text = (NSString)"092418376";
			barcode.Symbology = SFBarcodeSymbolType.SFBarcodeSymbolTypeCode128C;
			SFCode128CSettings settings = new SFCode128CSettings();
			settings.BarHeight = 20;
			settings.NarrowBarWidth = 1f;

			barcode.SymbologySettings = settings;

			PdfDocument pdfdoc = new PdfDocument ();
			PdfPage page = pdfdoc.Pages.Add ();


			//barcode.Draw (frameRect);
			//this.AddSubview (barcode);
		}



		private void GetImageFromView ()//UIView view)
		{
			IGBarcodeView BarcodeView = IGBarcodeView.CreateBarcode(IGBarcodeType.IGBarcodeTypeCode128);
			BarcodeView.LayoutMargins = new UIEdgeInsets (0, 0, 0, 0);
			BarcodeView.Frame = new CGRect(0f, 0f, 60f, 30f);
			BarcodeView.BackgroundColor = UIColor.Clear;
			BarcodeView.Opaque = false;
			BarcodeView.SetValue("114077");
			BarcodeView.ShowText = false;

			UIGraphics.BeginImageContextWithOptions(BarcodeView.Bounds.Size, BarcodeView.Opaque, 0.0f);
			BarcodeView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
			barcodeImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			//CGRect rect = new CGRect(new CGPoint(70f, 145f), BarcodeView.Bounds.Size);
			//image.Draw(rect);
//
//				using (CGContext context = UIGraphics.GetCurrentContext ()) {
//
//					IGBarcodeView BarcodeView = IGBarcodeView.CreateBarcodeFrame (IGBarcodeType.IGBarcodeTypeCode128, new CGRect (0, 0, 70, 12));
//
//					BarcodeView.BackgroundColor = UIColor.Clear;
//					BarcodeView.SetValue ("114077");
//					BarcodeView.ShowText = false;
//					//BarcodeView.Center = new CGPoint (126f, 180f);
//					UIView view = BarcodeView;
//
//					UIGraphics.BeginImageContextWithOptions (view.Bounds.Size, view.Opaque, 0.0f);
//					view.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
//					barcodeImage = UIGraphics.GetImageFromCurrentImageContext ();
//					UIGraphics.EndImageContext ();
//			
//				}
			//	BarcodeView.BackgroundColor = UIColor.White;
			//	BarcodeView.SetValue ("114077");
			//	BarcodeView.ShowText = false;


		}
	}
}

