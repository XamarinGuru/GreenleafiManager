// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace GreenleafiManagerPhone
{
    [Register ("ShopifyProdcutDetailsVIewController")]
    partial class ShopifyProdcutDetailsVIewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView ShopifyDescriptionTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ShopifyPriceTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ShopifyTitleTextField { get; set; }

        [Action ("ResetShopifyDataTouchUp:")]
        partial void ResetShopifyDataTouchUp (Foundation.NSObject sender);
        
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
