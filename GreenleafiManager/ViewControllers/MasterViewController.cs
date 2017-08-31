using System;
using System.Collections.Generic;

using UIKit;
using Foundation;
using CoreGraphics;
using Infragistics;
using System.Threading.Tasks;
using ObjCRuntime;


namespace GreenleafiManager
{
	public partial class MasterViewController : UITableViewController
	{
		public DetailViewController DetailViewController { get; set; }

		DataSource dataSource;
		IGGridViewDataSourceHelper _dsh;
		IGGridView _grid;
		IGRowPath _row;
		UIView _slideView;
		UIButton _button;


		public MasterViewController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Master", "Master");
			
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				PreferredContentSize = new CGSize (320f, 600f);
				ClearsSelectionOnViewWillAppear = false;
			}
		}

		public QSInventoryService InventoryService { get; set; }

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			_slideView = new UIView (new CoreGraphics.CGRect (0, 0, 100, 100));
			_button = UIButton.FromType (UIButtonType.InfoDark);
			_button.Frame = new CoreGraphics.CGRect (0, 0, 100, 100);
			_button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_button.AddTarget (this, new Selector ("ShowDetails"), UIControlEvent.TouchUpInside);
			_slideView.Add (_button);

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.LeftBarButtonItem = EditButtonItem;

			var addButton = new UIBarButtonItem (UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;

			DetailViewController = (DetailViewController)((UINavigationController)SplitViewController.ViewControllers [1]).TopViewController;

			//TableView.Source = dataSource = new DataSource (this);

			InventoryService = QSInventoryService.DefaultService;
			await InventoryService.InitializeStoreAsync ();

			_grid = new IGGridView(new CoreGraphics.CGRect(0,100, this.View.Frame.Size.Width, this.View.Frame.Size.Height), IGGridViewStyle.IGGridViewStyleDefault);
			_grid.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_grid.Theme = new IGGridViewLightTheme ();
			_grid.SwipeRowAction = IGGridViewSwipeRowAction.IGGridViewSwipeRowActionManualSlideLeft;
			_grid.WeakDelegate = this;
			//this.View.AddSubview (_gridView);

			_dsh = new IGGridViewDataSourceHelper ();
			_dsh.AutoGenerateColumns = false;


			IGGridViewImageColumnDefinition imgcol = new IGGridViewImageColumnDefinition ("Image", IGGridViewImageColumnDefinitionPropertyType.IGGridViewImageColumnDefinitionPropertyTypeImage);
			imgcol.HeaderText = "Item";
			imgcol.HeaderTextAlignment = UITextAlignment.Center;
			imgcol.CacheImages = true;
			imgcol.ContentMode = UIViewContentMode.ScaleAspectFit;
			imgcol.Width = IGColumnWidth.CreateNumericColumnWidth (75f);

			_dsh.ColumnDefinitions.Add (imgcol);

			IGGridViewColumnDefinition col = new IGGridViewColumnDefinition ("DisplayDescription");
			col.HeaderText = "Description";
			col.HeaderTextAlignment = UITextAlignment.Center;

			_dsh.ColumnDefinitions.Add (col);
			var items = await InventoryService.GetNSItems ();

			_dsh.Data = items.ToArray();


			//gridView.WeakDataSource = new DataProvider();
			_grid.DataSource = _dsh;
			_grid.UpdateData();
			this.View.AddSubview(_grid);
			//_gridView.WeakDelegate = new MyDelegate (_dsh);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		void AddNewItem (object sender, EventArgs args)
		{
			dataSource.Objects.Insert (0, DateTime.Now);

			using (var indexPath = NSIndexPath.FromRowSection (0, 0))
				TableView.InsertRows (new [] { indexPath }, UITableViewRowAnimation.Automatic);
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showDetail") {
				var indexPath = TableView.IndexPathForSelectedRow;
				var item = dataSource.Objects [indexPath.Row];
				var controller = (DetailViewController)((UINavigationController)segue.DestinationViewController).TopViewController;
				controller.SetDetailItem (item);
				controller.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
				controller.NavigationItem.LeftItemsSupplementBackButton = true;
			}
		}
		[Export ("gridView:canSlideRowLeft:")]
		public bool CanSlideRowLeft (Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
		{
			return true;
		}

		[Export ("gridView:viewForSlideRowLeft:")]
		public UIKit.UIView ResolveSlideRowLeftView (Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
		{
			_row = path;
			return _slideView;
		}

		[Export("ShowDetails")]
		public void ShowDetails()
		{
			//NSObject obj = _dsh.ResolveDataValueForCell (new Infragistics.IGCellPath(_path.RowIndex, (System.nint)_path.SectionIndex, (System.nint)1));
			//string desc = obj.ToString();
			NSObject obj = _dsh.ResolveDataObjectForRow (new Infragistics.IGRowPath (_row.RowIndex, (System.nint)_row.SectionIndex));//, (System.nint)1));
			//new UIAlertView("Touch3", String.Format("Row {0} touched",desc), null, "OK", null).Show();
			//InventoryDetailViewController inventoryDetailViewController = new InventoryDetailViewController((InventoryNS)obj);
			//inventoryDetailViewController.InventoryService = InventoryService;

			//this.NavigationController.PushViewController (inventoryDetailViewController, true);

		}
		class DataSource : UITableViewSource
		{
			static readonly NSString CellIdentifier = new NSString ("Cell");
			readonly List<object> objects = new List<object> ();
			readonly MasterViewController controller;

			public DataSource (MasterViewController controller)
			{
				this.controller = controller;
			}

			public IList<object> Objects {
				get { return objects; }
			}

			// Customize the number of sections in the table view.
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return objects.Count;
			}

			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (CellIdentifier, indexPath);

				cell.TextLabel.Text = objects [indexPath.Row].ToString ();

				return cell;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the specified item to be editable.
				return true;
			}

			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					// Delete the row from the data source.
					objects.RemoveAt (indexPath.Row);
					controller.TableView.DeleteRows (new [] { indexPath }, UITableViewRowAnimation.Fade);
				} else if (editingStyle == UITableViewCellEditingStyle.Insert) {
					// Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
				}
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
					controller.DetailViewController.SetDetailItem (objects [indexPath.Row]);
			}
		}
	}
}


