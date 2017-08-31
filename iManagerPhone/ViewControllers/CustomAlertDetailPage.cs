using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using GreenleafiManagerPhone;
using System.Threading.Tasks;

namespace GreenleafiManagerPhone.ViewControllers
{  
	public class CustomAlertDetailPage : UIView {

		public UIView background;

		private nfloat height;
		private List<string> categories; 
		private nfloat width;

        public bool IsNewCodeAlertPresent;
        private bool IsCategoryAlertOpen;
		public UIView alert;
		private CustomFilterAlert filterAlert;
		private InventoryDetailViewController owner;

		private bool IsFilterAlertPresent;
		public CustomAlertDetailPage ( nfloat w, nfloat h, List<string> _categories, string title,  InventoryDetailViewController owner) {
			IsFilterAlertPresent = false;
			this.owner = owner;
			IsCategoryAlertOpen = true;
			categories = _categories.OrderBy(q => q).ToList();
			height = h;
			width = w;
			Initialize(); 
		}

		public CustomAlertDetailPage ( RectangleF bounds ) : base( bounds ) {
			Initialize();
		}

		private void Initialize () {
			//Add background grey color
			background =new UIView() {BackgroundColor=UIColor.Black, Alpha = 0.5f};
			background.Frame=new CGRect(0, 0, width, height);


			alert=new UIView(){BackgroundColor=UIColor.White};
			alert.Frame= new CGRect(width / 8, height / 6.5, (width / 8) * 6, height / 1.4);
			alert.Layer.CornerRadius = 10f;
			alert.ExclusiveTouch = false; 
			var titleLabel=new UILabel() {Text = "Category"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			var messageLabel=new UILabel() {Text = "Select an category"};
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.Frame = new CGRect(5, 20, width / 1.5, 20);
			messageLabel.Frame= new CGRect(width/12, 50, width/3-30, 20);

			var table = new UITableView( Bounds );
			string[] tableItems= categories.ToArray();
			table.Source = new TableSource( tableItems, this );
			table.Frame = new CGRect(5, 90, width / 1.5, height / 1.8);
			table.Layer.BackgroundColor = new CoreGraphics.CGColor( 0,0,0, 0.6f );

			alert.ExclusiveTouch = false;
			alert.AddSubview( titleLabel );
			alert.AddSubview( messageLabel );
			alert.AddSubview( table );

			AddSubview( background );
			AddSubview(alert );
		}


		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
            var touch = touches.AnyObject as UITouch;
            var touchCoordinats = touch.LocationInView(background);
            var a = (touchCoordinats.X > (width / 4));
            var b = touchCoordinats.X < (3 * width / 4);
            var c = (touchCoordinats.Y > (height / 7.5));
            var d = touchCoordinats.Y < (9.3 * height / 12.8);
		    if ( !(a && b && c && d) || !IsNewCodeAlertPresent ) {

		        base.TouchesBegan( touches, evt );
		        RemoveFromSuperview();
		        if ( IsFilterAlertPresent ) {
		            if ( IsNewCodeAlertPresent ) {
		                filterAlert.newCodeAlert.RemoveFromSuperview();
		                IsNewCodeAlertPresent = false;
		                owner.SetAlertOpen( true );
		            }
		            IsFilterAlertPresent = false;
		            filterAlert.RemoveFromSuperview();
		        }

		        if ( !IsCategoryAlertOpen )
		            AddSubview( alert );
		    }
		}

		public class TableSource : UITableViewSource
		{
			private readonly string CellIdentifier = "TableCell";

			private readonly string[] TableItems;

			private CustomAlertDetailPage owner;
			public TableSource(string[] items, CustomAlertDetailPage owner )
			{
				TableItems = items;
				this.owner = owner;
			}

			public override async void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
			//	base.RowSelected (tableView, indexPath);
				owner.IsCategoryAlertOpen = false;
				owner.alert.RemoveFromSuperview();
				owner.IsFilterAlertPresent = true;
				owner.filterAlert = await CustomFilterAlert.CreateAsync(owner.height, owner.width, TableItems[indexPath.Row],"sdf", "", owner.owner, owner);
				owner.filterAlert.Frame = new CGRect(0, 20, owner.width, owner.height);
				owner.AddSubview(owner.filterAlert);

				tableView.DeselectRow(indexPath, true);
			}
			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return TableItems.Length;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CellIdentifier);
				var item = TableItems[indexPath.Row];

				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
				}

				cell.TextLabel.Text = item;

				return cell;
			}
		}
	}

	public class CustomFilterAlert : UIView
	{
		private nfloat height;
		private List<string> categories;
		private nfloat width;
		private UIButton button;
		public NewCodeAlert newCodeAlert;
		private UIView alert;
		private  InventoryDetailViewController owner;
		private CustomAlertDetailPage masterAlert;
		private UITableView table;
		private string Category = "";

		private CustomFilterAlert()//nfloat h,  nfloat w, string category, string title, string message,  InventoryDetailViewController owner, CustomAlertDetailPage masterAlert)
		{
			//this.Category = category;
			//this.masterAlert = masterAlert;
			//this.owner = owner;
			//masterAlert.IsNewCodeAlertPresent = false;
			//var InventoryService = QSAzureService.InventoryService;
			//var res = InventoryService.InitializeStoreAsync().Result; 
			////InventoryService.LoadItemsFromLocalDB();
			//var listCodes = InventoryService.ItemCodes.Distinct().ToList().Where(x => x.Category == category);
			//var TableList = new List<string>();
			//foreach ( var i in listCodes ) {
			//	TableList.Add( i.Value );
			//}

			//categories = TableList;
			//height = h;
			//width = w;
			//Initialize();
		}
		private async Task<CustomFilterAlert> InitalizeAsync(nfloat h, nfloat w, string category, string title, string message, InventoryDetailViewController owner, CustomAlertDetailPage masterAlert)
		{
			this.Category = category;
			this.masterAlert = masterAlert;
			this.owner = owner;
			masterAlert.IsNewCodeAlertPresent = false;
			var InventoryService = AzureService.InventoryService;
			//var res = await InventoryService.InitializeStoreAsync();
			//InventoryService.LoadItemsFromLocalDB();
			var listCodes = InventoryService.ItemCodes.Distinct().ToList().Where(x => x.Category == category).OrderBy(x=>x.Value).ToList();
			var TableList = new List<string>();
			foreach (var i in listCodes)
			{
				TableList.Add(i.Value);
			}

			categories = TableList;
			height = h;
			width = w;
			Initialize();
			return this;
		}
		public static Task<CustomFilterAlert> CreateAsync(nfloat h, nfloat w, string category, string title, string message, InventoryDetailViewController owner, CustomAlertDetailPage masterAlert)
		{
			var ret = new CustomFilterAlert();
			return ret.InitalizeAsync(h, w, category, title, message, owner, masterAlert);
		}
		public CustomFilterAlert(RectangleF bounds) : base(bounds)
		{
			Initialize();
		}

		private void Initialize()
		{
			//Add background grey color

			alert = new UIView() { BackgroundColor = UIColor.White };
			alert.Frame = new CGRect(width / 8, height / 6.5, (width / 8) * 6, height / 1.4);

			var titleLabel=new UILabel() {Text = "Item Codes"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.Frame = new CGRect(5, 20, width / 1.5, 20);

			table =  new UITableView(Bounds);
			string[] tableItems = categories.ToArray();
			table.Source = new TableSource(tableItems, this);
			table.Frame = new CGRect(5, 90, width / 1.5, height / 1.8);
			table.Layer.BackgroundColor = new CoreGraphics.CGColor(0, 0, 0, 0.6f);

			button = UIButton.FromType(UIButtonType.RoundedRect);
			button.SetTitle ("+", UIControlState.Normal);
			button.Frame = new CGRect(5, height / 1.8+110, width / 3.1, height / 15);
			button.Layer.BackgroundColor = new CoreGraphics.CGColor(1,1,1,1 );
			button.Layer.BorderWidth = 2;
			button.Layer.CornerRadius = 5;
			button.Layer.BorderColor = new CoreGraphics.CGColor(0,0,0,1);
			button.TouchDown += Button_TouchDown;

			newCodeAlert = new NewCodeAlert (width, height);
			newCodeAlert.Frame=new CoreGraphics.CGRect(0, 20, width, height);

			alert.Layer.CornerRadius = 10f;
			alert.AddSubview(titleLabel);
			alert.AddSubview(table);
			//alert.AddSubview (button);

			AddSubview(alert);
		}

		private void Button_TouchDown(object sender, EventArgs e)
		{
			alert.RemoveFromSuperview();

			newCodeAlert.CancelCodeButton.TouchDown+=(o, _e)=>{
				newCodeAlert.RemoveFromSuperview();
				AddSubview(alert);
                masterAlert.IsNewCodeAlertPresent =false;
				owner.SetAlertOpen (true);
			};

			newCodeAlert.SaveCodeButton.TouchDown +=async (o, _e) => {
				var data=newCodeAlert.GetDataToSave();
				var p=AzureService.InventoryService.ItemCodes.ToList().Where(x=>x.Code==data[2]&&x.Value==data[1]);

				if(p.ToList().Count==0){
				await AzureService.InventoryService.InsertIntoItemCodes(new ItemCode{
					Category=data[0],
					Value=data[1],
					Code=data[2]
				});
                    masterAlert.IsNewCodeAlertPresent =false;
				owner.SetAlertOpen (true);
					await AzureService.DefaultService.PushAsync();
					//AzureService.InventoryService.GetLocalNSItems();//RMR TODO Refactor - might not need this any more
					//await AzureService.InventoryService.UpdateItemsFromLocalDB();//RMR TODO Refactor - might not need this any more
				var listCodes = AzureService.InventoryService.ItemCodes.Distinct().ToList().Where(x => x.Category == data[0]);
				var TableList = new List<string>();
				foreach ( var i in listCodes ) {
					TableList.Add( i.Value );
				}
					table.Source=new TableSource(TableList.ToArray(), this);
				};
				newCodeAlert.RemoveFromSuperview();
				AddSubview(alert);
			};
			AddSubview (newCodeAlert);
            masterAlert.IsNewCodeAlertPresent = true;
			owner.SetAlertOpen (true);
		}

		public class TableSource : UITableViewSource
		{
			private readonly string CellIdentifier = "TableCell";

			private readonly string[] TableItems;

			private CustomFilterAlert owner;
			public TableSource(string[] items, CustomFilterAlert owner) {
				this.owner = owner;
				TableItems = items;
			}

			public override  void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var code = AzureService.InventoryService.ItemCodes.Distinct ().Where (x => x.Value == TableItems[indexPath.Row]);
					 
				owner.owner.UpdateCode (code.ToList()[0].Code); 
				tableView.DeselectRow(indexPath, true); 

				owner.masterAlert.RemoveFromSuperview();
				owner.RemoveFromSuperview ();
				owner.masterAlert.AddSubview(owner.masterAlert.alert);
			}
			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return TableItems.Length;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{


				var cell = tableView.DequeueReusableCell(CellIdentifier);
				var item = TableItems[indexPath.Row];

				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
				}

				cell.TextLabel.Text = item;

				return cell;
			}
		}

	}

	public class MetalsAlert : UIView {

		public UIView background;

		private nfloat height;
		private List<string> categories; 
		private nfloat width;

		private bool IsCategoryAlertOpen;
		public UIView alert; 
		private InventoryDetailViewController owner;

		//private bool IsFilterAlertPresent;
		public MetalsAlert( nfloat w, nfloat h, List<string> _categories, InventoryDetailViewController owner) {
			this.owner = owner; 
			categories = _categories;
			height = h;
			width = w;
			Initialize(); 
		}

		public MetalsAlert( RectangleF bounds ) : base( bounds ) {
			Initialize();
		}

		private void Initialize () {
			//Add background grey color
			background =new UIView() {BackgroundColor=UIColor.Black, Alpha = 0.5f};
			background.Frame=new CGRect(0, 0, width, height);


			alert=new UIView(){BackgroundColor=UIColor.White};
			alert.Frame= new CGRect(width / 8, height / 6.5, (width/8)*6, height/1.4);
			alert.Layer.CornerRadius = 10f;
			alert.ExclusiveTouch = false; 

			var titleLabel=new UILabel() {Text = "Metal codes"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.Frame= new CGRect(5, 20, width / 1.5, 20); 

			var table = new UITableView( Bounds );
			string[] tableItems = new string[] {
												"10K Y/G",
								                "10K W/G",
												"10K P/G",
												"10K TT/G",
												"10K TRI/G",
												"",
                                                "14K Y/G",
												"14K W/G",
												"14K P/G",
												"14K TT/G",
												"14K TRI/G",
                                                "",
												"18K Y/G",
												"18K W/G",
												"18K P/G",
												"18K TT/G",
												"18K TRI/G",
                                                "",
												"24K Y/G",
												"24K W/G",
                                                "",
												"Platinum",
                                                "",
												"Wood",
												"Tungsten",
												"Stainless Steel"
			}; 
			categories.ToArray();
			table.Source = new TableSource( tableItems, this);
			table.Frame = new CGRect(5, 90, width/ 1.5, height / 1.8);
			table.Layer.BackgroundColor = new CoreGraphics.CGColor( 0,0,0, 0.6f );

			alert.ExclusiveTouch = false;
			alert.AddSubview( titleLabel );
			alert.AddSubview( table );

			AddSubview( background );
			AddSubview(alert );
		}


		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			RemoveFromSuperview();
			 
			if(!IsCategoryAlertOpen)
				AddSubview(alert);

		}
		public class TableSource : UITableViewSource
		{
			private readonly string CellIdentifier = "TableCell";

			private readonly string[] TableItems;

			private MetalsAlert owner;
			public TableSource(string[] items, MetalsAlert owner )
			{
				TableItems = items;
				this.owner = owner;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				if (String.IsNullOrWhiteSpace(TableItems[indexPath.Row]))
					return;
				
				owner.owner.SetMetalCode (TableItems[indexPath.Row]);
				owner.RemoveFromSuperview ();
				tableView.DeselectRow(indexPath, true);
			}
			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return TableItems.Length;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CellIdentifier);
				var item = TableItems[indexPath.Row];

				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
				}

				cell.TextLabel.Text = item;

				return cell;
			}
		}


	}

	public class LocationsAlert : UIView {

		public UIView background;

		private nfloat height;
		private List<string> locations; 
		private nfloat width;

		private bool IsCategoryAlertOpen;
		public UIView alert; 
		private InventoryDetailViewController owner;

		private bool IsFilterAlertPresent;
		public LocationsAlert( nfloat w, nfloat h, List<string> _locations, InventoryDetailViewController owner) {
			this.owner = owner; 
			locations = _locations;
			height = h;
			width = w;
			Initialize(); 
		}

		public LocationsAlert( RectangleF bounds ) : base( bounds ) {
			Initialize();
		}

		private void Initialize () {
			//Add background grey color
			background =new UIView() {BackgroundColor=UIColor.Black, Alpha = 0.5f};
			background.Frame=new CGRect(0, 0, width, height);


			alert=new UIView(){BackgroundColor=UIColor.White};
			alert.Frame= new CGRect(width / 8, height / 6.5, (width / 8) * 6, height / 1.4);
			alert.Layer.CornerRadius = 10f;
			alert.ExclusiveTouch = false; 

			var titleLabel=new UILabel() {Text = "Locations"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f); 
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.Frame = new CGRect(5, 20, width / 1.5, 20);

			var table = new UITableView( Bounds );
			string[] tableItems = locations.ToArray();
			table.Source = new TableSource( tableItems, this);
			table.Frame = new CGRect(5, 90, width / 1.5, height / 1.8);
			table.Layer.BackgroundColor = new CoreGraphics.CGColor( 0,0,0, 0.6f );

			alert.ExclusiveTouch = false;
			alert.AddSubview( titleLabel );
			alert.AddSubview( table );

			AddSubview( background );
			AddSubview(alert );
		}


		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			RemoveFromSuperview();

			if(!IsCategoryAlertOpen)
				AddSubview(alert);

		}
		public class TableSource : UITableViewSource
		{
			private readonly string CellIdentifier = "TableCell";

			private readonly string[] TableItems;

			private LocationsAlert owner;
			public TableSource(string[] items, LocationsAlert owner )
			{
				TableItems = items;
				this.owner = owner;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				owner.owner.SetLocation (TableItems[indexPath.Row]);
				owner.RemoveFromSuperview ();
				tableView.DeselectRow(indexPath, true);
			}
			public override nint RowsInSection(UITableView tableview, nint section)
			{
				return TableItems.Length;
			}

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell(CellIdentifier);
				var item = TableItems[indexPath.Row];

				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
				}

				cell.TextLabel.Text = item;

				return cell;
			}
		}


	}
}

