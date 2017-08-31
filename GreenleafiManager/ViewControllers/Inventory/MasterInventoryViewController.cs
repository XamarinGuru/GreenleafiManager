using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using GreenleafiManager.ViewControllers;
using Infragistics;
using System.Threading.Tasks;
using ObjCRuntime;
using UIKit;
using ZXing.Mobile;

namespace GreenleafiManager
{
	public partial class MasterInventoryViewController : UIViewController
	{
		public static LoadingOverlay loadingOverlay;
		public static UpdatingOverlay updatingOverlay;
		private UIButton _button;
		private CustomAlert Alert;
		private CustomFilterAlert filterAlert;
		public IGGridViewDataSourceHelper _dsh;
		public IGGridView _grid;
		private IGRowPath _row;
		public static IGCellPath ScrollToCell;
		private UIView _slideView;
		public bool isInvoiceSelected;
		private NewCodeAlert newCodeAlert;

		public MasterInventoryViewController()
		{
			ShowOnlySoldItems = false;
		}
		public override void ViewDidLayoutSubviews()
		{
			Alert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
		}

		public void ShowLoading()
		{
			View.Add(loadingOverlay);
		}

		public void HideLoading()
		{
			loadingOverlay.Hide();
		}

		public MasterInventoryViewController(IntPtr handle) : base(handle)
		{
			Title = NSBundle.MainBundle.LocalizedString("Inventory", "Inventory");
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				PreferredContentSize = new CGSize(320f, 600f);
			}
		}

		public DetailViewController DetailViewController { get; set; }

		public delegate void Del(string message);

		public void AddAlertMethod(object o, EventArgs e)
		{
			View.AddSubview(filterAlert);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			AzureService.InvoiceService.IsInvoiceSelected = isInvoiceSelected;

			ScrollToCell = null;
			var categories = AzureService.InventoryService.Categories;
			Alert = new CustomAlert(View.Bounds.Size.Width, View.Bounds.Height, categories, "Choose category", this);

			newCodeAlert = new NewCodeAlert(View.Bounds.Size.Width, View.Bounds.Height);

			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
			updatingOverlay = new UpdatingOverlay(bounds);



			_slideView = new UIView(new CoreGraphics.CGRect(0, 0, 100, 100));
			_button = UIButton.FromType(UIButtonType.InfoDark);
			_button.Frame = new CoreGraphics.CGRect(0, 0, 100, 100);
			_button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_button.AddTarget(this, new Selector("ShowDetails"), UIControlEvent.TouchUpInside);
			_slideView.Add(_button);


			var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewItem);
			addButton.AccessibilityLabel = "addButton";
			NavigationItem.RightBarButtonItem = addButton;


			IGGridViewImageColumnDefinition imgcol = new IGGridViewImageColumnDefinition("ThumbnailImageUrl", IGGridViewImageColumnDefinitionPropertyType.IGGridViewImageColumnDefinitionPropertyTypeImage);
			//ImageColumnDefinition imgcol = new ImageColumnDefinition("ThumbnailImage", IGGridViewImageColumnDefinitionPropertyType.IGGridViewImageColumnDefinitionPropertyTypeImage, this);
			imgcol.LoadAsync = true;
			imgcol.CacheImages = true;
			imgcol.HeaderText = "Item";
			imgcol.HeaderTextAlignment = UITextAlignment.Center;
			imgcol.ContentMode = UIViewContentMode.ScaleAspectFit;
			imgcol.Width = IGColumnWidth.CreatePercentColumnWidth(34f);

			_dsh = new IGGridViewDataSourceHelper();

			_dsh.AutoGenerateColumns = false;
			_dsh.FilteringKey = "SearchString";
			_dsh.FilterType = IGGridViewFilterConditionType.IGGridViewFilterConditionTypeStringContains;
			_dsh.ColumnDefinitions.Add(imgcol);

			FrontTagColumnDefinition frontTagColDef = new FrontTagColumnDefinition("frontTag");
			frontTagColDef.HeaderText = "FrontTagColDef";
			frontTagColDef.Width = IGColumnWidth.CreatePercentColumnWidth(33f);
			_dsh.ColumnDefinitions.Add(frontTagColDef);

			BackTagColumnDefinition backTagColDef = new BackTagColumnDefinition("backTag");
			backTagColDef.HeaderText = "BackTagColDef";
			backTagColDef.Width = IGColumnWidth.CreatePercentColumnWidth(33f);
			_dsh.ColumnDefinitions.Add(backTagColDef);

			_grid = new IGGridView(new CoreGraphics.CGRect(0, 100, this.View.Frame.Size.Width, this.View.Frame.Size.Height), IGGridViewStyle.IGGridViewStyleDefault);

			_grid.EmptyRows = false;

			_grid.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			_grid.SelectionType = IGGridViewSelectionType.IGGridViewSelectionTypeRow;
			_grid.SwipeRowAction = IGGridViewSwipeRowAction.IGGridViewSwipeRowActionManualSlideBoth;

			_grid.WeakDelegate = new MyDelegate(this);
			_grid.FilterAction = IGGridViewFilterAction.IGGridViewFilterActionImmediate;
			_grid.HeaderHeight = 0;

			_grid.DataSource = _dsh;
			_grid.RowHeight = 300;
			this.View.AddSubview(_grid);
			this.Title = "Inventory";
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public void ShowUpdatingOverlay()
		{
			var bounds = UIScreen.MainScreen.Bounds;
			updatingOverlay = new UpdatingOverlay(bounds);
			View.Add(updatingOverlay);
		}

		public void HideUpdatingOverlay()
		{
			updatingOverlay.Hide();
		}


		public class MyDelegate : IGGridViewDelegate
		{
			MasterInventoryViewController parentController;
			IGRowPath _row;

			public MyDelegate(MasterInventoryViewController parent)
			{
				parentController = parent;
			}

			[Export("gridView:WillSelectRow:")]
			public override IGRowPath WillSelectRow(Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
			{
				_row = path;
				//gridView.scro
				MasterInventoryViewController.ScrollToCell = path.ConvertToCellPath();
				ShowDetails();
				return null;
			}

			[Export("gridView:viewForSlideRowLeft:")]
			public override UIKit.UIView ResolveSlideRowLeftView(Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
			{
				_row = path;

				UILabel label = new UILabel();
				label.BackgroundColor = (new UIColor((float)(255.0 / 255.0), (float)(59.0 / 255.0), (float)(48.0 / 255.0), (float)(1.0)));// UIColor.Red;
				label.TextColor = UIColor.White;
				label.TextAlignment = UITextAlignment.Center;
				label.Text = "Delete";
				label.UserInteractionEnabled = true;
				UITapGestureRecognizer tapGestureRecognizer = new UITapGestureRecognizer(DeleteRow);
				label.AddGestureRecognizer(tapGestureRecognizer);
				return label;
			}

			[Export("gridView:viewForSlideRowRight:")]
			public override UIKit.UIView ResolveSlideRowRightView(Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
			{
				_row = path;

				IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)parentController._grid.WeakDataSource;
				InventoryNS item = (InventoryNS)dsh.ResolveDataObjectForRow(new Infragistics.IGRowPath(_row.RowIndex, (System.nint)_row.SectionIndex));


				UILabel label = new UILabel();
				label.BackgroundColor = item.Sold ? (new UIColor((float)(255.0 / 255.0), (float)(59.0 / 255.0), (float)(48.0 / 255.0), (float)(1.0))) : (new UIColor((float)(76.0 / 255.0), (float)(217.0 / 255.0), (float)(100.0 / 255.0), (float)(1.0)));// UIColor.Green : UIColor.LightGray;
				label.TextColor = UIColor.White;
				label.TextAlignment = UITextAlignment.Center;
				label.Text = item.Sold ? "Unmark Sold" : "Mark Sold";
				label.UserInteractionEnabled = true;
				UITapGestureRecognizer tapGestureRecognizer = item.Sold ? new UITapGestureRecognizer(UnMarkRowSold) : new UITapGestureRecognizer(MarkRowSold);
				label.AddGestureRecognizer(tapGestureRecognizer);
				return label;
			}

			[Export("MarkRowSold")]
			public async void MarkRowSold()
			{
				IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)parentController._grid.WeakDataSource;
				InventoryNS item = (InventoryNS)dsh.ResolveDataObjectForRow(new Infragistics.IGRowPath(_row.RowIndex, (System.nint)_row.SectionIndex));

				parentController.ShowUpdatingOverlay();

				await AzureService.InventoryService.MarkItemSoldAsync(item.ConvertToInventory());

				parentController.HideUpdatingOverlay();

				AzureService.InventoryService.NeedLocalRefresh = true;
				parentController.ViewDidAppear(true);
			}

			[Export("UnMarkRowSold")]
			public async void UnMarkRowSold()
			{
				IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)parentController._grid.WeakDataSource;
				InventoryNS item = (InventoryNS)dsh.ResolveDataObjectForRow(new Infragistics.IGRowPath(_row.RowIndex, (System.nint)_row.SectionIndex));

				parentController.ShowUpdatingOverlay();

				await AzureService.InventoryService.MarkItemSoldAsync(item.ConvertToInventory(), false);

				parentController.HideUpdatingOverlay();

				AzureService.InventoryService.NeedLocalRefresh = true;
				parentController.ViewDidAppear(true);
			}

			[Export("DeleteRow")]
			public async void DeleteRow()
			{
				IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)parentController._grid.WeakDataSource;
				InventoryNS item = (InventoryNS)dsh.ResolveDataObjectForRow(new Infragistics.IGRowPath(_row.RowIndex, (System.nint)_row.SectionIndex));

				parentController.ShowUpdatingOverlay();

				await AzureService.InventoryService.DeleteItem(item.ConvertToInventory());

				parentController.HideUpdatingOverlay();

				AzureService.InventoryService.NeedLocalRefresh = true;
				parentController.ViewDidAppear(true);
			}

			[Export("ShowDetails")]
			public void ShowDetails()
			{
               	IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)parentController._grid.WeakDataSource;
				NSObject obj = dsh.ResolveDataObjectForRow(new Infragistics.IGRowPath(_row.RowIndex, (System.nint)_row.SectionIndex));
				
				var foundItem = AzureService.InventoryService.Items.Where(x => x.Id == ((InventoryNS)obj).Id).FirstOrDefault();

                Storyboards.LoadingItemDetailsViewController loadingViewController = parentController.Storyboard.InstantiateViewController("LoadingItemDetailsViewController") as Storyboards.LoadingItemDetailsViewController;
				loadingViewController.ParentMasterInventoryViewController = parentController;
                loadingViewController.InventoryItem = new InventoryNS(foundItem);

				parentController.NavigationController.PushViewController(loadingViewController, true);

				//InventoryDetailViewController inventoryDetailViewController = parentController.Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
				//inventoryDetailViewController.ParentMasterInventoryViewController = parentController;
				//inventoryDetailViewController.InventoryItem = new InventoryNS(foundItem);//RMR building new instead of loading existing

				//parentController.NavigationController.PushViewController(inventoryDetailViewController, true);
			}

		}

		void AddNewItem(object sender, EventArgs args)
		{
			InventoryNS obj = new InventoryNS();

			obj.Sku = AzureService.InventoryService.GetNextSku();
			InventoryDetailViewController inventoryDetailViewController = this.Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
			inventoryDetailViewController.InventoryItem = (InventoryNS)obj;
			inventoryDetailViewController.ParentMasterInventoryViewController = this;
			this.NavigationController.PushViewController(inventoryDetailViewController, true);
		}
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			ShowUpdatingOverlay();
		}
		public override async void ViewDidAppear(bool animated)
		{
			//var bounds = UIScreen.MainScreen.Bounds;
			base.ViewDidAppear(animated);
			if (AzureService.InventoryService.NeedServerRefresh)
			{
				//Show the loading overlay on the UI thread

				await AzureService.DefaultService.PushAsync();
				await AzureService.InventoryService.UpdateItemsFromAzure();
				//await AzureService.InventoryService.UpdateItemsFromLocalDB();

				var items = AzureService.InventoryService.ItemsNS;

				_dsh.Data = items?.Where(x => x.Sold == ShowOnlySoldItems).ToArray();
				_grid.UpdateData();
				//clear loading
				AzureService.InventoryService.NeedServerRefresh = false;
			}
			else if (AzureService.InventoryService.NeedLocalRefresh)
			{
				AzureService.InventoryService.UpdateImagesFromRequiredUpdateList();

				await AzureService.InventoryService.UpdateItemsFromLocalDB();
				//var items = AzureService.InventoryService.GetLocalNSItems()
				var items = AzureService.InventoryService.ItemsNS;

				_dsh.Data = items?.Where(x => x.Sold == ShowOnlySoldItems).ToArray();
				_grid.UpdateData();
				AzureService.InventoryService.NeedLocalRefresh = false;

			}
			else {
				await UpdateGrid();
			}
			if (ScrollToCell != null)
			{
				_grid.ScrollToCell(ScrollToCell, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
			}
			//clear loading
			HideUpdatingOverlay();
		}

		public async Task UpdateGrid()
		{
			if (_grid != null && _dsh != null)//RMR refactor, data can be null thats the whole idea && _dsh.Data != null)
			{
				var items = AzureService.InventoryService.ItemsNS;

				if (FilterButton.TitleLabel.Text.ToLower().Contains("filter"))
				{
					_dsh.Data = items?.Where(x => x.Sold == ShowOnlySoldItems).ToArray();
					_grid.UpdateData();
				}
			}
		}


		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "showInventoryDetail")
			{
				//var indexPath = TableView.IndexPathForSelectedRow;
				//var item = dataSource.Objects [indexPath.Row];
				var controller = (InventoryDetailViewController)((UINavigationController)segue.DestinationViewController).TopViewController;
				//NSObject obj = _dsh.ResolveDataObjectForRow (new Infragistics.IGRowPath (_row.RowIndex, (System.nint)_row.SectionIndex));//, (System.nint)1));
				//controller.SetDetailItem ((InventoryNS)obj);
				controller.NavigationItem.LeftBarButtonItem = SplitViewController.DisplayModeButtonItem;
				controller.NavigationItem.LeftItemsSupplementBackButton = true;
			}
		}

		[Export("gridView:canSlideRowLeft:")]
		public bool CanSlideRowLeft(Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
		{
			return true;
		}

		[Export("gridView:viewForSlideRowLeft:")]
		public UIKit.UIView ResolveSlideRowLeftView(Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
		{
			_row = path;
			return _slideView;
		}

		[Export("ShowDetails")]
		public void ShowDetails()
		{
			NSObject obj = _dsh.ResolveDataObjectForRow(new Infragistics.IGRowPath(_row.RowIndex, (System.nint)_row.SectionIndex));

			InventoryDetailViewController inventoryDetailViewController = this.Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
			inventoryDetailViewController.InventoryItem = (InventoryNS)obj;
			inventoryDetailViewController.ParentMasterInventoryViewController = this;
			this.NavigationController.PushViewController(inventoryDetailViewController, true);
		}



		//Scanning
		public async void Scan()
		{
			//var itemsWithImages = AzureService.InventoryService.Items.Where(x => x.Images != null && x.Images.Count > 0).ToList();
			var scanner = new ZXing.Mobile.MobileBarcodeScanner();
			var result = await scanner.Scan();

			if (result != null)
			{

				var item = AzureService.InventoryService.Items.Where(i => i.Sku == Int32.Parse(result.Text).ToString()).First();

				if (item != null)
				{
					InventoryDetailViewController inventoryDetailViewController = this.Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
					//RMR this is required so creating new InventoryNS doesn't fail
					await AzureService.InventoryService.AllCodesAndIdsSetupConfirmationWithSync();
					inventoryDetailViewController.InventoryItem = new InventoryNS(item);
					inventoryDetailViewController.ParentMasterInventoryViewController = this;
					this.NavigationController.PushViewController(inventoryDetailViewController, true);
				}
				//Console.WriteLine("Scanned Barcode: " + result.Text);
			}

		}

		public async void FilterButdsfton_TouchUpInside(string Value)
		{
			if (Value == "ShowAll")
			{
				await AzureService.DefaultService.PushAsync();

				await AzureService.InventoryService.UpdateItemsFromLocalDB();
				var items = AzureService.InventoryService.ItemsNS;

				_dsh.Data = items?.Where(x => x.Sold == ShowOnlySoldItems).ToArray();
				_grid.UpdateData();
				FilterButton.SetTitle("Filter", UIControlState.Normal);
				return;
			}
			else
			{
				var Code = AzureService.InventoryService.ItemCodes.Where(x => x.Value == Value);
				if (Code.Count() > 0)
				{
					await AzureService.DefaultService.PushAsync(); ;
					var code = Code.ToList()[0].Code;
					await AzureService.InventoryService.UpdateItemsFromLocalDB();
					var itemsArry = AzureService.InventoryService.ItemsNS.Where(x => x.GlItemCode == code
																			&& x.Sold == ShowOnlySoldItems)
																			.ToArray();
					
					_dsh.Data = itemsArry;
					_grid.UpdateData();
					FilterButton.SetTitle(String.Format("Showing {0} {1}", itemsArry.Count(), Utils.Pluralize(Value)), UIControlState.Normal);
				}
				else
				{
					FilterButton.SetTitle(String.Format("Showing 0 {0}", Utils.Pluralize(Value)), UIControlState.Normal);
				}
			}
		}

		public async void FilterByCategory(string Value)
		{
			var CategoryCodes = AzureService.InventoryService.ItemCodes.Where(x => x.Category == Value);

			var listItems = new List<InventoryNS>();
			foreach (var item in CategoryCodes)
			{
				await AzureService.DefaultService.PushAsync();

				//var inventorys = (AzureService.InventoryService.GetLocalNSItems()).Where(x => x.GlItemCode == item.Code);
				var inventorys = (AzureService.InventoryService.ItemsNS).Where(x => x.GlItemCode == item.Code);
				foreach (var i in inventorys)
					listItems.Add(i);
			}

			_dsh.Data = listItems?.Where(x => x.Sold == ShowOnlySoldItems).ToArray();
			_grid.UpdateData();
			FilterButton.SetTitle(String.Format("Showing {0} {1}", listItems.Count(), Utils.Pluralize(Value)), UIControlState.Normal);
		}
		public bool ShowOnlySoldItems
		{
			get;
			set;
		}

		partial void FilterButton_TouchUpInside(UIButton sender)
		{
			View.AddSubview(Alert);
		}


		public class TableSource : UITableViewSource
		{

			string[] TableItems;
			string CellIdentifier = "TableCell";

			public TableSource(string[] items)
			{
				TableItems = items;
			}

			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return TableItems.Length;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
				string item = TableItems[indexPath.Row];

				//---- if there are no cells to reuse, create a new one
				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
				}

				cell.TextLabel.Text = item;

				return cell;
			}
		}

		partial void ScanButton_TouchUpInside(UIButton sender)
		{
			//RefreshGridData ();
			Scan();
		}

		public async void RefreshGridData()
		{

			ShowUpdatingOverlay();

			HideUpdatingOverlay();
		}

	}
}