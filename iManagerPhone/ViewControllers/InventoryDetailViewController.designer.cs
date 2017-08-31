// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GreenleafiManagerPhone
{
    [Register ("InventoryDetailViewController")]
    partial class InventoryDetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField GLCostTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ImagesButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField InfoLine1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField InfoLine2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField InfoLine3 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ItemCodeTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LastUpdatedField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LocationButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MetalCodeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Secret { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ShopifyButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ShowInStoreSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField Sku { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch SoldSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TagPrice { get; set; }

        [Action ("GLCost_EditingDidBegin:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GLCost_EditingDidBegin (UIKit.UITextField sender);

        [Action ("GLCostDidEndEditing:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GLCostDidEndEditing (UIKit.UITextField sender);

        [Action ("ImagesButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ImagesButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("ItemCodeButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ItemCodeButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("LocationButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LocationButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("MetalCodeButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MetalCodeButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("PrintTag_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PrintTag_TouchUpInside (UIKit.UIButton sender);

        [Action ("ShopifyButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShopifyButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("TouchDownGLCost:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TouchDownGLCost (UIKit.UITextField sender);

        void ReleaseDesignerOutlets ()
        {
            if (GLCostTextField != null) {
                GLCostTextField.Dispose ();
                GLCostTextField = null;
            }

            if (ImagesButton != null) {
                ImagesButton.Dispose ();
                ImagesButton = null;
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

            if (ItemCodeTextField != null) {
                ItemCodeTextField.Dispose ();
                ItemCodeTextField = null;
            }

            if (LastUpdatedField != null) {
                LastUpdatedField.Dispose ();
                LastUpdatedField = null;
            }

            if (LocationButton != null) {
                LocationButton.Dispose ();
                LocationButton = null;
            }

            if (MetalCodeButton != null) {
                MetalCodeButton.Dispose ();
                MetalCodeButton = null;
            }

            if (Secret != null) {
                Secret.Dispose ();
                Secret = null;
            }

            if (ShopifyButton != null) {
                ShopifyButton.Dispose ();
                ShopifyButton = null;
            }

            if (ShowInStoreSwitch != null) {
                ShowInStoreSwitch.Dispose ();
                ShowInStoreSwitch = null;
            }

            if (Sku != null) {
                Sku.Dispose ();
                Sku = null;
            }

            if (SoldSwitch != null) {
                SoldSwitch.Dispose ();
                SoldSwitch = null;
            }

            if (TagPrice != null) {
                TagPrice.Dispose ();
                TagPrice = null;
            }
        }
    }
}