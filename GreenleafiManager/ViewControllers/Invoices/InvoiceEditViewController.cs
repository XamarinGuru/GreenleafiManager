
using System;
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GreenleafiManager
{
	public partial class InvoiceEditViewController : UIViewController
	{
		CGRect currentFrame;
		UIAlertController locationActionSheet, statusActionSheet, customerActionSheet, salesPersonActionSheet, paymentTypeActionSheet, itemTagPriceChangeActionSheet;
		UITextField txtCustomer, txtStatus, txtLocation, txtSalesPerson, txtPaymentAmount, txtPaymentType, txtApprovalCode, txtPaymentDate;
		UILabel lblAmountDue, lblSubTotalOrder, lblTax, lblTotalOrder;
		ModalPickerViewController modalPicker;
		UISwitch isMailOrderSwitch, taxInSwitch;
		UITableView paymentTable, itemTableView;

		private string _AddedItemID;
		double amountDue = 0, paidAmount = 0, total = 0, tax = 0, itemsTotal = 0, pricePerItemToDeductForTaxIn;

		public List<Payment> paymentList = new List<Payment>();
		public List<Payment> paymentListToRemove = new List<Payment>();
		public List<Item> itemListToRemove = new List<Item>();
		UIAlertController alertWithTextBox;
		public List<Item> itemsList = new List<Item>();
		public List<Item> tempitemsList = new List<Item>();
			public List<Payment> tempPaymentList = new List<Payment>();
		public Invoice objInvoice = new Invoice();
		public bool isFromCreatePage = false;
		public static LoadingOverlay loadingOverlay;
		UIScrollView scrollView;
		private string objStatus;
		public InvoiceEditViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			this.scrollView.UnsubscribeKeyboardScrollView();
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			currentFrame = this.View.Bounds;
			var bounds = UIScreen.MainScreen.Bounds;
			scrollView = new UIScrollView(this.View.Frame);
			scrollView.ContentSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

			loadingOverlay = new LoadingOverlay(bounds, "processing...");
			ShowLoading();
			locationActionSheet = UIAlertController.Create("Location", "Select Location", UIAlertControllerStyle.ActionSheet);
			customerActionSheet = UIAlertController.Create("Customers", "Select Customer", UIAlertControllerStyle.ActionSheet);

			CreateStatusActionSheet();
			CreateSalesPersonActionSheet();
			CreateDateActionSheet();
			CreatePaymentTypeActionSheet();
			CreateEditView();

			if (objInvoice != null)
			{
				txtCustomer.Text = objInvoice.Customer?.FirstName;
				txtLocation.Text = objInvoice.LocationName;//?.Name;
				txtSalesPerson.Text = objInvoice.User?.FullName;
				if (objInvoice.IsCancelled == true)
					txtStatus.Text = "Cancelled";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue != 0)
					txtStatus.Text = "Unfinished";
				else if (objInvoice.PaidAmount != 0 && objInvoice.BalanceDue == 0)
					txtStatus.Text = "Completed";
				else
					txtStatus.Text = "Unfinished";
				objStatus = txtStatus.Text;
			}
			CreateLocationActionSheet();

			//ScrollView
			this.View.Add(scrollView);

			if (!isFromCreatePage)
			{
				var btn = new UIBarButtonItem("< Back", UIBarButtonItemStyle.Plain, (sender, e) =>
				 {
					 if (haveChangedBeenMade())
					 {
						 UIAlertController alert = UIAlertController.Create("Leaving the page?", "If you leave the page before saving, all changes will be lost.", UIAlertControllerStyle.Alert);
						 alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
						 {
						 }));
						 alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, async (actionOK) =>
						 {
							 foreach (var item in AzureService.InvoiceService.ItemsList)
							 {
								 if (item.AddNewFromInvoice)
								 {
									 item.AddNewFromInvoice = false;
									 await AzureService.InventoryService.MarkItemSoldAsync(item, false);
								 }
							 }
							 foreach (var item in itemListToRemove)
							 {
								 if (item.RemoveFromInvoice)
								 {
									 AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == item.Id).RemoveFromInvoice = false;
								 }
							 }

							 paymentListToRemove = new List<Payment>();
							 paymentList = new List<Payment>();
							 itemsList = new List<Item>();
							 itemListToRemove = new List<Item>();
							 AzureService.InvoiceService.ItemsList = new List<Item>();
							 this.NavigationController.PopViewController(true);
						 }));
						 PresentViewController(alert, true, null);
					 }
					 else
					 {
						 this.NavigationController.PopViewController(true);
					 }
				 });
				this.NavigationItem.LeftBarButtonItem = btn;
			}
			SetupAmounts();
		}

		public void ShowLoading()
		{
			loadingOverlay.Alpha = 1;
			scrollView.Add(loadingOverlay);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			this.scrollView.InitKeyboardScrollView();
			if (loadingOverlay != null)
			{
				if (loadingOverlay.Alpha == 0)
					ShowLoading();
			}
			if (AzureService.InvoiceService.Customer != null && objInvoice != null)
			{
				objInvoice.CustomerId = AzureService.InvoiceService.Customer.Id;
				objInvoice.Customer = (Customer)AzureService.InvoiceService.Customer;
				txtCustomer.Text = AzureService.InvoiceService.Customer.FirstName + " " + AzureService.InvoiceService.Customer.LastName;

				AzureService.InvoiceService.Customer = null;
			}

			if (AzureService.InvoiceService.ItemsList != null && AzureService.InvoiceService.ItemsList.Count > 0)
			{
				foreach (var item in AzureService.InvoiceService.ItemsList)
				{
					if (!itemsList.Any(x => x.Id == item.Id))
					{
						itemsList.Add(item);
						UIAlertController alert = UIAlertController.Create("Enter Sale Price", "", UIAlertControllerStyle.Alert);
						UITextField field = null;
						UITextField descriptionField = null;
						alert.AddTextField((textField) =>
						{
							field = textField;
							//field.Text = item.TagPrice.ToString()
							field.Placeholder = "Sale Price";
							field.AutocorrectionType = UITextAutocorrectionType.No;
							field.KeyboardType = UIKeyboardType.NumberPad;
							field.ReturnKeyType = UIReturnKeyType.Done;
							field.ClearButtonMode = UITextFieldViewMode.WhileEditing;

						});

						_AddedItemID = item.Id;

						alert.AddAction(UIAlertAction.Create("Next", UIAlertActionStyle.Default, (actionOK) =>
						{
							double newPrice = 0;

							Double.TryParse(field.Text, out newPrice);



							if (newPrice > 0)
							{
								itemsList.FirstOrDefault(x => x.Id == _AddedItemID).Price = newPrice;
							}
							else
							{
								UIAlertController innerAlert = UIAlertController.Create("Invalid Sale Price Input", "Please enter a valid number", UIAlertControllerStyle.Alert);

								PresentViewController(innerAlert, true, null);
								innerAlert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (actionInnerOk) =>
								{
									PresentViewController(alert, true, null);
								}));
								return;
							}

							UIAlertController descAlert = UIAlertController.Create("Update Description for Invoice", "", UIAlertControllerStyle.Alert);

							descAlert.AddTextField((textField) =>
						{
							descriptionField = textField;
							descriptionField.Text = item.Description;
							descriptionField.AutocorrectionType = UITextAutocorrectionType.Yes;
							descriptionField.KeyboardType = UIKeyboardType.NumberPad;
							descriptionField.ReturnKeyType = UIReturnKeyType.Done;
							descriptionField.ClearButtonMode = UITextFieldViewMode.WhileEditing;

						});
							descAlert.AddAction(UIAlertAction.Create("Done", UIAlertActionStyle.Default, (actionDone) =>
						{
							itemsList.FirstOrDefault(x => x.Id == _AddedItemID).InvoiceDescription = descriptionField.Text;
							var tempList = objInvoice.Items.ToList();
							tempList.AddRange(itemsList.Where(x => x.AddNewFromInvoice).ToList());
							itemTableView.Source = new InvoiceItemTableSource(tempList, this);
							itemTableView.ReloadData();

							SetupAmounts();
						}));
							PresentViewController(descAlert, true, null);



						}));
						PresentViewController(alert, true, null);
					}
				}
			}
			if (objInvoice.Items != null && itemTableView != null)
			{
				//update description
				foreach (var item in objInvoice.Items)
					if (AzureService.InventoryService.Items.Any(x => x.Id == item.Id))
						item.InvoiceDescription = AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == item.Id).InvoiceDescription;

				var tempList = objInvoice.Items.ToList();
				tempList.AddRange(itemsList.Where(x => x.AddNewFromInvoice).ToList());
				//var balanceDue = Convert.ToDouble(itemsList.Sum(x => x.ItemSalePrice));
				//balanceDue = balanceDue + objInvoice.BalanceDue;
				//txtPaidAmount.Text = balanceDue.ToString("C");
				itemTableView.Source = new InvoiceItemTableSource(tempList, this);
				itemTableView.ReloadData();
				SetupAmounts();

			}
			tempitemsList = objInvoice.Items.ToList();

			if (objInvoice.Payments != null && objInvoice.Payments.Count > 0)
			{
				paymentList = objInvoice.Payments.ToList();
				tempPaymentList = paymentList.ToList();
				paymentTable.Source = new InvoicePaymentTableSource(paymentList, this);
				paymentTable.ReloadData();
			}
			if (loadingOverlay != null)
				loadingOverlay.Hide();

			SetupAmounts();

		}

		void CreateLocationActionSheet()
		{
			//await AzureService.LocationService.UpdateLocationsFromAzure();
			if (AzureService.LocationService.Locations != null)
			{
				foreach (Location item in AzureService.LocationService.Locations)
				{
					locationActionSheet.AddAction(UIAlertAction.Create(item.Name, UIAlertActionStyle.Default, (action) =>
						{
							txtLocation.Text = action.Title;
							objInvoice.LocationId = item.Id;
							SetupAmounts();
						}));
				}
			}
		}
		void CreateStatusActionSheet()
		{
			//objInvoice
			if (objInvoice != null)
			{
				if (objInvoice.IsCancelled)
				{
					statusActionSheet = UIAlertController.Create("Status", "Select Status", UIAlertControllerStyle.ActionSheet);
					statusActionSheet.AddAction(UIAlertAction.Create("Reopen", UIAlertActionStyle.Default, (action) => OnStatusSelection(action.Title)));
				}
				else
				{
					statusActionSheet = UIAlertController.Create("Status", "Select Status", UIAlertControllerStyle.ActionSheet);
					statusActionSheet.AddAction(UIAlertAction.Create("Cancelled", UIAlertActionStyle.Default, (action) => OnStatusSelection(action.Title)));
				}
			}
			//statusActionSheet.AddAction(UIAlertAction.Create("New", UIAlertActionStyle.Default, (action) => OnStatusSelection(action.Title)));
			//statusActionSheet.AddAction(UIAlertAction.Create("InProcess", UIAlertActionStyle.Default, (action) => OnStatusSelection(action.Title)));
			//statusActionSheet.AddAction(UIAlertAction.Create("Completed", UIAlertActionStyle.Default, (action) => OnStatusSelection(action.Title)));
		}
		void CreateCustomerActionSheet()
		{
			MasterCustomerViewController mivc = this.Storyboard.InstantiateViewController("MasterCustomerViewController") as MasterCustomerViewController;
			mivc.isAddCustomer = true;
			this.NavigationController.PushViewController(mivc, true);
		}


		void CreateSalesPersonActionSheet()
		{
			salesPersonActionSheet = UIAlertController.Create("Sales Person", "Select sales person", UIAlertControllerStyle.ActionSheet);
			//if (AzureService.UserService.Users == null)
			//{
			//	//await userService.InitializeStoreAsync();
			//	await AzureService.UserService.UpdateUsersFromAzure();
			//}
			if (AzureService.UserService.Users != null)
			{
				foreach (User item in AzureService.UserService.Users)
				{
					salesPersonActionSheet.AddAction(UIAlertAction.Create(item.FirstName, UIAlertActionStyle.Default, (action) =>
						{
							txtSalesPerson.Text = action.Title;
							//customerActionSheet.sele
							objInvoice.User = item;
							objInvoice.UserId = item.Id;
						}));
				}
			}

			//salesPersonActionSheet = UIAlertController.Create("Sales Person", "Select Person", UIAlertControllerStyle.ActionSheet);
		}

		void CreatePaymentTypeActionSheet()
		{
			paymentTypeActionSheet = UIAlertController.Create("Payment Type", "Select Payment Type", UIAlertControllerStyle.ActionSheet);
			if (AzureService.InvoiceService.PaymentTypes != null)
			{
				foreach (var item in AzureService.InvoiceService.PaymentTypes)
				{
					paymentTypeActionSheet.AddAction(UIAlertAction.Create(item.Name, UIAlertActionStyle.Default, (action) => OnPaymentTypeSelection(action.Title)));
				}
			}
		}

		void OnPaymentTypeSelection(string title)
		{
			txtPaymentType.Text = title;
			//paymentType.Name = title;
		}

		void OnSalesPersonSelection(string title)
		{
			txtSalesPerson.Text = title;
			objInvoice.UserId = title;
		}
		void OnCustomerSelection(string title)
		{
			txtCustomer.Text = title;
			SetupAmounts();
		}
		void OnLocationSelection(string title)
		{
			txtLocation.Text = title;
			SetupAmounts();
		}
		void OnStatusSelection(string title)
		{
			txtStatus.Text = title;
			//objInvoice.
		}
		void CreateDateActionSheet()
		{
			modalPicker = new ModalPickerViewController(ModalPickerType.Date, "Select Date", this)
			{
				HeaderBackgroundColor = UIColor.Red,
				HeaderTextColor = UIColor.White,
				TransitioningDelegate = new ModalPickerTransitionDelegate(),
				ModalPresentationStyle = UIModalPresentationStyle.Custom
			};
			modalPicker.DatePicker.Mode = UIDatePickerMode.Date;

			modalPicker.OnModalPickerDismissed += (s, ea) =>
			{
				var dateFormatter = new NSDateFormatter()
				{
					DateFormat = "MM/dd/yyyy"
				};

				txtPaymentDate.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
			};
		}

		//Edit View Init
		void CreateEditView()
		{
			nfloat yAxis = 0, flatY = 5;
			nfloat vHeight = 40;
			nfloat vWidth = 0, vFullWidth = 0;
			var mainView = new UIView(new CGRect(20, 10, currentFrame.Width - 40, currentFrame.Height - 40));
			vFullWidth = mainView.Frame.Width;
			nfloat columnW = vFullWidth / 20;
			vWidth = vFullWidth / 3;
			var lblStatus = new UILabel(new CGRect(0, 0, currentFrame.Width, vHeight));
			lblStatus.Text = "Status";
			mainView.Add(lblStatus);
			yAxis = lblStatus.Frame.Height + flatY;//Carry Y this for next view

			txtStatus = new UITextField(new CGRect(0, yAxis, vWidth, vHeight));
			txtStatus.Placeholder = "Status";
			txtStatus.Enabled = false;
			txtStatus.ToGreenLeafTextField();
			mainView.Add(txtStatus);

			var selectStatusButton = new UIButton(new CGRect(txtStatus.Frame.Width, txtStatus.Frame.Y, vHeight, vHeight));
			selectStatusButton.SetTitle("...", UIControlState.Normal);
			selectStatusButton.BackgroundColor = UIColor.Gray;
			selectStatusButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			selectStatusButton.TouchUpInside += (sender, e) =>
			{
				UIPopoverPresentationController presentationPopover = statusActionSheet.PopoverPresentationController;
				if (presentationPopover != null)
				{
					presentationPopover.SourceView = selectStatusButton;
					presentationPopover.SourceRect = selectStatusButton.Bounds;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Any;
				}
				PresentViewController(statusActionSheet, true, null);
			};
			mainView.Add(selectStatusButton);

			isMailOrderSwitch = new UISwitch(new CGRect(vWidth + vHeight * 2, yAxis, vHeight, vHeight));
			isMailOrderSwitch.On = objInvoice.IsMailOrder;
			isMailOrderSwitch.TouchUpInside += (sender, e) =>
			{
				SetupAmounts();
			};
			mainView.Add(isMailOrderSwitch);
			var lblMailOrder = new UILabel(new CGRect(isMailOrderSwitch.Frame.X + vHeight + 20, yAxis - 2, vWidth, vHeight));
			lblMailOrder.Text = "Is Mail Order";
			mainView.Add(lblMailOrder);


			taxInSwitch = new UISwitch(new CGRect(isMailOrderSwitch.Frame.X + (vWidth + vHeight* 2), yAxis, vHeight, vHeight));
			taxInSwitch.On = objInvoice.IsMailOrder;
			taxInSwitch.TouchUpInside += (sender, e) =>
			{
				updateAmountsBasedOnTaxIn();
			};
			mainView.Add(taxInSwitch);
			var lblTaxIn = new UILabel(new CGRect(taxInSwitch.Frame.X + vHeight + 20, yAxis - 2, vWidth, vHeight));
			lblTaxIn.Text = "Tax In";
			mainView.Add(lblTaxIn);


			yAxis += vHeight;

			//2nd Row Controls
			var lblLocation = new UILabel(new CGRect(0, yAxis, vWidth, vHeight));
			lblLocation.Text = "Location";
			mainView.Add(lblLocation);

			var lblCustomer = new UILabel(new CGRect(vWidth + 20, yAxis, vWidth, vHeight));
			lblCustomer.Text = "Customer";
			mainView.Add(lblCustomer);

			var lblSalesPerson = new UILabel(new CGRect(vWidth * 2 + 20, yAxis, vWidth, vHeight));
			lblSalesPerson.Text = "Sales Person";
			mainView.Add(lblSalesPerson);

			txtLocation = new UITextField(new CGRect(0, lblLocation.Frame.Y + vHeight, vWidth - vHeight - 10, vHeight));
			txtLocation.Placeholder = "Location";
			txtLocation.Enabled = false;
			txtLocation.ToGreenLeafTextField();
			mainView.Add(txtLocation);

			var selectLocationButton = new UIButton(new CGRect(txtLocation.Frame.Width, txtLocation.Frame.Y, vHeight, vHeight));
			selectLocationButton.SetTitle("...", UIControlState.Normal);
			selectLocationButton.BackgroundColor = UIColor.Gray;
			selectLocationButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			selectLocationButton.TouchUpInside += (sender, e) =>
			{
				UIPopoverPresentationController presentationPopover = locationActionSheet.PopoverPresentationController;
				if (presentationPopover != null)
				{
					presentationPopover.SourceView = selectLocationButton;
					presentationPopover.SourceRect = selectLocationButton.Bounds;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Any;
				}
				PresentViewController(locationActionSheet, true, null);
			};
			mainView.Add(selectLocationButton);


			txtCustomer = new UITextField(new CGRect(vWidth + 10, lblCustomer.Frame.Y + vHeight, vWidth - vHeight - 15, vHeight));
			txtCustomer.Placeholder = "Customer";
			txtCustomer.Enabled = false;
			txtCustomer.ToGreenLeafTextField();
			mainView.Add(txtCustomer);

			var selectCustomerButton = new UIButton(new CGRect(txtCustomer.Frame.X + txtCustomer.Frame.Width, txtCustomer.Frame.Y, vHeight, vHeight));
			selectCustomerButton.SetTitle("...", UIControlState.Normal);
			selectCustomerButton.BackgroundColor = UIColor.Gray;
			selectCustomerButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			selectCustomerButton.TouchUpInside += (sender, e) =>
			{
				CreateCustomerActionSheet();

			};
			mainView.Add(selectCustomerButton);

			txtSalesPerson = new UITextField(new CGRect(vWidth * 2 + 10, lblSalesPerson.Frame.Y + vHeight, vWidth - vHeight - 15, vHeight));
			txtSalesPerson.Placeholder = "Sales Person";
			txtSalesPerson.Enabled = false;
			txtSalesPerson.ToGreenLeafTextField();
			mainView.Add(txtSalesPerson);

			var selectSalesPersonButton = new UIButton(new CGRect(txtSalesPerson.Frame.X + txtSalesPerson.Frame.Width, txtSalesPerson.Frame.Y, vHeight, vHeight));
			selectSalesPersonButton.SetTitle("...", UIControlState.Normal);
			selectSalesPersonButton.BackgroundColor = UIColor.Gray;
			selectSalesPersonButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			selectSalesPersonButton.TouchUpInside += (sender, e) =>
			{
				UIPopoverPresentationController presentationPopover = salesPersonActionSheet.PopoverPresentationController;
				if (presentationPopover != null)
				{
					presentationPopover.SourceView = selectSalesPersonButton;
					presentationPopover.SourceRect = selectSalesPersonButton.Bounds;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Any;
				}
				PresentViewController(salesPersonActionSheet, true, null);
			};
			mainView.Add(selectSalesPersonButton);

			//Add(mainView);



			//Items
			var lblItemsHeading = new UILabel(new CGRect(0, txtSalesPerson.Frame.Y + vHeight + 5, vFullWidth, vHeight));//new UILabel(new CGRect(0, paymentTable.Frame.Y + paymentTable.Frame.Height - 10, vFullWidth, vHeight));
			lblItemsHeading.Text = "Items";
			lblItemsHeading.TextAlignment = UITextAlignment.Center;
			lblItemsHeading.BackgroundColor = UIColor.LightGray;
			lblItemsHeading.TextColor = UIColor.Black;
			lblItemsHeading.Layer.CornerRadius = 10;
			lblItemsHeading.ClipsToBounds = true;
			lblItemsHeading.Font = UIFont.BoldSystemFontOfSize(20);
			mainView.Add(lblItemsHeading);

			//var addItemButton = new UIButton(new CGRect(0, lblItemsHeading.Frame.Y + lblItemsHeading.Frame.Height + 5, vFullWidth, vHeight));
			var addItemButton = new UIButton(new CGRect(0, lblItemsHeading.Frame.Y + lblItemsHeading.Frame.Height + 5, vFullWidth / 2 - 5, vHeight));
			addItemButton.ToButtonFormat("Search for Items");
			//NG : Add click
			addItemButton.TouchUpInside += ((sender, e) =>
				{
					//InvoiceService.isInvoiceSelected = true;
					MasterInventoryViewController mivc = this.Storyboard.InstantiateViewController("MasterInventoryViewController") as MasterInventoryViewController;
					mivc.isInvoiceSelected = true;
					this.NavigationController.PushViewController(mivc, true);
				});
			var btnAddbySKU = new UIButton(new CGRect(addItemButton.Frame.X + addItemButton.Frame.Width + 10, addItemButton.Frame.Y, vFullWidth / 2 - 10, vHeight));
			btnAddbySKU.SetTitle("Add by SKU", UIControlState.Normal);
			btnAddbySKU.BackgroundColor = UIColor.White;
			btnAddbySKU.Layer.BorderWidth = 1;
			btnAddbySKU.Layer.BorderColor = UIColor.Gray.CGColor;
			btnAddbySKU.SetTitleColor(UIColor.Gray, UIControlState.Normal);
			btnAddbySKU.ClipsToBounds = true;
			//rzee
			//NG : Add click
			btnAddbySKU.TouchUpInside += (object sender, EventArgs e) =>
			{
				;
				alertWithTextBox = UIAlertController.Create("Add Item", "Enter SKU Number", UIAlertControllerStyle.Alert);
				alertWithTextBox.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
				alertWithTextBox.AddAction(UIAlertAction.Create("Add Item", UIAlertActionStyle.Default, async (UIAlertAction obj) => await FetchItem()));
				alertWithTextBox.AddTextField((field) =>
				{
					field.Placeholder = "SKU";
					field.ToNumeric();
				});
				PresentViewController(alertWithTextBox, animated: true, completionHandler: null);
			};
			mainView.Add(addItemButton);
			mainView.Add(btnAddbySKU);
			mainView.UserInteractionEnabled = true;

			itemTableView = new UITableView(new CGRect(0, addItemButton.Frame.Y + vHeight + 10, vFullWidth, 150), UITableViewStyle.Grouped);
			itemTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			var tempList = objInvoice.Items.ToList();
			tempList.AddRange(itemsList.Where(x => x.AddNewFromInvoice).ToList());
			itemTableView.Source = new InvoiceItemTableSource(tempList, this);
			itemTableView.ReloadData();

			var headerView = new UIView(new CGRect(0, 0, currentFrame.Width - 240, 40));

			var lblItemCode = new UILabel(new CGRect(0, 0, columnW * 2, 40))
			{
				Text = "ITEM CODE",
			};
			lblItemCode.ToGreenLeafLabel();
			headerView.Add(lblItemCode);

			var lblMetalCode = new UILabel(new CGRect(lblItemCode.Frame.X + lblItemCode.Frame.Width - 1, 0, columnW * 2, 40))
			{
				Text = "METAL",
			};
			lblMetalCode.ToGreenLeafLabel();
			headerView.Add(lblMetalCode);

			var lblSKU = new UILabel(new CGRect(lblMetalCode.Frame.X + lblMetalCode.Frame.Width - 1, 0, columnW * 2, 40))
			{
				Text = "SKU",
			};
			lblSKU.ToGreenLeafLabel();
			headerView.Add(lblSKU);

			var lblAmount = new UILabel(new CGRect(lblSKU.Frame.X + lblSKU.Frame.Width - 1, 0, columnW * 3, 40))
			{
				Text = "AMOUNT",
			};
			lblAmount.ToGreenLeafLabel();
			headerView.Add(lblAmount);

			var lblDes = new UILabel(new CGRect(lblAmount.Frame.X + lblAmount.Frame.Width - 1, 0, columnW * 11, 40))
			{
				Text = "DESCRIPTION",
			};
			lblDes.ToGreenLeafLabel();
			headerView.Add(lblDes);
			itemTableView.TableHeaderView = headerView;
			mainView.Add(itemTableView);

			scrollView.AddSubview(mainView);

			//Payments
			var lblPaymentHeading = new UILabel(new CGRect(0, itemTableView.Frame.Y + itemTableView.Frame.Height - 10, vFullWidth, vHeight));//new UILabel(new CGRect(0, txtSalesPerson.Frame.Y + vHeight + 5, vFullWidth, vHeight));
			lblPaymentHeading.Text = "Payment";
			lblPaymentHeading.TextAlignment = UITextAlignment.Center;
			lblPaymentHeading.BackgroundColor = UIColor.LightGray;
			lblPaymentHeading.TextColor = UIColor.Black;
			lblPaymentHeading.Layer.CornerRadius = 10;
			lblPaymentHeading.ClipsToBounds = true;
			lblPaymentHeading.Font = UIFont.BoldSystemFontOfSize(20);
			mainView.Add(lblPaymentHeading);


			nfloat wdth = vFullWidth / 2;
			nfloat y = lblPaymentHeading.Frame.Y + 5 + lblPaymentHeading.Frame.Height;
			nfloat xLoc = 0;
			nfloat xSpacing = 20;


			//Item Sub Totale
			lblSubTotalOrder = new UILabel(new CGRect(0, y, wdth / 2, 30));
			lblSubTotalOrder.Text = String.Format("Items Total: {0}", objInvoice.TotalPrice.ToString("C"));
			mainView.Add(lblSubTotalOrder);

			//Taxes
			lblTax = new UILabel(new CGRect(wdth / 2, y, wdth / 2, 30));
			lblTax.Text = String.Format("Tax: {0}", ((double)objInvoice.LocationTax * objInvoice.TotalPrice).ToString("C"));
			mainView.Add(lblTax);

			//Ordertotal
			lblTotalOrder = new UILabel(new CGRect(wdth, y, wdth / 3, 30));
			lblTotalOrder.Text = String.Format("Total: {0}", objInvoice.OrderTotal.ToString("C"));
			mainView.Add(lblTotalOrder);

			//Amount Duee
			lblAmountDue = new UILabel(new CGRect(lblTotalOrder.Frame.X + lblTotalOrder.Frame.Width + 10, y, wdth - vHeight + 20, 30));
			lblAmountDue.Text = String.Format("Amount Due: {0}", ((double)objInvoice.OrderTotal - objInvoice.PaidAmount).ToString("C"));
			mainView.Add(lblAmountDue);

			//Payment View

			y = lblTotalOrder.Frame.Y + 5 + lblTotalOrder.Frame.Height;
			txtPaymentAmount = new UITextField(new CGRect(0, y, wdth / 2 - 10, vHeight));
			txtPaymentAmount.Placeholder = "Paid Amount";
			txtPaymentAmount.ToGreenLeafTextField();
			txtPaymentAmount.ToNumeric();
			mainView.Add(txtPaymentAmount);

			if (objInvoice != null)
				SetupAmounts();
			//	txtPaidAmount.Text = objInvoice.BalanceDue.ToString("C");

			txtPaymentType = new UITextField(new CGRect(wdth / 2, y, wdth / 2 - 10 - vHeight, vHeight));
			txtPaymentType.Placeholder = "Payment Type";
			txtPaymentType.ToGreenLeafTextField();
			txtPaymentType.Enabled = false;
			mainView.Add(txtPaymentType);

			var selectPaymentTypeButton = new UIButton(new CGRect(txtPaymentType.Frame.X + txtPaymentType.Frame.Width, txtPaymentType.Frame.Y, vHeight, vHeight));
			selectPaymentTypeButton.SetTitle("...", UIControlState.Normal);
			selectPaymentTypeButton.BackgroundColor = UIColor.Gray;
			selectPaymentTypeButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			selectPaymentTypeButton.TouchUpInside += (sender, e) =>
			{
				UIPopoverPresentationController presentationPopover = paymentTypeActionSheet.PopoverPresentationController;
				if (presentationPopover != null)
				{
					presentationPopover.SourceView = selectPaymentTypeButton;
					presentationPopover.SourceRect = selectPaymentTypeButton.Bounds;
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Any;
				}
				PresentViewController(paymentTypeActionSheet, true, null);

			};
			mainView.Add(selectPaymentTypeButton);

			txtApprovalCode = new UITextField(new CGRect(selectPaymentTypeButton.Frame.X + vHeight + 10, y, wdth / 3, vHeight));
			txtApprovalCode.Placeholder = "Approval Code";
			txtApprovalCode.ToGreenLeafTextField();
			txtApprovalCode.ToNumeric();
			mainView.Add(txtApprovalCode);

			txtPaymentDate = new UITextField(new CGRect(txtApprovalCode.Frame.X + txtApprovalCode.Frame.Width + 10, y, wdth / 3 - vHeight + 20, vHeight));
			txtPaymentDate.Placeholder = "Payment Date";
			txtPaymentDate.Enabled = false;
			txtPaymentDate.ToGreenLeafTextField();
			txtPaymentDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
			mainView.Add(txtPaymentDate);

			var selectPaymentDateButton = new UIButton(new CGRect(txtPaymentDate.Frame.X + txtPaymentDate.Frame.Width, txtPaymentDate.Frame.Y, vHeight, vHeight));
			selectPaymentDateButton.SetTitle("...", UIControlState.Normal);
			selectPaymentDateButton.BackgroundColor = UIColor.Gray;
			selectPaymentDateButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			selectPaymentDateButton.TouchUpInside += (sender, e) =>
			{
				PresentViewController(modalPicker, true, null);
			};
			mainView.Add(selectPaymentDateButton);

			var addPaymentButton = new UIButton(new CGRect(selectPaymentDateButton.Frame.X + vHeight + 10, selectPaymentDateButton.Frame.Y, wdth / 3 - 45, vHeight));
			addPaymentButton.ToButtonFormat("Add");
			mainView.Add(addPaymentButton);

			addPaymentButton.TouchUpInside += ((sender, e) =>
			{
				if (txtPaymentType.Text.Trim() == null || txtPaymentType.Text == string.Empty)
				{
					var alert = new UIAlertView("Payment Type Required", "Can't add a payment without a payment type.", null, "Ok");
					alert.Show();
					return;
				}
				//DJ adding payment
				var payment = new Payment();
				payment.PaidAmount = Convert.ToDecimal(Regex.Replace(txtPaymentAmount.Text, @"[$\(\)@-]", ""));
				payment.Date = txtPaymentDate.Text;
				payment.ApprovalCode = txtApprovalCode.Text;
				payment.Add = true;
				payment.Remove = false;
				payment.Update = false;

				//Should update due balance
				var paymentType = new PaymentType();
				if (AzureService.InvoiceService.PaymentTypes != null)
					paymentType = AzureService.InvoiceService.PaymentTypes.FirstOrDefault(x => x.Name.Trim().ToUpper() == txtPaymentType.Text.Trim().ToUpper());

				payment.PaymentType = paymentType;
				payment.PaymentTypeId = paymentType?.Id;
				paymentList.Add(payment);
				paymentTable.Source = new InvoicePaymentTableSource(paymentList, this);
				paymentTable.ReloadData();

				//Clear textBoxes
				//txtPaidAmount.Text = string.Empty;
				SetupAmounts();
				txtPaymentDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
				txtPaymentType.Text = string.Empty;
				txtApprovalCode.Text = string.Empty;

			});
			paymentTable = new UITableView(new CGRect(0, selectPaymentDateButton.Frame.Y + vHeight + 10, vFullWidth, vHeight * 3 - 30));
			paymentTable.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			paymentTable.RowHeight = UITableView.AutomaticDimension;
			paymentTable.EstimatedRowHeight = 35f;


			mainView.Add(paymentTable);

			//Save Button
			var saveButton = new UIButton(new CGRect(120, paymentTable.Frame.Height + paymentTable.Frame.Y + 20, currentFrame.Width / 2 - 120, 50));
			saveButton.SetTitle("Save", UIControlState.Normal);
			saveButton.BackgroundColor = UIColor.White;
			saveButton.Layer.BorderWidth = 1;
			saveButton.Layer.BorderColor = UIColor.Gray.CGColor;
			saveButton.SetTitleColor(UIColor.Gray, UIControlState.Normal);
			saveButton.ClipsToBounds = true;
			//NG : Save Invoice
			saveButton.TouchUpInside += ((sender, e) =>
				{
					SaveInvoice();
				});
			scrollView.Add(saveButton);

			//Cancel Button
			var cancelButton = new UIButton(new CGRect(currentFrame.Width / 2 + 10, saveButton.Frame.Y, currentFrame.Width / 2 - 130, 50));
			cancelButton.SetTitle("Cancel", UIControlState.Normal);
			cancelButton.BackgroundColor = UIColor.White;
			cancelButton.Layer.BorderWidth = 1;
			cancelButton.Layer.BorderColor = UIColor.Gray.CGColor;
			cancelButton.SetTitleColor(UIColor.Gray, UIControlState.Normal);
			cancelButton.ClipsToBounds = true;
			scrollView.Add(cancelButton);
			cancelButton.TouchUpInside += ((sender, e) =>
			{
				UIAlertController alert = UIAlertController.Create("Leaving the page?", "If you leave the page before saving," +
																	   " all changes will be lost.", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
				{
				}));
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
				{
					//DJ Cancel button click
					foreach (var item in AzureService.InvoiceService.ItemsList)
					{
						if (item.AddNewFromInvoice)
						{
							item.AddNewFromInvoice = false;
							AzureService.InventoryService.MarkItemSoldAsync(item, false);
						}
					}
					foreach (var item in itemListToRemove)
					{
						if (item.RemoveFromInvoice)
						{
							AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == item.Id).RemoveFromInvoice = false;
						}
					}
					paymentListToRemove = new List<Payment>();
					paymentList = new List<Payment>();
					itemsList = new List<Item>();
					itemListToRemove = new List<Item>();
					AzureService.InvoiceService.ItemsList = new List<Item>();
					this.NavigationController.PopViewController(true);
					this.NavigationController.PopViewController(true);
				}));
				PresentViewController(alert, true, null);
			});
		}
		private void updateAmounts()
		{
			//bool isMailOrder = isMailOrderSwitch.On;

			if (paymentList != null)
			{
				paidAmount = Convert.ToDouble(paymentList.Sum(x => x.PaidAmount));
			}

			if (itemsList != null)
			{
				var templist = itemsList.Where(x => x.AddNewFromInvoice).ToList();
				templist.AddRange(objInvoice.Items);
				templist = templist.Distinct().ToList();
				itemsTotal = Convert.ToDouble(templist.Sum(x => x.ItemSalePrice));
			}

			if (isMailOrderSwitch.On)
			{
				if (this.txtLocation.Text == "LA" && objInvoice.Customer != null && objInvoice.Customer.Province != null &&
					(objInvoice.Customer.Province.Trim().Replace(".", "").ToLower() != "ca" && objInvoice.Customer.Province.Trim().ToLower() != "california"))
				{
					tax = 0;
				}
				else
				{
					tax = Math.Round((double)(itemsTotal * (double)objInvoice.LocationTax), 2);
				}
			}
			else
			{
				tax = Math.Round((double)(itemsTotal * (double)objInvoice.LocationTax), 2);
			}

			total = itemsTotal + tax;
			amountDue = total - paidAmount;
		}

		private void updateAmountLabels()
		{

			lblAmountDue.Text = String.Format("Amount Due: {0}", amountDue.ToString("C"));
			lblSubTotalOrder.Text = String.Format("Items Sub Total: {0}", itemsTotal.ToString("C"));
			lblTax.Text = String.Format("Tax: {0}", tax.ToString("C"));
			lblTotalOrder.Text = String.Format("Total: {0}", total.ToString("C"));

			txtPaymentAmount.Text = (total - (double)paidAmount).ToString("C");
		}
		private void updateAmountsBasedOnTaxIn()
		{
			updateAmounts();
			var tempList = itemsList.Where(x => x.AddNewFromInvoice).ToList();
			tempList.AddRange(objInvoice.Items);
			tempList = tempList.Distinct().ToList();

			if (taxInSwitch.On)//take the discount out
			{
				pricePerItemToDeductForTaxIn = Math.Round(tax / tempList.Count(), 2);
				foreach (var item in tempList)
				{
					item.Price = item.ItemSalePrice - pricePerItemToDeductForTaxIn;
				}
			}
			else //add it back in
			{

				foreach (var item in tempList)
				{
					item.Price = item.ItemSalePrice + pricePerItemToDeductForTaxIn;
				}
			}

			itemTableView.Source = new InvoiceItemTableSource(tempList, this);
			itemTableView.ReloadData();
			SetupAmounts();
		}

		public void SetupAmounts()
		{
			updateAmounts();
			updateAmountLabels();
		}
		private async void SaveInvoice()
		{

			//Checking is user attemp payment insertion 
			if (txtPaymentType != null && txtPaymentType.Text.Length > 0)
			{
				var alert = new UIAlertView("Payment Required", "Please add payment", null, "Ok");
				alert.Show();
				return;
			}
			ShowLoading();

			objInvoice.IsMailOrder = isMailOrderSwitch.On;
			if (paymentList != null)
			{
				objInvoice.PaidAmount = Convert.ToDouble(paymentList.Sum(x => x.PaidAmount));
				paymentList.AddRange(paymentListToRemove);
				objInvoice.Payments = paymentList;
			}


			if (itemsList.Count == 0 && objInvoice.Items.Count == 0)
			{
				loadingOverlay.Hide();
				var alert = new UIAlertView("Items Required", "Invoice can't be created without items.", null, "Ok");
				alert.Show();
				return;
			}
			objInvoice.PaidAmount = 0;
			objInvoice.OrderTotal = 0;
			if (objInvoice.Items != null)
			{
				var templist = itemsList.Where(x => x.AddNewFromInvoice).ToList();
				templist.AddRange(objInvoice.Items);
				objInvoice.Items = templist;
				objInvoice.OrderTotal = Convert.ToDecimal(objInvoice.Items.Sum(x => x.ItemSalePrice));
				templist.AddRange(itemListToRemove);
				objInvoice.Items = templist;
			}
			else
			{
				objInvoice.Items = itemsList;
				objInvoice.OrderTotal = Convert.ToDecimal(objInvoice.Items.Sum(x => x.ItemSalePrice));
				itemsList.AddRange(itemListToRemove);
				objInvoice.Items = itemsList;
			}

			objInvoice.TotalPrice = Math.Round((double)(objInvoice.OrderTotal + (decimal)(objInvoice.OrderTotal * objInvoice.LocationTax)), 2);

			if (txtStatus.Text == "Cancelled")
				objInvoice.IsCancelled = true;
			else
				objInvoice.IsCancelled = false;

			try
			{
				AzureService.InvoiceService.ItemsList?.Clear();
				AzureService.InvoiceService.IsInvoiceSelected = false;
				if (objInvoice.Payments != null)
				{
					foreach (var item in objInvoice.Payments)
					{
						if (item.PaymentType == null)
						{
							loadingOverlay.Hide();
							var alert = new UIAlertView("Payment Type Required", "One payment is missing a payment type", null, "Ok");
							alert.Show();
							return;
						}
						if (item.Id == string.Empty)
							item.Id = Guid.NewGuid().ToString();
					}
				}

				this.NavigationItem.HidesBackButton = true;
				this.NavigationItem.LeftBarButtonItem = null;

				await AzureService.InvoiceService.InsertOrSaveInvoice(objInvoice);
				itemsList = new List<Item>();
				itemListToRemove = new List<Item>();

			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(@"ERROR {0}", ex.Message);
			}
			loadingOverlay.Hide();
			this.NavigationItem.HidesBackButton = false;
			this.NavigationController.PopViewController(true);
			this.NavigationController.PopViewController(true);
		}

		async Task FetchItem()
		{
			ShowLoading();
			try
			{
				if (AzureService.InventoryService.Items != null && alertWithTextBox != null)
				{
					var item = new Item();
					string sku = alertWithTextBox.TextFields[0].Text.Trim();
					if (AzureService.InventoryService.Items.Any(x => x.Sku.Equals(sku) && !x.IsSold))
					{
						item = AzureService.InventoryService.Items.FirstOrDefault(x => x.Sku.Equals(sku) && !x.IsSold);
						await AzureService.InventoryService.MarkItemSoldAsync(item, true);
						item.IsSold = true;
						item.AddNewFromInvoice = true;
						AzureService.InvoiceService.ItemsList.Insert(0, item);
						AddItems();
					}
					else
					{
						loadingOverlay.Hide();
						var alert = new UIAlertView("Error", "Item is sold or doesn't exist.", null, "Ok");
						alert.Show();
						return;
					}
				}
			}
			catch (Exception ex)
			{
				//RMR TODO Umm this is bad?
			}
			finally
			{
				loadingOverlay.Hide();
			}
		}

		void AddItems()
		{
			if (AzureService.InvoiceService.ItemsList != null && AzureService.InvoiceService.ItemsList.Count > 0)
			{
				foreach (var item in AzureService.InvoiceService.ItemsList)
				{
					if (!itemsList.Any(x => x.Id == item.Id))
					{
						itemsList.Add(item);
						UIAlertController alert = UIAlertController.Create("Enter Sale Price", "", UIAlertControllerStyle.Alert);
						UITextField field = null;
						UITextField descriptionField = null;
						alert.AddTextField((textField) =>
						{
							field = textField;
							//field.Text = item.TagPrice.ToString();
							field.Placeholder = "Sale Price";
							field.AutocorrectionType = UITextAutocorrectionType.No;
							field.KeyboardType = UIKeyboardType.NumberPad;
							field.ReturnKeyType = UIReturnKeyType.Done;
							field.ClearButtonMode = UITextFieldViewMode.WhileEditing;

						});

						_AddedItemID = item.Id; ;
						alert.AddAction(UIAlertAction.Create("Next", UIAlertActionStyle.Default, (actionOK) =>
						{
							double newPrice = 0;

							Double.TryParse(field.Text, out newPrice);



							if (newPrice > 0)
							{
								itemsList.FirstOrDefault(x => x.Id == _AddedItemID).Price = newPrice;
							}
							else
							{
								UIAlertController innerAlert = UIAlertController.Create("Invalid Sale Price Input", "Please enter a valid number", UIAlertControllerStyle.Alert);

								PresentViewController(innerAlert, true, null);
								innerAlert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (actionInnerOk) =>
								{
									PresentViewController(alert, true, null);
								}));
								return;
							}

							UIAlertController descAlert = UIAlertController.Create("Update Description for Invoice", "", UIAlertControllerStyle.Alert);

							descAlert.AddTextField((textField) =>
						{
							descriptionField = textField;
							descriptionField.Text = item.Description;
							descriptionField.AutocorrectionType = UITextAutocorrectionType.Yes;
							descriptionField.KeyboardType = UIKeyboardType.NumberPad;
							descriptionField.ReturnKeyType = UIReturnKeyType.Done;
							descriptionField.ClearButtonMode = UITextFieldViewMode.WhileEditing;

						});
							descAlert.AddAction(UIAlertAction.Create("Done", UIAlertActionStyle.Default, (actionDone) =>
						{
							itemsList.FirstOrDefault(x => x.Id == _AddedItemID).InvoiceDescription = descriptionField.Text;
							var tempList = objInvoice.Items.ToList();
							tempList.AddRange(itemsList.Where(x => x.AddNewFromInvoice).ToList());
							itemTableView.Source = new InvoiceItemTableSource(tempList, this);
							itemTableView.ReloadData();

							SetupAmounts();
						}));
							PresentViewController(descAlert, true, null);



						}));
						PresentViewController(alert, true, null);
					}
				}
			}
			if (objInvoice.Items != null && itemTableView != null)
			{
				//update description
				foreach (var item in objInvoice.Items)
					if (AzureService.InventoryService.Items.Any(x => x.Id == item.Id))
						item.InvoiceDescription = AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == item.Id).InvoiceDescription;

				var tempList = objInvoice.Items.ToList();
				tempList.AddRange(itemsList.Where(x => x.AddNewFromInvoice).ToList());
				//var balanceDue = Convert.ToDouble(itemsList.Sum(x => x.ItemSalePrice));
				//balanceDue = balanceDue + objInvoice.BalanceDue;
				//txtPaidAmount.Text = balanceDue.ToString("C");
				itemTableView.Source = new InvoiceItemTableSource(tempList, this);
				itemTableView.ReloadData();
				SetupAmounts();
			}
			if (objInvoice.Payments != null && objInvoice.Payments.Count > 0)
			{
				paymentList = objInvoice.Payments.ToList();
				paymentTable.Source = new InvoicePaymentTableSource(paymentList, this);
				paymentTable.ReloadData();
			}

		}

		private bool PaymentsChanged = false;
		private bool ItemsChanged = false;
		private bool StatusChanged = false;
		private bool haveChangedBeenMade()
		{
			var templist = objInvoice.Items.ToList();

			if (tempitemsList.Count != templist.Count)
				ItemsChanged = true;
			else {
				for (int i = 0; i < tempitemsList.Count; i++)
				{
					if (tempitemsList[i] != templist[i])
					{
						ItemsChanged = true;
						break;
					}
				}
			}
			if (tempPaymentList.Count != paymentList.Count)
				PaymentsChanged = true;
			else {
				for (int i = 0; i < tempitemsList.Count; i++)
				{
					if (tempPaymentList[i] != paymentList[i])
					{
						PaymentsChanged = true;
						break;
					}
				}
			}
			if (PaymentsChanged || ItemsChanged || StatusChanged ||
				objInvoice.IsMailOrder != isMailOrderSwitch.On ||
				objInvoice.Location.Name != txtLocation.Text ||
				 objInvoice.Customer.FullName != txtCustomer.Text ||
				objInvoice.User.FullName != txtSalesPerson.Text ||
			    objStatus!= txtStatus.Text ||
			txtLocation.Text != objInvoice.LocationName ||
			txtSalesPerson.Text != objInvoice.User?.FullName ||
			    txtCustomer.Text != objInvoice.Customer?.FirstName ||

				objInvoice.TotalPrice != Math.Round((double)(objInvoice.OrderTotal + (decimal)(objInvoice.OrderTotal * objInvoice.LocationTax)), 2)
				)
			{
				return true;
			}
			return false;
		}
	}
}