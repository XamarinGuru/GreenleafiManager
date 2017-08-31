using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace GreenleafiManager
{
	public class InvoicePaymentCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("InvoicePaymentCell");
		public static readonly UINib Nib;
		public UIView seprator;
		UITableView tblView;
		UIViewController controller;
		public InvoicePaymentCell(UITableView _tblView, UIViewController _controller)
		{
			tblView = _tblView;
			controller = _controller;
		}

		public void UpdateCell(Payment objItem)
		{
			nfloat vFullWidth = tblView.Frame.Width;
			var label1 = new UILabel(new CGRect(0, 0, vFullWidth / 4 - 10, 40));
			label1.Text = objItem.PaidAmount.ToString("C");
			label1.ToGreenLeafLabel();
			ContentView.Add(label1);

			var label2 = new UILabel(new CGRect(label1.Frame.X + label1.Frame.Width + 10, 0, vFullWidth / 4 - 10, 40));
			label2.Text = objItem.PaymentType?.Name;
			label2.ToGreenLeafLabel();
			ContentView.Add(label2);

			var label3 = new UILabel(new CGRect(label2.Frame.X + label2.Frame.Width + 10, 0, vFullWidth / 6, 40));
			label3.Text = objItem.ApprovalCode;
			label3.ToGreenLeafLabel();
			ContentView.Add(label3);

			var label4 = new UILabel(new CGRect(label3.Frame.X + label3.Frame.Width + 10, 0, vFullWidth / 5 - 10, 40));
			label4.Text = objItem.Date;
			label4.ToGreenLeafLabel();
			ContentView.Add(label4);

			var btnCross = new UIButton(new CGRect(label4.Frame.X + label4.Frame.Width + 10, 0, 40, 40));
			btnCross.SetTitle("X", UIControlState.Normal);
			btnCross.Layer.BorderWidth = 1;
			btnCross.Layer.BorderColor = UIColor.Gray.CGColor;
			btnCross.SetTitleColor(UIColor.Black, UIControlState.Normal);
			btnCross.TouchUpInside += ((sender, e) =>
			{
				UIAlertController alert = UIAlertController.Create("Delete payment from invoice?", "Are you sure you want to remove this payment from this invoice?" +
																	   "Hit the save button to save changes.", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
				{
				}));
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
				{
					//DJ remove payment
					objItem.Add = false;
					objItem.Update = false;
					objItem.Remove = true;
					((InvoiceEditViewController)controller).paymentListToRemove.Add(objItem);
					((InvoiceEditViewController)controller).paymentList.Remove(objItem);
					tblView.Source = new InvoicePaymentTableSource(((InvoiceEditViewController)controller).paymentList, controller);
					tblView.ReloadData();
					((InvoiceEditViewController)controller).SetupAmounts();
				}));
				((InvoiceEditViewController)controller).PresentViewController(alert, true, null);

			});
			ContentView.Add(btnCross);
		}

	}
}
