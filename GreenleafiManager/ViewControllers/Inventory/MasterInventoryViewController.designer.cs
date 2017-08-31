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

namespace GreenleafiManager
{
	[Register ("MasterInventoryViewController")]
	partial class MasterInventoryViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton FilterButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ScanButton { get; set; }

		[Action ("FilterButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void FilterButton_TouchUpInside (UIButton sender);

		[Action ("ScanButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void ScanButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (FilterButton != null) {
				FilterButton.Dispose ();
				FilterButton = null;
			}
			if (ScanButton != null) {
				ScanButton.Dispose ();
				ScanButton = null;
			}
		}
	}
}
