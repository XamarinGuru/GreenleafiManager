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

namespace GreenleafiManager
{
	public partial class MasterLocationViewController : UIViewController
	{
		public static LoadingOverlay loadingOverlay;

		SfDataGrid dataGrid;
		UISearchBar filterText = new UISearchBar();
		public Location SelectedItem { get; set; }
		public Location SwipedItem { get; set; }

		private UIView _slideView;
		private UIButton _button;

		public MasterLocationViewController(IntPtr handle) : base(handle)
		{
			var bounds = UIScreen.MainScreen.Bounds;

			loadingOverlay = new LoadingOverlay(bounds);

		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			_slideView = new UIView(new CoreGraphics.CGRect(0, 0, 100, 100));
			_button = UIButton.FromType(UIButtonType.InfoDark);
			_button.Frame = new CoreGraphics.CGRect(0, 0, 100, 100);
			_button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_button.AddTarget(this, new Selector("ShowDetails"), UIControlEvent.TouchUpInside);
			_slideView.Add(_button);

			await CreateGrid();
		}

		public async override void ViewWillAppear(Boolean animated)
		{
			base.ViewWillAppear(true);

			await UpdateGridWithLocalData();
		}

		private void ShowLoading()
		{
			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
			View.Add(loadingOverlay);
		}

		private void HideLoading()
		{
			loadingOverlay.Hide();
		}

		private async Task UpdateGridWithLocalData()
		{
			if (dataGrid != null)
			{
				ShowLoading();

				dataGrid.IsBusy = true;
				await AzureService.LocationService.UpdateLocationsFromLocalDB();
				dataGrid.ItemsSource = AzureService.LocationService.Locations;
				dataGrid.SortColumnDescriptions.Clear();
				dataGrid.SortColumnDescriptions.Add(new SortColumnDescription()
				{
					ColumnName = "Name",
					SortDirection = Syncfusion.Data.ListSortDirection.Ascending
				});
				this.dataGrid.IsBusy = false;
				if (SelectedItem != null)
				{
					if (AzureService.LocationService.Locations.Any(x => x.Id == SelectedItem.Id))
					{
						dataGrid.SelectedItem = AzureService.LocationService.Locations.Where(x => x.Id == SelectedItem.Id).First();
						dataGrid.ScrollToRowIndex(dataGrid.SelectedIndex);
						dataGrid.SelectedItem = null;
					}
					else
						dataGrid.SelectedItem = null;
				}
				HideLoading();
			}
		}

		private void CreateAddNewButton()
		{
			var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;
		}

		private async Task CreateGrid()
		{
			ShowLoading();

			CreateAddNewButton();

			//await _LocationService.InitializeStoreAsync();
			await AzureService.LocationService.UpdateLocationsFromAzure();

			dataGrid = new SfDataGrid();
			dataGrid.ItemsSource = AzureService.LocationService.Locations;
			dataGrid.AllowSorting = true;
			dataGrid.SelectionMode = SelectionMode.Single;
			dataGrid.SelectionChanged += DataGrid_SelectionChanged;

			SetupGrid();

			dataGrid.AllowPullToRefresh = true;
			dataGrid.PullToRefreshCommand = (ICommand)(new RelayCommand(param => ExecutePullToRefresh()));

			filterText.Frame = new CGRect(0, 60, View.Frame.Width, 55);
			filterText.TextChanged += OnFilterTextChanged;
			filterText.Placeholder = "Search Locations";
			int hex = 0xCECED2;

			filterText.BackgroundColor = UIColor.Clear.FromHex(hex);
			filterText.TintColor = UIColor.Clear.FromHex(hex);
			filterText.BarTintColor = UIColor.Clear.FromHex(hex);
			filterText.Layer.BorderWidth = 1;
			filterText.Layer.BorderColor = Extensions.CGColorFromHex(hex);
			AzureService.LocationService.FilterTextChanged = OnFilterChanged;

			View.AddSubview(filterText);

			View.InsertSubviewBelow(dataGrid, filterText);

			await UpdateGridWithLocalData();
			HideLoading();
		}

		private void SetupGrid()
		{
			dataGrid.HeaderRowHeight = 45;
			dataGrid.RowHeight = 45;

			dataGrid.Frame = new CGRect(0, 115, View.Frame.Width, View.Frame.Height - 115);
			dataGrid.AutoGenerateColumns = false;
			dataGrid.ColumnSizer = ColumnSizer.Star;

			GridTextColumn nameColumn = new GridTextColumn();
			nameColumn.MappingName = "Name";
			nameColumn.HeaderText = "Name";

			GridTextColumn cityColumn = new GridTextColumn();
			cityColumn.MappingName = "City";
			cityColumn.HeaderText = "City";

			GridTextColumn phoneColumn = new GridTextColumn();
			phoneColumn.MappingName = "Phone";
			phoneColumn.HeaderText = "Phone";

			dataGrid.Columns.Add(nameColumn);
			dataGrid.Columns.Add(cityColumn);
			dataGrid.Columns.Add(phoneColumn);

			UIButton leftSwipeViewText = new UIButton();
			leftSwipeViewText.SetTitle("Edit", UIControlState.Normal);
			leftSwipeViewText.SetTitleColor(UIColor.White, UIControlState.Normal);
			leftSwipeViewText.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			leftSwipeViewText.BackgroundColor = UIColor.FromRGB(0, 158, 218);

			CustomSwipeView leftSwipeView = new CustomSwipeView();
			leftSwipeView.AddSubview(leftSwipeViewText);

			UIButton rightSwipeViewText = new UIButton();
			rightSwipeViewText.SetTitle("Delete", UIControlState.Normal);
			rightSwipeViewText.SetTitleColor(UIColor.White, UIControlState.Normal);
			rightSwipeViewText.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			rightSwipeViewText.BackgroundColor = UIColor.FromRGB(220, 89, 95);
			rightSwipeViewText.AddGestureRecognizer(new UITapGestureRecognizer(DeleteTapped));

			CustomSwipeView rightSwipeView = new CustomSwipeView();
			rightSwipeView.AddSubview(rightSwipeViewText);

			this.dataGrid.AllowSwiping = true;
			this.dataGrid.LeftSwipeView = leftSwipeView;
			this.dataGrid.RightSwipeView = rightSwipeView;
			this.dataGrid.SwipeEnded += SfGrid_SwipeEnded;
			this.dataGrid.SwipeStarted += SfGrid_SwipeStarted;

		}

		void DataGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
		{
			var obj = dataGrid.SelectedItem;
			SelectedItem = (Location)obj;

			LocationDetailViewController locationDetailViewController = this.Storyboard.InstantiateViewController("LocationDetailViewController") as LocationDetailViewController;
			locationDetailViewController.Location = SelectedItem;
			locationDetailViewController.MasterViewController = this;
			locationDetailViewController.EditOrCreate = "updated";

			this.NavigationController.PushViewController(locationDetailViewController, true);
		}

		public class CustomSwipeView : UIView
		{
			public CustomSwipeView()
			{
			}
			public override void LayoutSubviews()
			{
				var start = 0;
				var childWidth = this.Frame.Width;
				foreach (var v in this.Subviews)
				{
					v.Frame = new CGRect(start, 0, childWidth + start, this.Frame.Height);
					start = start + (int)this.Frame.Width;
				}
			}
		}
		void DeleteTapped(UITapGestureRecognizer gesture)
		{
			if (gesture.State == UIGestureRecognizerState.Ended)
			{
				var alert = UIAlertController.Create("Delete Location", String.Format("Delete {0}?", SwipedItem.Name), UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
				alert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Default, (UIAlertAction obj) => DeleteSwipedItem()));
				PresentViewController(alert, animated: true, completionHandler: null);
			}
		}
		private void SfGrid_SwipeEnded(object sender, SwipeEndedEventArgs e)
		{
			SwipedItem = (Location)e.RowData;
		}
		private void SfGrid_SwipeStarted(object sender, SwipeStartedEventArgs e)
		{
			SwipedItem = (Location)e.RowData;
		}
		private async void DeleteSwipedItem()
		{
			if (AzureService.InvoiceService.Invoices.Where(x => x.LocationId == SwipedItem.Id).Any())
			{
				var alert2 = UIAlertController.Create("Cannot Delete", String.Format("{0} is associated with existing invoices and cannot be deleted.", SwipedItem.Name), UIAlertControllerStyle.Alert);
				alert2.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert2, animated: true, completionHandler: null);

				return;
			}
			if (AzureService.InventoryService.Items.Where(x => x.LocationId == SwipedItem.Id).Any())
			{
				var alert2 = UIAlertController.Create("Cannot Delete", String.Format("{0} is associated with existing items and cannot be deleted.", SwipedItem.Name), UIAlertControllerStyle.Alert);
				alert2.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert2, animated: true, completionHandler: null);

				return;
			}
			//await AzureService.LocationService.Delete(SwipedItem);
			await AzureService.LocationService.Delete(SwipedItem);
			var alert = UIAlertController.Create("Deleted", String.Format("Deleted {0}", SwipedItem.Name), UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

			PresentViewController(alert, animated: true, completionHandler: null);
			await UpdateGridWithLocalData();
		}


		void OnFilterTextChanged(object sender, UISearchBarTextChangedEventArgs e)
		{

			AzureService.LocationService.FilterText = e.SearchText;

		}

		private void OnFilterChanged()
		{
			if (dataGrid.View != null)
			{
				this.dataGrid.View.Filter = AzureService.LocationService.FilerRecords;
				this.dataGrid.View.RefreshFilter();
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
			await AzureService.LocationService.UpdateLocationsFromAzure();
			dataGrid.ItemsSource = AzureService.LocationService.Locations;
			dataGrid.SortColumnDescriptions.Clear();
			dataGrid.SortColumnDescriptions.Add(new SortColumnDescription()
			{
				ColumnName = "Name",
				SortDirection = Syncfusion.Data.ListSortDirection.Ascending
			});
			this.dataGrid.IsBusy = false;
		}
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showInventoryDetail")
			{
				var controller = (LocationDetailViewController)((UINavigationController)segue.DestinationViewController).TopViewController;
				controller.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
				controller.NavigationItem.LeftItemsSupplementBackButton = true;
			}
		}
		void AddNewItem(object sender, EventArgs args)
		{
			LocationDetailViewController locationDetailViewController = this.Storyboard.InstantiateViewController("LocationDetailViewController") as LocationDetailViewController;
			locationDetailViewController.Location = new Location();
			locationDetailViewController.MasterViewController = this;
			locationDetailViewController.EditOrCreate = "added";
			this.NavigationController.PushViewController(locationDetailViewController, true);
		}

	}

	//partial class MasterLocationViewController : UIViewController
	//{
	//	public static LoadingOverlay loadingOverlay;
	//	public IGGridViewDataSourceHelper _dsh;
	//	public IGGridView _grid;
	//	public LocationService _locationService {
	//		get;
	//		set;
	//	}

	//	private UIView _slideView;
	//	private UIButton _button;

	//	public MasterLocationViewController (IntPtr handle) : base (handle)
	//	{
	//		var bounds = UIScreen.MainScreen.Bounds;

	//		loadingOverlay = new LoadingOverlay (bounds);
	//		_locationService = AzureService.LocationService;
	//	}

	//	public override async void ViewDidLoad ()
	//	{
	//		base.ViewDidLoad ();

	//		_slideView = new UIView (new CoreGraphics.CGRect (0, 0, 100, 100));
	//		_button = UIButton.FromType (UIButtonType.InfoDark);
	//		_button.Frame = new CoreGraphics.CGRect (0, 0, 100, 100);
	//		_button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
	//		_button.AddTarget (this, new Selector ("ShowDetails"), UIControlEvent.TouchUpInside);
	//		_slideView.Add (_button);


	//		await CreateGrid ();
	//	}

	//	private async Task CreateGrid()
	//	{
	//		ShowLoading ();

	//		//await _locationService.AddLocationToLocalDB (new Locations (){ Name = "Los Angeles", Province = "LA", Phone = "000-2222"});
	//		//await _locationService.AddLocationToLocalDB (new Locations (){ Name = "Alaska", Province = "AK", Phone = "000-2222"});
	//		//await _locationService.AddLocationToLocalDB (new Locations (){ Name = "Maui", Province = "HI", Phone = "000-2222"});
	//		//await _locationService.AddLocationToLocalDB (new Locations (){ Name = "Online", Province = "ON", Phone = "000-2222"});

	//		_dsh = new IGGridViewDataSourceHelper ();
	//		_dsh.AutoGenerateColumns = false;

	//		await _locationService.InitializeStoreAsync ();
	//		var data = _locationService.GetLocalNSItems ();

	//		IGGridViewColumnDefinition valCol = new IGGridViewColumnDefinition("Name");
	//		_dsh.ColumnDefinitions.Add (valCol);

	//		IGGridViewColumnDefinition valCol1 = new IGGridViewColumnDefinition("City");
	//		_dsh.ColumnDefinitions.Add (valCol1);

	//		IGGridViewColumnDefinition valCol2 = new IGGridViewColumnDefinition("Province");
	//		_dsh.ColumnDefinitions.Add (valCol2);

	//		IGGridViewColumnDefinition valCol3 = new IGGridViewColumnDefinition("Phone");
	//		_dsh.ColumnDefinitions.Add (valCol3);

	//		_dsh.Data = data.ToArray();

	//		_grid = new IGGridView (new CoreGraphics.CGRect (0, 100, this.View.Frame.Size.Width, this.View.Frame.Size.Height), IGGridViewStyle.IGGridViewStyleDefault);
	//		_grid.WeakDelegate = new LocationDelegate (this);

	//		_grid.FilterAction = IGGridViewFilterAction.IGGridViewFilterActionImmediate;
	//		_grid.DataSource = _dsh;
	//		_grid.UpdateData ();

	//		HideLoading ();
	//		this.View.AddSubview (_grid);
	//	}

	//	public override void ViewWillAppear (Boolean animated)
	//	{
	//		base.ViewWillAppear (true);
	//		UpdateGrid ();
	//	}

	//	private void UpdateGrid()
	//	{
	//		if (_grid != null) {
	//			_locationService = AzureService.LocationService;
	//			var data = _locationService.GetLocalNSItems ();
	//			_dsh.Data = data.ToArray();
	//			_grid.DataSource = _dsh;
	//			_grid.UpdateData ();
	//		}
	//	}



	//	private void ShowLoading()
	//	{
	//		View.Add (loadingOverlay);
	//	}

	//	private void HideLoading()
	//	{
	//		loadingOverlay.Hide ();
	//	}

	//	public class LocationDelegate : IGGridViewDelegate
	//	{
	//		MasterLocationViewController parentController;
	//		IGRowPath _row;
	//		public LocationDelegate (MasterLocationViewController parent)
	//		{
	//			parentController = parent;
	//		}

	//		public override async void UpdateRequested (IGGridView gridView)
	//		{
	//			var service =  AzureService.LocationService;
	//			await service.SyncAsync ();
	//			gridView.FinishedUpdating(); 
	//		}

	//		[Export ("gridView:WillSelectRow:")]
	//		public override IGRowPath WillSelectRow (Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
	//		{
	//			_row = path;
	//			ShowDetails ();
	//			return null;
	//		}

	//		[Export ("ShowDetails")]
	//		public void ShowDetails ()
	//		{
	//			IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)parentController._grid.WeakDataSource;
	//			NSObject obj = dsh.ResolveDataObjectForRow (new Infragistics.IGRowPath (_row.RowIndex, (System.nint)_row.SectionIndex));

	//			var foundItem = AzureService.LocationService.Items.Where (x => x.Id == ((LocationNs)obj).Id).FirstOrDefault ();
	//			LocationDetailViewController locationDetailViewController = parentController.Storyboard.InstantiateViewController ("LocationDetailViewController") as LocationDetailViewController; 
	//			locationDetailViewController.Location = new LocationNs (foundItem);

	//			parentController.NavigationController.PushViewController (locationDetailViewController, true);

	//		}
	//	}
	//}
}
