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
	[Register ("CurrentStockReportResultsViewController")]
	partial class CurrentStockReportResultsViewController
	{
		[Outlet]
		UIKit.UILabel NumberOfItemsLabel { get; set; }

		[Outlet]
		UIKit.UILabel ReportDateFilterLabel { get; set; }

		[Outlet]
		UIKit.UILabel ReportLocationFilterLabel { get; set; }

		[Outlet]
		UIKit.UILabel TotalCostLabel { get; set; }

		[Outlet]
		UIKit.UILabel TotalTagPriceLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ReportDateFilterLabel != null) {
				ReportDateFilterLabel.Dispose ();
				ReportDateFilterLabel = null;
			}

			if (ReportLocationFilterLabel != null) {
				ReportLocationFilterLabel.Dispose ();
				ReportLocationFilterLabel = null;
			}

			if (NumberOfItemsLabel != null) {
				NumberOfItemsLabel.Dispose ();
				NumberOfItemsLabel = null;
			}

			if (TotalTagPriceLabel != null) {
				TotalTagPriceLabel.Dispose ();
				TotalTagPriceLabel = null;
			}

			if (TotalCostLabel != null) {
				TotalCostLabel.Dispose ();
				TotalCostLabel = null;
			}
		}
	}
}
