
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace GreenleafiManager
{
	public partial class InvoiceDetailViewController : UIViewController
	{
		//Static layout data
		nfloat initialTopMargin = DefaultDetailsLayoutSettings.InitialTopMargin;
		nfloat initialLeftMargin = DefaultDetailsLayoutSettings.InitialLeftMargin;
		nfloat initialRightMargin = DefaultDetailsLayoutSettings.InitialRightMargin;

		nfloat defaultLabelHeight = DefaultDetailsLayoutSettings.InvoiceLabelHeight;
		nfloat defaultLabelWidth = DefaultDetailsLayoutSettings.InvoiceLabelWidth;

		nfloat defaultTextFieldHeight = DefaultDetailsLayoutSettings.InvoiceTextFieldHeight;
		nfloat defaultTextFieldWidth = DefaultDetailsLayoutSettings.InvoiceTextFieldWidth;

		nfloat defaultVerticalSpacing = DefaultDetailsLayoutSettings.DefaultVerticalSpacing;
		nfloat defaultHorizontalSpacing = DefaultDetailsLayoutSettings.InvoiceHorizontalSpacing;
		nfloat defaultItemVerticalSpacing = DefaultDetailsLayoutSettings.InvoiceVerticalSpacing;

		//Editable fields
		UILabel invoiceNumber;
		UILabel invoiceDate;
		UILabel status;
		UILabel location;
		UILabel salesperson;

		UILabel totalCost;
		UILabel paymentDue;
		UILabel subTotal;
		UILabel salesTaxTextField;

		UILabel customerName;
		UILabel customerNumber;
		UILabel customerPhoneNumber;
		public UILabel invoiceNote;
		UISwitch isMailOrderSwitch;
		UIButton btnEdit, btnPDFView, btnPrint;
		public Invoice objInvoice = new Invoice();

		public MasterInvoiceViewController MasterViewController { get; set; }

		public Invoice Invoice
		{
			get;
			set;
		}

		public string EditOrCreate
		{
			get;
			set;
		}
		public InvoiceDetailViewController(IntPtr handle) : base(handle)
		{
			//			var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveInvoice);
			//			saveButton.AccessibilityLabel = "saveButton";
			//			NavigationItem.RightBarButtonItem = saveButton;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.NavigationController.SetNavigationBarHidden(false, false);
			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;
			defaultLabelWidth = (nfloat)this.View.Bounds.Size.Width * 0.18f;
			defaultTextFieldWidth = (nfloat)this.View.Bounds.Size.Width * 0.18f;
			SetupViewUIItems();
			ConfigureView();
		}

		private void ConfigureView()
		{
			//invoiceNumberTextField.ShouldReturn += TextFieldShouldReturn;
			//invoiceDateTextField.ShouldReturn += TextFieldShouldReturn;
			//statusTextField.ShouldReturn += TextFieldShouldReturn;
			//locationTextField.ShouldReturn += TextFieldShouldReturn;
			//salespersonTextField.ShouldReturn += TextFieldShouldReturn;
			//totalCostTextField.ShouldReturn += TextFieldShouldReturn;
			//paymentDueTextField.ShouldReturn += TextFieldShouldReturn;
			//salesTaxTextField.ShouldReturn += TextFieldShouldReturn;
			//invoiceDateTextField.ShouldReturn += TextFieldShouldReturn;

			SetupFields();
		}

		private void SetupViewUIItems()
		{
			BuildInfoiceBasicInfoUI(40, initialTopMargin);
			nfloat _X = View.Frame.Width / 2 + 30;
			BuildCustomerUI(_X, initialTopMargin);
			BuildCostDetailsUI(40, (isMailOrderSwitch.Frame.Y + (defaultTextFieldHeight * 1.5f)));
			BuildPaymentDetailsUI(40, salesTaxTextField.Frame.Y + salesTaxTextField.Frame.Height + 50);
		}
		private void BuildInfoiceBasicInfoUI(nfloat initialX, nfloat initialY)
		{
			nfloat currentWidth = View.Frame.Width - 2 * initialX;

			btnEdit = new UIButton(new CGRect(initialX, initialY, currentWidth / 3 - 20, 40));
			btnEdit.SetTitle("EDIT", UIControlState.Normal);
			btnEdit.ToGreenLeafButton();
			btnEdit.TouchUpInside += ((sender, e) =>
			{
				InvoiceEditViewController ivc = this.Storyboard.InstantiateViewController("InvoiceEditViewController") as InvoiceEditViewController;
				ivc.objInvoice = objInvoice;
				this.NavigationController.PushViewController(ivc, true);
			});
			View.Add(btnEdit);

			btnPDFView = new UIButton(new CGRect(btnEdit.Frame.X + btnEdit.Frame.Width + 10, initialY, currentWidth / 3 - 20, 40));
			btnPDFView.SetTitle("VIEW PDF", UIControlState.Normal);
			btnPDFView.ToGreenLeafButton();
			btnPDFView.TouchUpInside += ((sender, e) =>
			{
				InvoicePreviewViewController ivc = this.Storyboard.InstantiateViewController("InvoicePreviewViewController") as InvoicePreviewViewController;
				ivc.objInvoice = objInvoice;
				this.NavigationController.PushViewController(ivc, true);
			});
			View.Add(btnPDFView);

			btnPrint = new UIButton(new CGRect(btnPDFView.Frame.X + btnPDFView.Frame.Width + 10, initialY, currentWidth / 3 - 20, 40));
			btnPrint.SetTitle("PRINT PDF", UIControlState.Normal);
			btnPrint.ToGreenLeafButton();
			btnPrint.TouchUpInside += ((sender, e) =>
			{
				InvoicePreviewViewController ivc = this.Storyboard.InstantiateViewController("InvoicePreviewViewController") as InvoicePreviewViewController;
				ivc.objInvoice = objInvoice;
				ivc.IsPrint = true;
				this.NavigationController.PushViewController(ivc, true);
			});
			View.Add(btnPrint);

			var label = new UILabel(new CGRect(initialX, initialY + btnPrint.Frame.Height + 30, defaultTextFieldWidth, defaultLabelHeight));
			label.Text = "INVOICE NUMBER";
			label.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(label);

			invoiceNumber = new UILabel(new CGRect(label.Frame.X + defaultTextFieldWidth + defaultHorizontalSpacing, initialY + btnPrint.Frame.Height + 30, defaultTextFieldWidth, defaultLabelHeight));
			invoiceNumber.Text = "89521";
			invoiceNumber.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(invoiceNumber);

			var seprator1 = new UIView(new CGRect(initialX, label.Frame.Y + label.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator1.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator1);
			var label2 = new UILabel(new CGRect(initialX, label.Frame.Y + label.Frame.Height + defaultItemVerticalSpacing, defaultLabelWidth, defaultLabelHeight));
			label2.Text = "INVOICE DATE";
			label2.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(label2);

			invoiceDate = new UILabel(new CGRect(label2.Frame.X + label2.Frame.Width + defaultHorizontalSpacing, label2.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			invoiceDate.Text = "15/06/2016";
			invoiceDate.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(invoiceDate);

			var seprator2 = new UIView(new CGRect(initialX, label2.Frame.Y + label2.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator2.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator2);

			var label3 = new UILabel(new CGRect(initialX, label2.Frame.Y + defaultLabelHeight + defaultItemVerticalSpacing, defaultLabelWidth, defaultLabelHeight));
			label3.Text = "STATUS";
			label3.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(label3);

			status = new UILabel(new CGRect(label3.Frame.X + label3.Frame.Width + defaultHorizontalSpacing, label3.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			status.Text = "New";
			status.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(status);

			var seprator3 = new UIView(new CGRect(initialX, label3.Frame.Y + label3.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator3.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator3);

			var lable4 = new UILabel(new CGRect(initialX, label3.Frame.Y + defaultLabelHeight + defaultItemVerticalSpacing, defaultLabelWidth, defaultLabelHeight));
			lable4.Text = "LOCATION";
			lable4.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(lable4);

			location = new UILabel(new CGRect(lable4.Frame.X + lable4.Frame.Width + defaultHorizontalSpacing, lable4.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			location.Text = "Online";
			location.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(location);

			var seprator4 = new UIView(new CGRect(initialX, lable4.Frame.Y + lable4.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator4.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator4);

			var lable5 = new UILabel(new CGRect(initialX, lable4.Frame.Y + defaultLabelHeight + defaultItemVerticalSpacing, defaultLabelWidth, defaultLabelHeight));
			lable5.Text = "SALES PERSON";
			lable5.TextAlignment = UITextAlignment.Left;
			lable5.TextColor = GlobalUISettings.InvoiceLabelTextColor;
			lable5.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(lable5);

			salesperson = new UILabel(new CGRect(lable5.Frame.X + lable5.Frame.Width + defaultHorizontalSpacing, lable5.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			salesperson.Text = "John";
			salesperson.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(salesperson);

			var seprator5 = new UIView(new CGRect(initialX, lable5.Frame.Y + lable5.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator5.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator5);

			var lable6 = new UILabel(new CGRect(initialX, lable5.Frame.Y + defaultLabelHeight + defaultItemVerticalSpacing, defaultLabelWidth, defaultLabelHeight * 1.5));
			lable6.Text = "Is Mail Order";
			lable6.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(lable6);

			isMailOrderSwitch = new UISwitch(new CGRect(lable6.Frame.X + lable6.Frame.Width + defaultHorizontalSpacing, lable6.Frame.Y, defaultTextFieldWidth / 4, defaultTextFieldHeight));
			isMailOrderSwitch.Enabled = false;
			View.AddSubview(isMailOrderSwitch);

			#region Set Values

			if (objInvoice != null)
			{
				invoiceNumber.Text = objInvoice.Number;
				invoiceDate.Text = objInvoice.Date;
				location.Text = objInvoice.LocationName;//?.Name;
				if (objInvoice.IsCancelled == true)
					status.Text = "Cancelled";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue != 0)
					status.Text = "In Process";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue == 0)
					status.Text = "Completed";
				else
					status.Text = "New";
				isMailOrderSwitch.On = objInvoice.IsMailOrder;
				salesperson.Text = objInvoice.User?.FullName;
				#endregion
			}
		}
		private void BuildCustomerUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, btnEdit.Frame.Height + initialY + 30, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 40));
			label.Text = "Customer";
			label.TextAlignment = UITextAlignment.Center;
			label.BackgroundColor = UIColor.LightGray;
			label.TextColor = UIColor.Black;
			label.Layer.CornerRadius = 10;
			label.ClipsToBounds = true;
			label.Font = UIFont.BoldSystemFontOfSize(20);
			View.AddSubview(label);

			customerName = new UILabel(new CGRect(initialX,
				label.Frame.Y + label.Frame.Height + defaultItemVerticalSpacing,
				defaultTextFieldWidth, defaultLabelHeight));
			customerName.Text = "Andre";
			customerName.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(customerName);

			customerNumber = new UILabel(new CGRect(customerName.Frame.X + customerName.Frame.Width + defaultHorizontalSpacing,
				customerName.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			customerNumber.Text = "#66566";
			customerNumber.Font = GlobalUISettings.InvoiceTextFieldFont;
			customerNumber.Hidden = true;
			View.AddSubview(customerNumber);

			var seprator1 = new UIView(new CGRect(initialX, customerName.Frame.Y + customerName.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator1.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator1);
			var labelNotes = new UILabel(new CGRect(initialX,
				customerNumber.Frame.Y + defaultLabelHeight + defaultItemVerticalSpacing,
				defaultTextFieldWidth, defaultLabelHeight));
			labelNotes.Text = "Notes:";
			labelNotes.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(labelNotes);

			invoiceNote = new UILabel(new CGRect(initialX,
				labelNotes.Frame.Bottom,
				2 * defaultTextFieldWidth + defaultHorizontalSpacing,
				defaultLabelHeight * 4));
			invoiceNote.Font = GlobalUISettings.InvoiceTextFieldFont;
			invoiceNote.Lines = 3;
			View.AddSubview(invoiceNote);
			var seprator3 = new UIView(new CGRect(initialX, invoiceNote.Frame.Y + invoiceNote.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator3.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator3);

			UITapGestureRecognizer noteLabelTap = new UITapGestureRecognizer( () => 
			{
				var popup = new InvoiceNoteChangePopup(this.View.Bounds.Width, this.View.Bounds.Height, "Add note to invoice", this, objInvoice.Id);
				this.View.AddSubview(popup);
			});

			invoiceNote.UserInteractionEnabled = true;
			invoiceNote.AddGestureRecognizer(noteLabelTap);

			#region SetValues

			if (objInvoice.Customer != null)
			{
				customerName.Text = objInvoice.Customer?.FullName;
				customerNumber.Text = objInvoice.Customer?.Id; //TODO: Customer IDD
				invoiceNote.Text = objInvoice.Notes != null ? objInvoice.Notes : string.Empty;
			}
			#endregion
		}
		private void BuildCostDetailsUI(nfloat initialX, nfloat initialY)
		{
			var titleLabel = new UILabel(new CGRect(initialX, initialY, View.Frame.Width - 80, 40));
			titleLabel.Text = "COST DETAILS";
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.BackgroundColor = UIColor.LightGray;
			titleLabel.TextColor = UIColor.Black;
			titleLabel.Layer.CornerRadius = 10;
			titleLabel.ClipsToBounds = true;
			titleLabel.Font = UIFont.BoldSystemFontOfSize(20); ;
			View.AddSubview(titleLabel);

			var label = new UILabel(new CGRect(initialX, titleLabel.Frame.Y + defaultTextFieldHeight + defaultHorizontalSpacing, defaultTextFieldWidth, defaultLabelHeight));
			label.Text = "TOTAL COST";
			label.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(label);

			totalCost = new UILabel(new CGRect(label.Frame.X + defaultTextFieldWidth + defaultHorizontalSpacing, label.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			totalCost.Text = "";
			totalCost.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(totalCost);

			var seprator1 = new UIView(new CGRect(initialX, label.Frame.Y + label.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator1.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator1);

			var label2 = new UILabel(new CGRect(initialX, label.Frame.Y + label.Frame.Height + defaultItemVerticalSpacing, defaultLabelWidth, defaultLabelHeight));
			label2.Text = "PAYMENT DUE";
			label2.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(label2);

			paymentDue = new UILabel(new CGRect(label2.Frame.X + label2.Frame.Width + defaultHorizontalSpacing, label2.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			paymentDue.Text = "";
			paymentDue.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(paymentDue);

			var seprator2 = new UIView(new CGRect(initialX, label2.Frame.Y + label2.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator2.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator2);

			var _X = View.Frame.Width / 2 + 30;
			var label3 = new UILabel(new CGRect(_X, label.Frame.Y, defaultLabelWidth, defaultLabelHeight));
			label3.Text = "SUB TOTAL";
			label3.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(label3);

			subTotal = new UILabel(new CGRect(label3.Frame.X + label3.Frame.Width + defaultHorizontalSpacing, label3.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			subTotal.Text = "";
			subTotal.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(subTotal);

			var seprator3 = new UIView(new CGRect(label3.Frame.X, label3.Frame.Y + label3.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator3.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator3);

			var lable4 = new UILabel(new CGRect(label3.Frame.X, label3.Frame.Y + defaultLabelHeight + defaultItemVerticalSpacing, defaultLabelWidth, defaultLabelHeight));
			lable4.Text = "SALES TAX";
			lable4.Font = GlobalUISettings.InvoiceLabelFont;
			View.AddSubview(lable4);

			salesTaxTextField = new UILabel(new CGRect(lable4.Frame.X + lable4.Frame.Width + defaultHorizontalSpacing, lable4.Frame.Y, defaultTextFieldWidth, defaultLabelHeight));
			salesTaxTextField.Font = GlobalUISettings.InvoiceTextFieldFont;
			View.AddSubview(salesTaxTextField);

			var seprator4 = new UIView(new CGRect(lable4.Frame.X, lable4.Frame.Y + label3.Frame.Height + 2, 2 * defaultTextFieldWidth + defaultHorizontalSpacing, 1));
			seprator4.BackgroundColor = UIColor.Gray;
			View.AddSubview(seprator4);

			#region Set Values
			if (objInvoice != null)
			{
				totalCost.Text = objInvoice.TotalPrice.ToString("C");
				paymentDue.Text =objInvoice.BalanceDue.ToString("C");
				subTotal.Text = objInvoice.OrderTotal.ToString("C");
				salesTaxTextField.Text = (Math.Round(objInvoice.OrderTotal * objInvoice.LocationTax, 2)).ToString("C");
			}
			#endregion

		}

		private void BuildPaymentDetailsUI(nfloat initialX, nfloat initialY)
		{
			var titleLabel = new UILabel(new CGRect(initialX, initialY, View.Frame.Width - 80, 40));
			titleLabel.Text = "PAYMENT HISTORY";
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.BackgroundColor = UIColor.LightGray;
			titleLabel.TextColor = UIColor.Black;
			titleLabel.Layer.CornerRadius = 10;
			titleLabel.ClipsToBounds = true;
			titleLabel.Font = UIFont.BoldSystemFontOfSize(20); ;
			View.AddSubview(titleLabel);

			var itemTable = new UITableView(new CGRect(80, initialY + 50, View.Frame.Width - 160, 200));
			itemTable.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			nfloat vFullWidth = itemTable.Frame.Width;
			var headerView = new UIView(new CGRect(0, 0, View.Frame.Width - 160, 40));

			var lblAmount = new UILabel(new CGRect(0, 0, vFullWidth / 3, 40))
			{
				Text = "PAID AMOUNT",
				Font = UIFont.BoldSystemFontOfSize(17f),
			};
			headerView.Add(lblAmount);

			var lblPaymentType = new UILabel(new CGRect(lblAmount.Frame.X + lblAmount.Frame.Width, 0, vFullWidth / 3, 40))
			{
				Text = "PAYMENT TYPE",
				Font = UIFont.BoldSystemFontOfSize(17f),
			};
			headerView.Add(lblPaymentType);

			var lbldate = new UILabel(new CGRect(lblPaymentType.Frame.X + lblPaymentType.Frame.Width, 0, vFullWidth / 3, 40))
			{
				Text = "PAYMENT DATE",
				Font = UIFont.BoldSystemFontOfSize(17f),
			};
			headerView.Add(lbldate);

			itemTable.TableHeaderView = headerView;
			if (objInvoice != null && objInvoice.Payments != null)
			{
				var paymetlist = objInvoice.Payments.ToList();
				itemTable.Source = new InvoiceDetailPaymentSource(paymetlist, this);
				itemTable.ReloadData();
			}
			Add(itemTable);


		}
		public void SetupFields()
		{
			//			nameTextField.Text = Invoice.Name;
			//			phoneTextField.Text = Invoice.Phone;
			//
			//			address1TextField.Text = Invoice.Address1;
			//			address2TextField.Text = Invoice.Address2;
			//			cityTextField.Text = Invoice.City;
			//			provinceTextField.Text = Invoice.Province;
			//			zipTextField.Text = Invoice.Zip;
			//			countryTextField.Text = Invoice.Country;
		}

		private bool TextFieldShouldReturn(UITextField textfield)
		{
			nint nextTag = textfield.Tag + 1;
			UIResponder nextResponder = this.View.ViewWithTag(nextTag);
			if (nextResponder != null)
			{
				nextResponder.BecomeFirstResponder();
			}
			else {
				// Not found, so remove keyboard.
				textfield.ResignFirstResponder();
				//ResetTheScroller();
			}
			return false;
		}


		void ConvertPDFFromView()
		{
			NSMutableData pdfData = new NSMutableData();

			//UIGraphics.BeginPDFContext
			UIGraphics.BeginPDFContext(pdfData, View.Bounds, null);
			UIGraphics.BeginPDFPage();
			CGContext pdfContext = UIGraphics.GetCurrentContext();
			View.Layer.RenderInContext(pdfContext);
			UIGraphics.EndPDFContent();

			var documentDirectories = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, true);
			var documentDirectory = documentDirectories.ElementAt(0);
			var documentDirectoryFilename = Path.Combine(documentDirectory, "fileName.pdf"); //Not sure about extension 
			NSError error;
			pdfData.Save(documentDirectoryFilename, true, out error);

			var printInfo = UIPrintInfo.PrintInfo;

			printInfo.Duplex = UIPrintInfoDuplex.LongEdge;

			printInfo.OutputType = UIPrintInfoOutputType.General;

			printInfo.JobName = "Invoice";

			var printer = UIPrintInteractionController.SharedPrintController;

			printer.PrintInfo = printInfo;

			printer.PrintingItem = NSData.FromFile(documentDirectoryFilename);

			printer.ShowsPageRange = true;

			printer.Present(true, (handler, completed, err) =>
			{
				if (!completed && err != null)
				{
					Console.WriteLine("Printer Error");
				}
			});


			//System.IO.File.WriteAllBytes
			//NSArray documentDirectories = NSSearchPathDirectory. NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask,YES);
			//
			//			NSString* documentDirectory = [documentDirectories objectAtIndex:0];
			//			NSString documentDirectoryFilename = [documentDirectory stringByAppendingPathComponent:aFilename];
			//
			//			// instructs the mutable data object to write its context to a file on disk
			//			[pdfData writeToFile:documentDirectoryFilename atomically:YES];

		}
		void PrintingCompleted(UIPrintInteractionController controller, bool completed, NSError error)
		{

		}

		async void SaveInvoice(object sender, EventArgs args)
		{
			//			var invoice = new Invoice();
			//			//Invoice.Id = Invoice.Id;
			//			//Invoice.Name = nameTextField.Text;
			//			//Invoice.Address1 = address1TextField.Text;
			//			//Invoice.Address2 = address2TextField.Text;
			//			//Invoice.City = cityTextField.Text;
			//			//Invoice.Zip = zipTextField.Text;
			//			//Invoice.Country = countryTextField.Text;
			//			//Invoice.Phone = phoneTextField.Text;
			//			//Invoice.Province = provinceTextField.Text;
			//			if (!ValidateBeforeSavingOrCreating(Invoice))
			//				return;
			//
			//			await _InvoiceService.InsertOrSaveInvoice(invoice);
			//
			//			var alert = UIAlertController.Create("Invoice", "Invoice " + EditOrCreate + " successfuly", UIAlertControllerStyle.Alert);
			//			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) =>
			//			{
			//				MasterViewController.SelectedItem = Invoice;
			//				this.NavigationController.PopViewController(true);
			//			}));
			//			PresentViewController(alert, animated: true, completionHandler: null);

		}

	}
}
