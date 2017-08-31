using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using UIKit;

namespace GreenleafiManager
{
	public class InvoiceNoteChangePopup: UIView
	{
		public UIView background;
		public UIView alert;
		UITextView txtNote;
		UIButton btnCreate, btnCancel;
		private InvoiceDetailViewController owner;
		private string InvoiceId;
		private string Title;

		private nfloat height;
		private nfloat width;

		public InvoiceNoteChangePopup(nfloat w, nfloat h, string title, InvoiceDetailViewController owner, string invoiceID)
		{
			this.InvoiceId = invoiceID;
			this.Title = title;
			this.Frame = owner.View.Bounds;
			this.owner = owner;
			height = h;
			width = w;
			Initialize();
		}

		private void Initialize()
		{
			if (AzureService.InvoiceService.Invoices.Any(x => x.Id == InvoiceId))
			{
				this.UserInteractionEnabled = true;
				background = new UIView() { BackgroundColor = UIColor.Black, Alpha = 0.5f };
				background.Frame = new CGRect(0, 0, width, height);

				alert = new UIView() { BackgroundColor = UIColor.White };
				alert.Frame = new CGRect(width / 3, height / 6.5, width / 3, height / 1.5);
				alert.Layer.CornerRadius = 10f;
				alert.ExclusiveTouch = false;

				var titleLabel = new UILabel() { Text = Title };
				titleLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
				titleLabel.Frame = new CGRect(alert.Frame.GetMinX() - alert.Frame.Width + 2, 20, alert.Frame.Width - 4, 50);
				titleLabel.TextAlignment = UITextAlignment.Center;
				AddSubview(background);
				AddSubview(alert);
				alert.AddSubview(titleLabel);

				var lblActivity = new UILabel(new CGRect(alert.Frame.GetMinX() - alert.Frame.Width + 2, alert.Frame.Top - 2, alert.Frame.Width, 50));
				lblActivity.Text = "Note: ";
				alert.AddSubview(lblActivity);

				txtNote = new UITextView(new CGRect(lblActivity.Frame.X, lblActivity.Frame.Bottom + 2, titleLabel.Frame.Width, alert.Frame.Height / 3 ));
				var invoice = AzureService.InvoiceService.Invoices.FirstOrDefault(x => x.Id == InvoiceId);
				txtNote.Text = invoice.Notes;
				alert.AddSubview(txtNote);


				btnCancel = new UIButton(new CGRect(lblActivity.Frame.X, txtNote.Frame.Bottom + 100, lblActivity.Frame.Width / 2 - 2, 50));
				btnCancel.SetTitle("Cancel", UIControlState.Normal);
				btnCancel.BackgroundColor = UIColor.Gray;
				btnCancel.SetTitleColor(UIColor.White, UIControlState.Normal);
				btnCancel.TouchUpInside += delegate
				{
					Hide();
				};

				btnCreate = new UIButton(new CGRect(btnCancel.Frame.Width + 4, btnCancel.Frame.Y, btnCancel.Frame.Width - 2, 50));
				btnCreate.SetTitle("Save", UIControlState.Normal);
				btnCreate.BackgroundColor = UIColor.Gray;
				btnCreate.SetTitleColor(UIColor.White, UIControlState.Normal);
				btnCreate.TouchUpInside += async delegate
				{
					var invoiceToUpdate = AzureService.InvoiceService.Invoices.FirstOrDefault(x => x.Id == InvoiceId);
					invoiceToUpdate.Notes = txtNote.Text;
					foreach (var e in invoice.Payments)
					{
						e.Remove = false;
						e.Update = false;
						e.Add = false;
					}
					foreach (var e in invoice.Items)
					{
						e.AddNewFromInvoice = false;
						e.RemoveFromInvoice = false;
					}
					await AzureService.InvoiceService.InsertOrSaveInvoice(invoiceToUpdate);
					owner.invoiceNote.Text = invoiceToUpdate.Notes;
					Hide();
				};
				alert.AddSubview(btnCancel);
				alert.AddSubview(btnCreate);
			}
			else
				MessageBox(Title + " error", "No invoice found for the selected item.");
		}

		public void MessageBox(string Title, string message)
		{
			UIAlertView alertView = new UIAlertView(Title, message, null, "Ok", null);
			alertView.Show();
		}

		public void Hide()
		{
			UIView.Animate(
				0.5, // duration
				() => { Alpha = 0; },
				() => { RemoveFromSuperview(); }
			);
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
		{
			var touch = touches.AnyObject as UITouch;
			var touchCoordinats = touch.LocationInView(background);
			var a = (touchCoordinats.X > (width / 4));
			var b = touchCoordinats.X < (3 * width / 4);
			var c = (touchCoordinats.Y > (height / 7.5));
			var d = touchCoordinats.Y < (9.3 * height / 12.8);
			if (!(a && b && c && d))
			{
				base.TouchesBegan(touches, evt);
				Hide();
			}
		}
	}
}
