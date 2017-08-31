using System;
using System.Collections.Generic;
using Infragistics;
using Foundation;

namespace GreenleafiManager
{
	public class InvoiceGridDataSource : IGGridViewDataSource
	{
		List<Invoice> invoiceItems;
		public InvoiceGridDataSource(List<Invoice> _invoiceItems)
		{
			invoiceItems = _invoiceItems ?? new List<Invoice>();
		}
		public override nint NumberOfRowsInSection(IGGridView gridView, nint section)
		{
			return invoiceItems.Count;
		}

		public override nint NumberOfColumns(IGGridView gridView)
		{
			return 1;
		}

		public override IGGridViewCell CreateCell(IGGridView grid, IGCellPath path)
		{
			IGGridViewCell cell = (NewInvoiceGridCell)grid.DequeueReusableCell("Cell");
			cell = new NewInvoiceGridCell("Cell");
			(cell as NewInvoiceGridCell).UpdateUI(invoiceItems[(int)path.RowIndex]);
			return cell;
		}

	}


	public class InvoiceDelegate : IGGridViewDelegate
	{
		MasterInvoiceViewController parentController;
		IGRowPath _row;
		IGGridView _gridView;


		public InvoiceDelegate(MasterInvoiceViewController parent)
		{
			parentController = parent;

		}

		[Export("gridView:WillSelectRow:")]
		public override IGRowPath WillSelectRow(Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
		{
			MasterInvoiceViewController.ScrollToCell = path.ConvertToCellPath();
			_row = path;
			ShowDetails();
			return null;
		}


		[Export("ShowDetails")]
		public void ShowDetails()
		{
			var InvoiceDetailView = parentController.Storyboard.InstantiateViewController("InvoiceDetailViewController") as InvoiceDetailViewController;
			InvoiceDetailView.objInvoice = MasterInvoiceViewController.InvoiceList[(int)_row.RowIndex];
			parentController.NavigationController.PushViewController(InvoiceDetailView, true);
		}
	}
}
