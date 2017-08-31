using Foundation;
using System;
using System.Linq;
using System.CodeDom.Compiler;
using UIKit;
using Infragistics;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;
using Syncfusion.SfDataGrid;
using CoreGraphics;
using System.Windows.Input;
using System.IO;


namespace GreenleafiManager
{
	public partial class MasterInvoiceViewController : UIViewController
	{
		public static LoadingOverlay loadingOverlay;
		CGRect currentViewRectangle;

		TLTagsControl.TLTagsControl txtOrderDate, txtLocation, txtCustomerNumber;
		UITextField txtItem, txtOrderTotal, txtSku;
		ModalPickerViewController modalPicker;

		IGGridView gridView;
		SfDataGrid dataGrid;
		UISearchBar filterText = new UISearchBar();
		public Invoice SelectedItem { get; set; }

		UIAlertController locationActionSheet;
		public static IGCellPath ScrollToCell;

		private UIView _slideView;
		private UIButton _button;
		UIView filterSlideView = null;
		bool isVisible;
		public static List<Invoice> InvoiceList = new List<Invoice>();
		UIButton filterButton;

		UIView filterView;


		public MasterInvoiceViewController(IntPtr handle) : base(handle)
		{
			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ScrollToCell = null;
			this.NavigationItem.HidesBackButton = true;
			currentViewRectangle = this.View.Frame;
		}

		public override async void ViewDidAppear(bool animated)
		{
			ShowLoading();
			if (gridView != null)
			{
				await UpdateInvoiceList();
				gridView.WeakDataSource = new InvoiceGridDataSource(InvoiceList);
				gridView.UpdateData();
			}

			base.ViewDidAppear(animated);
			await CreateLocationActionSheet();
			CreateDateActionSheet();
			await CreateInvoiceView();
			filterSlideView.Hidden = true;

			if (ScrollToCell != null)
			{
				gridView.ScrollToCell(ScrollToCell, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
			}

			HideLoading();
		}
		private void filterCurrentData()
		{
			if (gridView != null)
			{
				gridView.WeakDataSource = new InvoiceGridDataSource(InvoiceList.Where(x=>x.SearchString.ToLower().Contains(filterText.Text.ToLower())).ToList());
				gridView.UpdateData();
			}
		}

		//Creating new view for invoice: rzee
		async Task CreateInvoiceView()
		{
			//Parent View
			var parentView = new UIView(new CGRect(0, 65, currentViewRectangle.Width, currentViewRectangle.Height - 65));
			//parentView.BackgroundColor = UIColor.Yellow; //Should Remove Later

			//Search View Container
			var searchViewContainer = new UIView(new CGRect(0, 0, currentViewRectangle.Width, 100));
			searchViewContainer.BackgroundColor = UIColor.Clear;

			//Filter Slide View
			filterSlideView = GetFilterSlideView();
			filterSlideView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.6f);
			filterSlideView.Opaque = false;

			var filterY = searchViewContainer.Frame.Y + 65;
			var filterW = currentViewRectangle.Width / 3;
			var prevFrame = new CGRect(-filterW, filterY, filterW, currentViewRectangle.Height - filterY);
			filterSlideView.Frame = prevFrame;
			filterSlideView.Add(GetFilterChildView(new CGRect(0, filterY, filterW, currentViewRectangle.Height)));

			//Filter Button
			filterButton = filterButton ?? new UIButton(new CGRect(10, 10, 150, 60));
			filterButton.SetTitle("Filter", UIControlState.Normal);
			filterButton.BackgroundColor = UIColor.LightGray;
			//searchViewContainer.Add(filterButton);
			filterButton.TouchUpInside += (sender, e) =>
			{
			};

			//Added SearchView
			searchViewContainer.Add(GetSearchView());

			//Added Search view to parent view
			parentView.Add(searchViewContainer);

			//Adding Grid
			parentView.InsertSubviewBelow(await CreateGrid(), searchViewContainer);

			//Added Filter View
			parentView.Add(filterSlideView);

			//Adding parentView to controller view
			Add(parentView);

			//Create Invoice Button
			CreateAddNewButton();

			var createInvoiceButton = new UIButton(new CGRect(10, 80, currentViewRectangle.Width - 20, 60));
			createInvoiceButton.BackgroundColor = UIColor.LightGray;
			createInvoiceButton.SetTitle("+ Create Invoice", UIControlState.Normal);

		}

		//Search Text Box 
		UIView GetSearchView()
		{
			filterText.Frame = new CGRect(0, 0, currentViewRectangle.Width - 0, 60);//(170, 10, currentViewRectangle.Width - 180, 60);
			filterText.TextChanged += OnFilterTextChanged;
			filterText.Placeholder = "Search Invoices";
			int hex = 0xCECED2;

			filterText.BackgroundColor = UIColor.Clear.FromHex(hex);
			filterText.TintColor = UIColor.Clear.FromHex(hex);
			filterText.BarTintColor = UIColor.Clear.FromHex(hex);
			filterText.Layer.BorderWidth = 1;
			filterText.Layer.BorderColor = Extensions.CGColorFromHex(hex);
			AzureService.InvoiceService.FilterTextChanged = OnFilterChanged;

			return filterText;
		}

		//Filter Slider View
		UIView GetFilterSlideView()
		{
			filterView = new UIView();
			return filterView;
		}

		//Filterview 
		UIView GetFilterChildView(CGRect filterViewFrame)
		{
			float vX = 40;
			float vY = 20;
			float vW = (float)filterViewFrame.Width - 80;
			float vH = 40;
			float padTop = 10 + vY;

			var mainChildView = new UIView(filterViewFrame);


			txtCustomerNumber = new TLTagsControl.TLTagsControl();//, new NSObject[] { NSObject.FromObject("Tag"), NSObject.FromObject("Hai"), NSObject.FromObject("Yaar"), NSObject.FromObject("Samjha"), NSObject.FromObject("Kar") }, TLTagsControl.TLTagsControlMode.Edit);
			txtCustomerNumber.BackgroundColor = UIColor.White;
			txtCustomerNumber.Frame = new CGRect(vX, vY, vW, vH);
			txtCustomerNumber.TagPlaceholder = "Customer Number";
			txtCustomerNumber.TagsBackgroundColor = UIColor.LightGray;
			txtCustomerNumber.TagsTextColor = UIColor.White;
			txtCustomerNumber.TagsDeleteButtonColor = UIColor.Green;
			mainChildView.Add(txtCustomerNumber);

			//2nd
			txtOrderDate = new TLTagsControl.TLTagsControl();//new CGRect(vX, vH + vY, vW, vH), new NSObject[] { NSObject.FromObject("") }, TLTagsControl.TLTagsControlMode.List);
			txtOrderDate.BackgroundColor = UIColor.White;
			txtOrderDate.Mode = TLTagsControl.TLTagsControlMode.Edit;
			txtOrderDate.Frame = new CGRect(vX, vH + vY + 5, vW, vH);
			txtOrderDate.TagPlaceholder = "Order Date";
			txtOrderDate.TagsBackgroundColor = UIColor.LightGray;
			txtOrderDate.TagsTextColor = UIColor.White;
			txtOrderDate.TagsDeleteButtonColor = UIColor.Green;
			var text = txtOrderDate.Subviews?.Where(x => x.GetType() == typeof(UITextField)).FirstOrDefault() as UITextField;
			text.ShouldBeginEditing = (txt) =>
			{
				return false;
			};
			//txtor.Enabled = false;
			mainChildView.Add(txtOrderDate);

			var selectDateButton = new UIButton(new CGRect(txtOrderDate.Frame.Width, txtOrderDate.Frame.Y, vH, vH));
			selectDateButton.SetTitle("...", UIControlState.Normal);
			selectDateButton.BackgroundColor = UIColor.Gray;
			selectDateButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			selectDateButton.TouchUpInside += (sender, e) =>
			{
				PresentViewController(modalPicker, true, null);
			};
			mainChildView.Add(selectDateButton);

			txtLocation = new TLTagsControl.TLTagsControl();//, new NSObject[] { NSObject.FromObject("Tag"), NSObject.FromObject("Hai"), NSObject.FromObject("Yaar"), NSObject.FromObject("Samjha"), NSObject.FromObject("Kar") }, TLTagsControl.TLTagsControlMode.Edit);
			txtLocation.BackgroundColor = UIColor.White;
			txtLocation.Mode = TLTagsControl.TLTagsControlMode.Edit;
			txtLocation.Frame = new CGRect(vX, vH * 2 + padTop, vW, vH);
			txtLocation.TagPlaceholder = "Location";
			txtLocation.TagsBackgroundColor = UIColor.LightGray;
			txtLocation.TagsTextColor = UIColor.White;
			txtLocation.TagsDeleteButtonColor = UIColor.Green;
			if (txtLocation.Tags == null)
				txtLocation.Tags = new NSMutableArray();
			mainChildView.Add(txtLocation);

			var selectLocationButton = new UIButton(new CGRect(txtLocation.Frame.Width, txtLocation.Frame.Y, vH, vH));
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
					presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Down;
				}
				PresentViewController(locationActionSheet, true, null);
			};
			mainChildView.Add(selectLocationButton);

			//5th Item
			txtItem = new UITextField(new CGRect(vX, vH * 3 + padTop + 5, vW, vH));
			txtItem.Placeholder = "Item";
			txtItem.ToGreenLeafTextField();
			mainChildView.Add(txtItem);

			//6th Item Text
			txtOrderTotal = new UITextField(new CGRect(vX, vH * 4 + padTop + 10, vW, vH));
			txtOrderTotal.Placeholder = "Order Total";
			txtOrderTotal.ToGreenLeafTextField();
			txtOrderTotal.KeyboardType = UIKeyboardType.NumberPad;
			mainChildView.Add(txtOrderTotal);

			//7th Item Text
			txtSku = new UITextField(new CGRect(vX, vH * 5 + padTop + 13, vW, vH));
			txtSku.Placeholder = "Sku";
			txtSku.ToGreenLeafTextField();
			mainChildView.Add(txtSku);

			//Apply Button
			var applyButton = new UIButton(new CGRect(vX, vH * 7 + padTop, vW, vH));
			applyButton.SetTitle("Apply", UIControlState.Normal);
			applyButton.BackgroundColor = UIColor.Blue;
			applyButton.TouchUpInside += /*async*/ (object sender, EventArgs e) =>
 			{
				 var filterItem = new InvoiceFilter();

				 //TODO: Apply Filter Here
				 //Invoice Numbers
				 if (txtCustomerNumber != null && txtCustomerNumber.Tags != null)
				 {
					 for (nuint i = 0; i < txtCustomerNumber.Tags.Count; i++)
					 {
						 filterItem.Numbers.Add(txtCustomerNumber.Tags.GetItem<NSString>(i));
					 }
				 }
				 //Order Dates
				 if (txtOrderDate != null && txtOrderDate.Tags != null)
				 {
					 for (nuint i = 0; i < txtOrderDate.Tags.Count; i++)
					 {
						 filterItem.OrderDates.Add(Convert.ToDateTime(txtOrderDate.Tags.GetItem<NSString>(i)));
					 }
				 }
				 //Locations
				 if (txtLocation != null && txtLocation.Tags != null)
				 {
					 for (nuint i = 0; i < txtLocation.Tags.Count; i++)
					 {
						 filterItem.Locations.Add(txtLocation.Tags.GetItem<NSString>(i));
					 }
				 }
				 //Order Itemm
				 if (txtItem != null && !string.IsNullOrEmpty(txtOrderTotal.Text))
				 {
					 filterItem.Item = txtItem.Text;
				 }
				 //Order Total
				 if (txtOrderTotal != null && !string.IsNullOrEmpty(txtOrderTotal.Text))
				 {
					 filterItem.OrderTotal = Convert.ToDecimal(txtOrderTotal.Text);
				 }
				 //Order SKU
				 if (txtSku != null && !string.IsNullOrEmpty(txtSku.Text))
				 {
					 filterItem.Sku = txtSku.Text;
				 }
				 //Apply Filter Code From Server
			 };
			mainChildView.Add(applyButton);

			//Cancel Filter
			var cancelButton = new UIButton(new CGRect(vX, vH * 9 + padTop, vW, vH));
			cancelButton.SetTitle("Hide Filter", UIControlState.Normal);
			cancelButton.BackgroundColor = UIColor.Blue;
			cancelButton.TouchUpInside += (sender, e) => { filterButton.SendActionForControlEvents(UIControlEvent.TouchUpInside); };
			mainChildView.Add(cancelButton);

			return mainChildView;
		}
		private void ShowLoading()
		{
			this.NavigationItem.HidesBackButton = true;
			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
			View.Add(loadingOverlay);
		}

		private void HideLoading()
		{
			loadingOverlay.Hide();
			this.NavigationItem.HidesBackButton = false;
		}

		private async Task UpdateInvoiceList()
		{
			if (AzureService.InvoiceService.Invoices == null || AzureService.InvoiceService.Invoices.Count == 0)
			{
				await AzureService.InvoiceService.UpdateInvoicesFromAzure();
			}
			if (InvoiceList != null)
				InvoiceList.Clear();

			InvoiceList = AzureService.InvoiceService.Invoices.OrderByDescending(x => x.Number).ToList();

			//await AzureService.InventoryService.ReloadLocalNSItems();
			if (AzureService.InventoryService.Items == null || AzureService.InventoryService.Items.Count == 0)
			{
				await AzureService.InventoryService.UpdateItemsFromAzure();
			}

			if (AzureService.CustomerService.Customers == null)
			{
				await AzureService.CustomerService.UpdateCustomersFromAzure();
			}

			if (AzureService.LocationService.Locations == null)
			{
				await AzureService.LocationService.UpdateLocationsFromAzure();
			}

			if (AzureService.UserService.Users == null)
			{
				await AzureService.UserService.UpdateUsersFromAzure();
			}

			foreach (var row in InvoiceList)
			{
				row.Customer = AzureService.CustomerService.Customers.FirstOrDefault(x => x.Id == row.CustomerId);
				//row.Location = AzureService.LocationService.Locations.FirstOrDefault(x => x.Id == row.LocationId);
				row.User = AzureService.UserService.Users.FirstOrDefault(x => x.Id == row.UserId);
				var payment = AzureService.InvoiceService.Payments.Where(x => x.InvoiceId == row.Id).ToList();
				row.Payments = payment;

				//Setting Paid / Balance amountt
				var amnt = Convert.ToDouble(payment.Sum(x => x.PaidAmount));
				row.PaidAmount = amnt;
				row.TotalPrice = Math.Round((double)(row.OrderTotal + (decimal)(row.OrderTotal * row.LocationTax)),2);

				row.Items = new List<Item>();
				if (AzureService.InvoiceService.InvoiceItems.Any(x => x.Invoice_Id == row.Id))
				{
					var inv = AzureService.InvoiceService.InvoiceItems.Where(x => x.Invoice_Id == row.Id).ToList();
					foreach (var items in inv)
						if (AzureService.InventoryService.Items.Any(x => x.Id == items.Item_Id))
							row.Items.Add(AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == items.Item_Id));
				}

				foreach (var pay in row.Payments)
				{
					pay.PaymentType = AzureService.InvoiceService.PaymentTypes.FirstOrDefault(x => x.Id == pay.PaymentTypeId);
				}
			}
		}

		private void CreateAddNewButton()
		{
			var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;
		}

		private async Task<UIView> CreateGrid()
		{
			await UpdateInvoiceList();

			gridView = gridView ?? new IGGridView(new CGRect(0, 75, this.View.Frame.Size.Width, this.View.Frame.Size.Height - 80), IGGridViewStyle.IGGridViewStyleDefault);
			gridView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			gridView.WeakDataSource = new InvoiceGridDataSource(InvoiceList);
			gridView.RowHeight = 200;
			gridView.WeakDelegate = new InvoiceDelegate(this);
			//gridView.RegisterClassForCellReuse(new Class("NewInvoiceGridCell"), "Cell");
			gridView.UpdateData();
			//this.View.AddSubview(gridView);

			return gridView;
		}

		private void SetupGrid()
		{
			dataGrid.HeaderRowHeight = 45;
			dataGrid.RowHeight = 45;

			dataGrid.Frame = new CGRect(0, 100, View.Frame.Width, View.Frame.Height - 115);
			dataGrid.AutoGenerateColumns = false;
			dataGrid.ColumnSizer = ColumnSizer.Star;

			GridTextColumn numberColumn = new GridTextColumn();
			numberColumn.MappingName = "Number";
			numberColumn.HeaderText = "Number";

			GridTextColumn dateColumn = new GridTextColumn();
			dateColumn.MappingName = "Date";
			dateColumn.HeaderText = "Date";
		}

		void DataGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
		{
			var obj = dataGrid.SelectedItem;
			SelectedItem = (Invoice)obj;
			InvoiceDetailViewController InvoiceDetailViewController = this.Storyboard.InstantiateViewController("InvoiceDetailViewController") as InvoiceDetailViewController;
			InvoiceDetailViewController.Invoice = SelectedItem;
			InvoiceDetailViewController.MasterViewController = this;
			InvoiceDetailViewController.EditOrCreate = "updated";

			this.NavigationController.PushViewController(InvoiceDetailViewController, true);
		}

		void OnFilterTextChanged(object sender, UISearchBarTextChangedEventArgs e)
		{
			AzureService.InvoiceService.FilterText = e.SearchText;
		}

		private void OnFilterChanged()
		{
			if (gridView != null)// && dataGrid.View != null)
			{
				filterCurrentData();
			}
		}

		public class RelayCommand : ICommand
		{
			private Action<object> execute;

			private Predicate<object> canExecute;

			private event EventHandler CanExecuteChangedInternal;

			public RelayCommand(Action<object> execute)
				: this(execute, DefaultCanExecute)
			{
			}

			public RelayCommand(Action<object> execute, Predicate<object> canExecute)
			{
				if (execute == null)
				{
					throw new ArgumentNullException("execute");
				}

				if (canExecute == null)
				{
					throw new ArgumentNullException("canExecute");
				}

				this.execute = execute;
				this.canExecute = canExecute;
			}

			public event EventHandler CanExecuteChanged
			{
				add
				{
					//CommandManager.RequerySuggested += value;
					this.CanExecuteChangedInternal += value;
				}

				remove
				{
					//CommandManager.RequerySuggested -= value;
					this.CanExecuteChangedInternal -= value;
				}
			}

			public bool CanExecute(object parameter)
			{
				return this.canExecute != null && this.canExecute(parameter);
			}

			public void Execute(object parameter)
			{
				this.execute(parameter);
			}

			public void OnCanExecuteChanged()
			{
				EventHandler handler = this.CanExecuteChangedInternal;
				if (handler != null)
				{
					//DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
					handler.Invoke(this, EventArgs.Empty);
				}
			}

			public void Destroy()
			{
				this.canExecute = _ => false;
				this.execute = _ => { return; };
			}

			private static bool DefaultCanExecute(object parameter)
			{
				return true;
			}
		}

		private async Task ExecutePullToRefresh()
		{
			dataGrid.IsBusy = true;
			await AzureService.InvoiceService.UpdateInvoicesFromAzure();
			dataGrid.ItemsSource = AzureService.InvoiceService.Invoices;
			dataGrid.SortColumnDescriptions.Clear();
			dataGrid.SortColumnDescriptions.Add(new SortColumnDescription()
			{
				ColumnName = "Date",
				SortDirection = Syncfusion.Data.ListSortDirection.Descending
			});
			this.dataGrid.IsBusy = false;
		}
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showInventoryDetail")
			{
				var controller = (InvoiceDetailViewController)((UINavigationController)segue.DestinationViewController).TopViewController;
				controller.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
				controller.NavigationItem.LeftItemsSupplementBackButton = true;
			}
		}
		void AddNewItem(object sender, EventArgs args)
		{
			//InvoicePreviewViewController InvoiceDetailView = this.Storyboard.InstantiateViewController("InvoicePreviewViewController") as InvoicePreviewViewController;
			//this.NavigationController.PushViewController(InvoiceDetailView, true);

			InvoiceCreateViewController InvoiceCreateView = this.Storyboard.InstantiateViewController("InvoiceCreateViewController") as InvoiceCreateViewController;
			this.NavigationController.PushViewController(InvoiceCreateView, true);


		}

		async Task CreateLocationActionSheet()
		{
			locationActionSheet = UIAlertController.Create("Location", "Select Location", UIAlertControllerStyle.ActionSheet);

			await AzureService.DefaultService.LocationSetupConfirmationWithSync();
			foreach (var item in AzureService.LocationService.Locations)
			{
				locationActionSheet.AddAction(UIAlertAction.Create(item.City, UIAlertActionStyle.Default, (action) => OnLoactionSheetSelection(action.Title)));
			}
		}

		void OnLoactionSheetSelection(string title)
		{
			var loc = NSObject.FromObject(title);
			if (!txtLocation.Tags.Contains(loc))
				txtLocation.Tags.Add(loc);
			txtLocation.ReloadTagSubviews();
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
					DateFormat = "MMMM dd, yyyy"
				};

				var date = NSObject.FromObject(modalPicker.DatePicker.Date);
				if (!txtOrderDate.Tags.Contains(date))
					txtOrderDate.AddTag(dateFormatter.ToString(modalPicker.DatePicker.Date));
				txtOrderDate.ReloadTagSubviews();
				//= dateFormatter.ToString(modalPicker.DatePicker.Date);
			};
		}
	}
}
