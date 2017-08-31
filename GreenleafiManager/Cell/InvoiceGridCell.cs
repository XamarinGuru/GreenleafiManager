using System;
using CoreGraphics;
using Foundation;
using Infragistics;
using UIKit;

namespace GreenleafiManager
{
	[Register("InvoiceGridCell", false)]
	public class InvoiceGridCell : IGGridViewCell
	{
		public static Invoice objInvoice;
		public InvoiceGridCell()
		{
			CreateFirstCol();
			CreateSecondCol();
			CreateThirdCol();
		}
		void CreateFirstCol()
		{
			var view = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width / 3, 300));

			var label1 = new UILabel(new CGRect(20, 10, view.Frame.Width - 20, 50));

			label1.Font = UIFont.BoldSystemFontOfSize(20);
			view.Add(label1);

			var label2 = new UILabel(new CGRect(20, 40, view.Frame.Width - 20, 30));
			label2.Text = "INVOICE NUMBER";
			view.InsertSubviewBelow(label2, label1);

			var label3 = new UILabel(new CGRect(20, 70, view.Frame.Width - 20, 50));
			label3.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label3, label2);

			var label4 = new UILabel(new CGRect(20, 100, view.Frame.Width - 20, 30));
			label4.Text = "TOTAL PRICE";
			view.InsertSubviewBelow(label4, label3);

			var label5 = new UILabel(new CGRect(20, 130, view.Frame.Width - 20, 50));
			label5.Text = "John Admin";
			label5.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label5, label4);

			var label6 = new UILabel(new CGRect(20, 160, view.Frame.Width - 20, 30));
			label6.Text = "INVOICE CREATOR";
			view.InsertSubviewBelow(label6, label5);
			this.Add(view);

			if (objInvoice != null)
			{
				label1.Text = objInvoice.Number;
				label3.Text = objInvoice.TotalPrice.ToString("C");
				label5.Text = objInvoice.User?.FullName;
				//label6.Text=objInvoice.UserId;
			}
		}

		void CreateSecondCol()
		{
			var view = new UIView(new CGRect(UIScreen.MainScreen.Bounds.Width / 3, 0, UIScreen.MainScreen.Bounds.Width / 3, 300));

			var label1 = new UILabel(new CGRect(20, 10, view.Frame.Width - 20, 50));
			label1.Font = UIFont.BoldSystemFontOfSize(20);
			view.Add(label1);

			var label2 = new UILabel(new CGRect(20, 40, view.Frame.Width - 20, 30));
			label2.Text = "INVOICE DATE";
			view.InsertSubviewBelow(label2, label1);

			var label3 = new UILabel(new CGRect(20, 70, view.Frame.Width - 20, 50));
			label3.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label3, label2);

			var label4 = new UILabel(new CGRect(20, 100, view.Frame.Width - 20, 30));
			label4.Text = "PAID AMOUNT";
			view.InsertSubviewBelow(label4, label3);

			var label5 = new UILabel(new CGRect(20, 130, view.Frame.Width - 20, 50));

			label5.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label5, label4);

			var label6 = new UILabel(new CGRect(20, 160, view.Frame.Width - 20, 30));
			label6.Text = "CUSTOMER";
			view.InsertSubviewBelow(label6, label5);
			this.Add(view);
			if (objInvoice != null)
			{
				label1.Text = objInvoice.Date;
				label3.Text = objInvoice.PaidAmount.ToString("C");
				if (objInvoice.Customer != null)
					label5.Text = objInvoice.Customer.FirstName + objInvoice.Customer.LastName;

			}
		}

		void CreateThirdCol()
		{
			var view = new UIView(new CGRect(UIScreen.MainScreen.Bounds.Width / 3 * 2, 0, UIScreen.MainScreen.Bounds.Width / 3, 300));

			var label1 = new UILabel(new CGRect(20, 10, view.Frame.Width - 20, 50));
			//label1.Text = "907796";
			label1.Font = UIFont.BoldSystemFontOfSize(20);
			view.Add(label1);



			var label2 = new UILabel(new CGRect(20, 40, view.Frame.Width - 20, 30));
			label2.Text = "STATUS";
			view.InsertSubviewBelow(label2, label1);

			var label3 = new UILabel(new CGRect(20, 70, view.Frame.Width - 20, 50));
			label3.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label3, label2);

			var label4 = new UILabel(new CGRect(20, 100, view.Frame.Width - 20, 30));
			label4.Text = "BALANCE DUE";
			view.InsertSubviewBelow(label4, label3);

			var label5 = new UILabel(new CGRect(20, 130, view.Frame.Width - 20, 50));
			//if (objInvoice.Location != null)
			if (!String.IsNullOrEmpty(objInvoice.LocationName))
				label5.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label5, label4);

			var label6 = new UILabel(new CGRect(20, 160, view.Frame.Width - 20, 30));
			label6.Text = "LOCATION";
			view.InsertSubviewBelow(label6, label5);
			this.Add(view);
			if (objInvoice != null)
			{
				if (objInvoice.IsCancelled == true)
					label1.Text = "Cancelled";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue != 0)
					label1.Text = "Unfinished";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue == 0)
					label1.Text = "Completed";
				else
					label1.Text = "Unfinished";
				label3.Text = objInvoice.BalanceDue.ToString("C");
				label5.Text = objInvoice.LocationName;//?.Name;
			}

		}
	}

	public class NewInvoiceGridCell : IGGridViewCell
	{
		public NewInvoiceGridCell(string identifier) : base(identifier)
		{

		}
		Invoice objInvoice;
		public void UpdateUI(Invoice _objInvoice)
		{
			this.objInvoice = _objInvoice;
			CreateFirstCol();
			CreateSecondCol();
			CreateThirdCol();
		}

		void CreateFirstCol()
		{
			var view = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width / 3, 300));

			var label1 = new UILabel(new CGRect(20, 10, view.Frame.Width - 20, 50));

			label1.Font = UIFont.BoldSystemFontOfSize(20);
			view.Add(label1);

			var label2 = new UILabel(new CGRect(20, 40, view.Frame.Width - 20, 30));
			label2.Text = "INVOICE NUMBER";
			view.InsertSubviewBelow(label2, label1);

			var label3 = new UILabel(new CGRect(20, 70, view.Frame.Width - 20, 50));
			label3.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label3, label2);

			var label4 = new UILabel(new CGRect(20, 100, view.Frame.Width - 20, 30));
			label4.Text = "TOTAL PRICE";
			view.InsertSubviewBelow(label4, label3);

			var label5 = new UILabel(new CGRect(20, 130, view.Frame.Width - 20, 50));
			label5.Text = "John Admin";
			label5.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label5, label4);

			var label6 = new UILabel(new CGRect(20, 160, view.Frame.Width - 20, 30));
			label6.Text = "INVOICE CREATOR";
			view.InsertSubviewBelow(label6, label5);
			this.Add(view);

			if (objInvoice != null)
			{
				label1.Text = objInvoice.Number;
				label3.Text = objInvoice.TotalPrice.ToString("C");
				label5.Text = objInvoice.User?.FullName;
				//label6.Text=objInvoice.UserId;
			}
		}

		void CreateSecondCol()
		{
			var view = new UIView(new CGRect(UIScreen.MainScreen.Bounds.Width / 3, 0, UIScreen.MainScreen.Bounds.Width / 3, 300));

			var label1 = new UILabel(new CGRect(20, 10, view.Frame.Width - 20, 50));
			label1.Font = UIFont.BoldSystemFontOfSize(20);
			view.Add(label1);

			var label2 = new UILabel(new CGRect(20, 40, view.Frame.Width - 20, 30));
			label2.Text = "INVOICE DATE";
			view.InsertSubviewBelow(label2, label1);

			var label3 = new UILabel(new CGRect(20, 70, view.Frame.Width - 20, 50));
			label3.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label3, label2);

			var label4 = new UILabel(new CGRect(20, 100, view.Frame.Width - 20, 30));
			label4.Text = "PAID AMOUNT";
			view.InsertSubviewBelow(label4, label3);

			var label5 = new UILabel(new CGRect(20, 130, view.Frame.Width - 20, 50));

			label5.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label5, label4);

			var label6 = new UILabel(new CGRect(20, 160, view.Frame.Width - 20, 30));
			label6.Text = "CUSTOMER";
			view.InsertSubviewBelow(label6, label5);
			this.Add(view);
			if (objInvoice != null)
			{
				label1.Text = objInvoice.Date;
				label3.Text = objInvoice.PaidAmount.ToString("C");
				if (objInvoice.Customer != null)
					label5.Text = objInvoice.Customer.FirstName + objInvoice.Customer.LastName;

			}
		}

		void CreateThirdCol()
		{
			var view = new UIView(new CGRect(UIScreen.MainScreen.Bounds.Width / 3 * 2, 0, UIScreen.MainScreen.Bounds.Width / 3, 300));

			var label1 = new UILabel(new CGRect(20, 10, view.Frame.Width - 20, 50));
			//label1.Text = "907796";
			label1.Font = UIFont.BoldSystemFontOfSize(20);
			view.Add(label1);



			var label2 = new UILabel(new CGRect(20, 40, view.Frame.Width - 20, 30));
			label2.Text = "STATUS";
			view.InsertSubviewBelow(label2, label1);

			var label3 = new UILabel(new CGRect(20, 70, view.Frame.Width - 20, 50));
			label3.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label3, label2);

			var label4 = new UILabel(new CGRect(20, 100, view.Frame.Width - 20, 30));
			label4.Text = "BALANCE DUE";
			view.InsertSubviewBelow(label4, label3);

			var label5 = new UILabel(new CGRect(20, 130, view.Frame.Width - 20, 50));
			//if (objInvoice.Location != null)
			if(!String.IsNullOrEmpty(objInvoice.LocationName))
				label5.Font = UIFont.BoldSystemFontOfSize(20);
			view.InsertSubviewBelow(label5, label4);

			var label6 = new UILabel(new CGRect(20, 160, view.Frame.Width - 20, 30));
			label6.Text = "LOCATION";
			view.InsertSubviewBelow(label6, label5);
			this.Add(view);
			if (objInvoice != null)
			{
				if (objInvoice.IsCancelled == true)
					label1.Text = "Cancelled";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue != 0)
					label1.Text = "Unfinished";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue == 0)
					label1.Text = "Completed";
				else
					label1.Text = "Unfinished";
				label3.Text = objInvoice.BalanceDue.ToString("C");
				label5.Text = objInvoice.LocationName;//?.Name;
			}

		}
	}
}
