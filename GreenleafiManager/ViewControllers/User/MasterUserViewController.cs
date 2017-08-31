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
	public partial class MasterUserViewController : UIViewController
	{
		public static LoadingOverlay loadingOverlay;

		SfDataGrid dataGrid;
		UISearchBar filterText = new UISearchBar();
		public User SelectedItem { get; set; }
		public User SwipedUser { get; set; }

		private UIView _slideView;
		private UIButton _button;

		public MasterUserViewController(IntPtr handle) : base(handle)
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
				await AzureService.UserService.UpdateUsersFromLocalDB();
				dataGrid.ItemsSource = AzureService.UserService.Users;
				dataGrid.SortColumnDescriptions.Clear();
				dataGrid.SortColumnDescriptions.Add(new SortColumnDescription()
				{
					ColumnName = "UserName",
					SortDirection = Syncfusion.Data.ListSortDirection.Ascending
				});
				this.dataGrid.IsBusy = false;
				if (SelectedItem != null)
				{
					if (AzureService.UserService.Users.Any(x => x.Id == SelectedItem.Id))
					{
						dataGrid.SelectedItem = AzureService.UserService.Users.Where(x => x.Id == SelectedItem.Id).First();
						dataGrid.ScrollToRowIndex(dataGrid.SelectedIndex);
						dataGrid.SelectedItem = null;
					}
					else
						dataGrid.SelectedItem = null;
				}
				HideLoading();
			}
		}

		private void SelectItem(User item)
		{
			if (SelectedItem != null)
			{
				dataGrid.SelectedItem = AzureService.UserService.Users.Where(x => x.Id == SelectedItem.Id).First();
				dataGrid.ScrollToRowIndex(dataGrid.SelectedIndex);
				//dataGrid.SelectedItem = null;
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

			//await _UserService.InitializeStoreAsync();
			await AzureService.UserService.UpdateUsersFromAzure();

			dataGrid = new SfDataGrid();
			dataGrid.ItemsSource = AzureService.UserService.Users;
			dataGrid.AllowSorting = true;
			dataGrid.SelectionMode = SelectionMode.Single;
			dataGrid.SelectionChanged += DataGrid_SelectionChanged;
			dataGrid.AllowSwiping = true;

			SetupGrid();

			dataGrid.AllowPullToRefresh = true;
			dataGrid.PullToRefreshCommand = (ICommand)(new RelayCommand(param => ExecutePullToRefresh()));

			filterText.Frame = new CGRect(0, 60, View.Frame.Width, 55);
			filterText.TextChanged += OnFilterTextChanged;
			filterText.Placeholder = "Search Users";
			int hex = 0xCECED2;
			filterText.BackgroundColor = UIColor.Clear.FromHex(hex);
			filterText.TintColor = UIColor.Clear.FromHex(hex);
			filterText.BarTintColor = UIColor.Clear.FromHex(hex);
			filterText.Layer.BorderWidth = 1;
			filterText.Layer.BorderColor = Extensions.CGColorFromHex(hex);
			AzureService.UserService.FilterTextChanged = OnFilterChanged;

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

			GridTextColumn displayNameColumn = new GridTextColumn();
			displayNameColumn.MappingName = "FirstName";
			displayNameColumn.HeaderText = "Name";

			GridTextColumn userNameColumn = new GridTextColumn();
			userNameColumn.MappingName = "UserName";
			userNameColumn.HeaderText = "Username";

			GridTextColumn emailColumn = new GridTextColumn();
			emailColumn.MappingName = "Email";
			emailColumn.HeaderText = "Email";

			GridTextColumn phoneColumn = new GridTextColumn();
			phoneColumn.MappingName = "Phone";
			phoneColumn.HeaderText = "Phone";

			dataGrid.Columns.Add(displayNameColumn);
			dataGrid.Columns.Add(userNameColumn);
			dataGrid.Columns.Add(emailColumn);
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
				var alert = UIAlertController.Create("Delete User", String.Format("Delete {0}?", SwipedUser.UserName), UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel,null));
				alert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Default,(UIAlertAction obj) => DeleteSwipedUser()));
				PresentViewController(alert, animated: true, completionHandler: null);
			}
		}
		private void SfGrid_SwipeEnded(object sender, SwipeEndedEventArgs e)
		{
			SwipedUser = (User)e.RowData;
			//initializeTextFields();
		}
		private void SfGrid_SwipeStarted(object sender, SwipeStartedEventArgs e)
		{
			SwipedUser = (User)e.RowData;
			//initializeTextFields();
		}
		private async void DeleteSwipedUser()
		{
			if (AzureService.InvoiceService.Invoices.Where(x => x.UserId == SwipedUser.Id).Any())
			{
				var alert2 = UIAlertController.Create("Cannot Delete", String.Format("{0} is associated with existing invoices and cannot be deleted.", SwipedUser.UserName), UIAlertControllerStyle.Alert);
				alert2.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
				PresentViewController(alert2, animated: true, completionHandler: null);

				return;
			}
			await AzureService.UserService.DeleteUser(SwipedUser);
			var alert = UIAlertController.Create("Deleted", String.Format("Deleted {0}", SwipedUser.UserName), UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

			PresentViewController(alert, animated: true, completionHandler: null);
			await UpdateGridWithLocalData();
		}


		void DataGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
		{
			var obj = dataGrid.SelectedItem;
			SelectedItem = (User)obj;

			UserDetailViewController UserDetailViewController = this.Storyboard.InstantiateViewController("UserDetailViewController") as UserDetailViewController;
			UserDetailViewController.User = SelectedItem;
			UserDetailViewController.MasterViewController = this;
			UserDetailViewController.EditOrCreate = "updated";

			this.NavigationController.PushViewController(UserDetailViewController, true);
		}

		void OnFilterTextChanged(object sender, UISearchBarTextChangedEventArgs e)
		{
			AzureService.UserService.FilterText = e.SearchText;
		}

		private void OnFilterChanged()
		{
			if (dataGrid.View != null)
			{
				this.dataGrid.View.Filter = AzureService.UserService.FilerRecords;
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
			await AzureService.UserService.UpdateUsersFromAzure();
			dataGrid.ItemsSource = AzureService.UserService.Users;
			dataGrid.SortColumnDescriptions.Clear();
			dataGrid.SortColumnDescriptions.Add(new SortColumnDescription()
			{
				ColumnName = "UserName",
				SortDirection = Syncfusion.Data.ListSortDirection.Ascending
			});
			this.dataGrid.IsBusy = false;
		}
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showUserDetailSeque")
			{
				var controller = (UserDetailViewController)((UINavigationController)segue.DestinationViewController).TopViewController;
				controller.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
				controller.NavigationItem.LeftItemsSupplementBackButton = true;
			}
		}

		void AddNewItem(object sender, EventArgs args)
		{
			UserDetailViewController UserDetailViewController = this.Storyboard.InstantiateViewController("UserDetailViewController") as UserDetailViewController;
			UserDetailViewController.User = new User();
			UserDetailViewController.MasterViewController = this;
			UserDetailViewController.EditOrCreate = "added";
			this.NavigationController.PushViewController(UserDetailViewController, true);
		}
	}
}
