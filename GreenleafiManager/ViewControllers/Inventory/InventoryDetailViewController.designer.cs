// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace GreenleafiManager
{
	[Register ("InventoryDetailViewController")]
	partial class InventoryDetailViewController
	{
		[Outlet]
		UIKit.UIButton AddToInvoice { get; set; }

		[Outlet]
		UIKit.UITextField GLCostTextField { get; set; }

		[Outlet]
		UIKit.UITextField InfoLine1 { get; set; }

		[Outlet]
		UIKit.UITextField InfoLine2 { get; set; }

		[Outlet]
		UIKit.UITextField InfoLine3 { get; set; }

		[Outlet]
		UIKit.UIScrollView InventoryScrollview { get; set; }

		[Outlet]
		UIKit.UIScrollView InventScrollView { get; set; }

		[Outlet]
		UIKit.UIButton ItemCodeButton { get; set; }

		[Outlet]
		UIKit.UITextField ItemCodeTextField { get; set; }

		[Outlet]
		UIKit.UILabel LblShowInStore { get; set; }

		[Outlet]
		UIKit.UIButton LocationButton { get; set; }

		[Outlet]
		UIKit.UIButton MetalCodeButton { get; set; }

		[Outlet]
		UIKit.UIButton PrintTag { get; set; }

		[Outlet]
		UIKit.UIButton ReplaceImage { get; set; }

		[Outlet]
		UIKit.UITextField Secret { get; set; }

		[Outlet]
		UIKit.UIButton ShopifyButton { get; set; }

		[Outlet]
		UIKit.UITextField Sku { get; set; }

		[Outlet]
		UIKit.UISwitch SoldSwitch { get; set; }

		[Outlet]
		UIKit.UISwitch SwichShowInStore { get; set; }

		[Outlet]
		UIKit.UITextField TagPrice { get; set; }

		[Action ("AddToInvoice_TouchUpInside:")]
		partial void AddToInvoice_TouchUpInside (UIKit.UIButton sender);

		[Action ("GLCostDidEndEditing:")]
		partial void GLCostDidEndEditing (UIKit.UITextField sender);

		[Action ("GLCostDidEndOnEx:")]
		partial void GLCostDidEndOnEx (UIKit.UITextField sender);

		[Action ("ItemCodeButton_TouchUpInside:")]
		partial void ItemCodeButton_TouchUpInside (UIKit.UIButton sender);

		[Action ("LocationButton_TouchUpInside:")]
		partial void LocationButton_TouchUpInside (UIKit.UIButton sender);

		[Action ("MetalCodeButton_TouchUpInside:")]
		partial void MetalCodeButton_TouchUpInside (UIKit.UIButton sender);

		[Action ("PrintTag_TouchUpInside:")]
		partial void PrintTag_TouchUpInside (UIKit.UIButton sender);

		[Action ("ShopifyButton_TouchUpInside:")]
		partial void ShopifyButton_TouchUpInside (Foundation.NSObject sender);

		[Action ("TouchDownGLCost:")]
		partial void TouchDownGLCost (UIKit.UITextField sender);

		[Action ("UIButton538_TouchUpInside:")]
		partial void UIButton538_TouchUpInside (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddToInvoice != null) {
				AddToInvoice.Dispose ();
				AddToInvoice = null;
			}

			if (GLCostTextField != null) {
				GLCostTextField.Dispose ();
				GLCostTextField = null;
			}

			if (InfoLine1 != null) {
				InfoLine1.Dispose ();
				InfoLine1 = null;
			}

			if (InfoLine2 != null) {
				InfoLine2.Dispose ();
				InfoLine2 = null;
			}

			if (InfoLine3 != null) {
				InfoLine3.Dispose ();
				InfoLine3 = null;
			}

			if (InventoryScrollview != null) {
				InventoryScrollview.Dispose ();
				InventoryScrollview = null;
			}

			if (InventScrollView != null) {
				InventScrollView.Dispose ();
				InventScrollView = null;
			}

			if (ItemCodeButton != null) {
				ItemCodeButton.Dispose ();
				ItemCodeButton = null;
			}

			if (ItemCodeTextField != null) {
				ItemCodeTextField.Dispose ();
				ItemCodeTextField = null;
			}

			if (LblShowInStore != null) {
				LblShowInStore.Dispose ();
				LblShowInStore = null;
			}

			if (LocationButton != null) {
				LocationButton.Dispose ();
				LocationButton = null;
			}

			if (MetalCodeButton != null) {
				MetalCodeButton.Dispose ();
				MetalCodeButton = null;
			}

			if (PrintTag != null) {
				PrintTag.Dispose ();
				PrintTag = null;
			}

			if (ReplaceImage != null) {
				ReplaceImage.Dispose ();
				ReplaceImage = null;
			}

			if (Secret != null) {
				Secret.Dispose ();
				Secret = null;
			}

			if (ShopifyButton != null) {
				ShopifyButton.Dispose ();
				ShopifyButton = null;
			}

			if (Sku != null) {
				Sku.Dispose ();
				Sku = null;
			}

			if (SoldSwitch != null) {
				SoldSwitch.Dispose ();
				SoldSwitch = null;
			}

			if (SwichShowInStore != null) {
				SwichShowInStore.Dispose ();
				SwichShowInStore = null;
			}

			if (TagPrice != null) {
				TagPrice.Dispose ();
				TagPrice = null;
			}
		}
	}
}
