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
	[Register ("MenuViewController")]
	partial class MenuViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton CustomersButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton InventoryButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton InvoicesButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton LocationsButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Reports { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Users { get; set; }

		[Action ("CustomersButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void CustomersButton_TouchUpInside (UIButton sender);

		[Action ("InventoryButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void InventoryButton_TouchUpInside (UIButton sender);

		[Action ("InvoicesButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void InvoicesButton_TouchUpInside (UIButton sender);

		[Action ("LocationsButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void LocationsButton_TouchUpInside (UIButton sender);

		[Action ("Reports_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Reports_TouchUpInside (UIButton sender);

		[Action ("Users_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Users_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (CustomersButton != null) {
				CustomersButton.Dispose ();
				CustomersButton = null;
			}
			if (InventoryButton != null) {
				InventoryButton.Dispose ();
				InventoryButton = null;
			}
			if (InvoicesButton != null) {
				InvoicesButton.Dispose ();
				InvoicesButton = null;
			}
			if (LocationsButton != null) {
				LocationsButton.Dispose ();
				LocationsButton = null;
			}
			if (Reports != null) {
				Reports.Dispose ();
				Reports = null;
			}
			if (Users != null) {
				Users.Dispose ();
				Users = null;
			}
		}
	}
}
