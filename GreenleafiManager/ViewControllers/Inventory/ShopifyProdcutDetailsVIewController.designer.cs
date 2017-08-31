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
	[Register("ShopifyProdcutDetailsVIewController")]
	partial class ShopifyProdcutDetailsVIewController
	{
		[Outlet]
		UIKit.UITextView ShopifyDescriptionTextView { get; set; }

		[Outlet]
		UIKit.UITextField ShopifyPriceTextField { get; set; }

		[Outlet]
		UIKit.UITextField ShopifyTitleTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ShopifyDescriptionTextView != null) {
				ShopifyDescriptionTextView.Dispose ();
				ShopifyDescriptionTextView = null;
			}

			if (ShopifyPriceTextField != null) {
				ShopifyPriceTextField.Dispose ();
				ShopifyPriceTextField = null;
			}

			if (ShopifyTitleTextField != null) {
				ShopifyTitleTextField.Dispose ();
				ShopifyTitleTextField = null;
			}
		}
	}
}
