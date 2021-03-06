﻿using System;
using UIKit;
using System.Collections.Generic;
using Foundation;

namespace GreenleafiManager
{
	public class InvoicePaymentTableSource : UITableViewSource
	{

		List<Payment> tableItems = new List<Payment>();
		UIViewController controller;

		public InvoicePaymentTableSource(List<Payment> _tableItems, UIViewController _controller)
		{
			tableItems = _tableItems;
			controller = _controller;
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
			NSString cellID = new NSString(string.Format("{0}cell", "InvoicePaymentCell"));
			InvoicePaymentCell cell = (InvoicePaymentCell)tableView.DequeueReusableCell(cellID);
			if (cell == null)
			{
				cell = new InvoicePaymentCell(tableView,controller);
			}
			cell.UpdateCell(tableItems[indexPath.Row]);
			return cell;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 50f;
		}
	}
}