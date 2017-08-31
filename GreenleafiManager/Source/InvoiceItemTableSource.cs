using System;
using UIKit;
using System.Collections.Generic;
using Foundation;

namespace GreenleafiManager
{
	public class InvoiceItemTableSource : UITableViewSource
	{

		List<Item> tableItems = new List<Item>();
		UIViewController controller;
		bool IsEdit;
		public InvoiceItemTableSource(List<Item> _tableItems, UIViewController _controller, bool _IsEdit = false)
		{
			tableItems = _tableItems;
			controller = _controller;
			IsEdit = _IsEdit;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return tableItems.Count;
		}

		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// BaseController.NavController.PushViewController(new AlertDetailViewController(tableItems[indexPath.Row]), true);
		}

		public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			NSString cellID = new NSString(string.Format("{0}cell", "InvoiceItemCell"));
			InvoiceItemCell cell = (InvoiceItemCell)tableView.DequeueReusableCell(cellID);
			if (cell == null)
			{
				cell = new InvoiceItemCell(tableView, controller, IsEdit);
			}
			cell.UpdateCell(tableItems[indexPath.Row]);
			return cell;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 30f;
		}
	}
}