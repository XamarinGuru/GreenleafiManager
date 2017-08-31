using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace GreenleafiManager
{
	public class InvoiceDetailPaymentCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("InvoiceDetailPaymentCell");
		public static readonly UINib Nib;
		public UIView seprator;
		UITableView tblView;
		UIViewController controller;
		public InvoiceDetailPaymentCell(UITableView _tblView)
		{
			tblView = _tblView;
			//controller = _controller;
		}

		public void UpdateCell(Payment objItem)
		{
			nfloat vFullWidth = tblView.Frame.Width;
			var label1 = new UILabel(new CGRect(0, 0, vFullWidth / 3, 40));
			label1.Text = objItem.PaidAmount.ToString("C");
			ContentView.Add(label1);

			var label2 = new UILabel(new CGRect(label1.Frame.X + label1.Frame.Width, 0, vFullWidth / 3, 40));
			label2.Text = objItem.PaymentType?.Name;
			ContentView.Add(label2);

			var label3 = new UILabel(new CGRect(label2.Frame.X + label2.Frame.Width, 0, vFullWidth / 3, 40));
			label3.Text = objItem.Date;
			ContentView.Add(label3);

			var seprator = new UIView(new CGRect(0, 40, vFullWidth, 1));
			seprator.BackgroundColor = UIColor.Gray;
			ContentView.Add(seprator);

		}

	}
}
