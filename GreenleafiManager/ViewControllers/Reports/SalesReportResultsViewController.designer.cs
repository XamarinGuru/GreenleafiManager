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
	[Register ("SalesReportResultsViewController")]
	partial class SalesReportResultsViewController
	{
		[Outlet]
		UIKit.UILabel DateRunLabel { get; set; }

		[Outlet]
		UIKit.UILabel ItemsSoldLabel { get; set; }

		[Outlet]
		UIKit.UILabel LocationLabel { get; set; }

		[Outlet]
		UIKit.UILabel ReportDateLabel { get; set; }

		[Outlet]
		UIKit.UILabel SalesPersonLabel { get; set; }

		[Outlet]
		UIKit.UILabel TotalSalesLabel { get; set; }

		[Outlet]
		UIKit.UILabel TotalSalesWithTaxLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ReportDateLabel != null) {
				ReportDateLabel.Dispose ();
				ReportDateLabel = null;
			}

			if (SalesPersonLabel != null) {
				SalesPersonLabel.Dispose ();
				SalesPersonLabel = null;
			}

			if (LocationLabel != null) {
				LocationLabel.Dispose ();
				LocationLabel = null;
			}

			if (ItemsSoldLabel != null) {
				ItemsSoldLabel.Dispose ();
				ItemsSoldLabel = null;
			}

			if (TotalSalesLabel != null) {
				TotalSalesLabel.Dispose ();
				TotalSalesLabel = null;
			}

			if (TotalSalesWithTaxLabel != null) {
				TotalSalesWithTaxLabel.Dispose ();
				TotalSalesWithTaxLabel = null;
			}

			if (DateRunLabel != null) {
				DateRunLabel.Dispose ();
				DateRunLabel = null;
			}
		}
	}
}
