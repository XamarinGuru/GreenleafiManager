using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace GreenleafiManager.ViewControllers {
    [Register ( "CustomAlert" )]
    public class CustomAlert : UIView {
        
		public UIView background;
        public bool IsNewCodeAlertPresent;
        private nfloat height;
        private List<string> categories; 
        private nfloat width;
       
        private bool IsCategoryAlertOpen;
        public UIView alert;
        private CustomFilterAlert filterAlert;
		private MasterInventoryViewController owner;

		private bool IsFilterAlertPresent;
		public CustomAlert ( nfloat w, nfloat h, List<string> _categories, string title, MasterInventoryViewController owner) {
			IsFilterAlertPresent = false;
			this.owner = owner;
            IsCategoryAlertOpen = true;
            categories = _categories;
            height = h;
            width = w;
            Initialize(); 
        }

        public CustomAlert ( RectangleF bounds ) : base( bounds ) {
            Initialize();
        }

        private void Initialize () {
            //Add background grey color
            background =new UIView() {BackgroundColor=UIColor.Black, Alpha = 0.5f};
            background.Frame=new CGRect(0, 0, width, height);
            background.ExclusiveTouch = true;


            alert=new UIView(){BackgroundColor=UIColor.White};
            alert.Frame= new CGRect(width / 3, height / 7, width/3, height/1.4);
            alert.Layer.CornerRadius = 10f;
			alert.ExclusiveTouch = false; 
			 
			var titleLabel=new UILabel() {Text = "Category"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
            var messageLabel=new UILabel() {Text = "Select an category"};


            titleLabel.Frame= new CGRect(width/9, 20, 160, 20);
			messageLabel.Frame= new CGRect(width/11, 50, width/3-30, 20);

            var table = new UITableView( Bounds );
			string[] tableItems =new string[categories.Count+1] ;
			tableItems[0]="Show All";
			for(int i=0; i<categories.Count; i++)
				tableItems[i+1]=categories[i];

            table.Source = new TableSource( tableItems, this );
            table.Frame = new CGRect(5, 100, width / 3.1, height / 1.8);
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
            base.TouchesBegan(touches, evt);
		    var touch = touches.AnyObject as UITouch;
		    var touchCoordinats=touch.LocationInView( background );
            var a= (touchCoordinats.X > (width / 4));
		    var b = touchCoordinats.X < (3*width/4);
            var c=(touchCoordinats.Y > (height / 7.5));
		    var d = touchCoordinats.Y < (9.3*height/12.8);
            if (!(a&&b&&c&&d)||!IsNewCodeAlertPresent)
            {

                RemoveFromSuperview();
                if ( IsFilterAlertPresent ) {
                    if (  IsNewCodeAlertPresent ) {
                        filterAlert.newCodeAlert.RemoveFromSuperview();
                         IsNewCodeAlertPresent = false;
                    }
                    IsFilterAlertPresent = false;
                    filterAlert.RemoveFromSuperview();

                }
                if ( !IsCategoryAlertOpen ) { AddSubview( alert ); }
            }
		}

		public class TableSource : UITableViewSource
        {
            private readonly string CellIdentifier = "TableCell";

            private readonly string[] TableItems;

            private CustomAlert owner;
            public TableSource(string[] items, CustomAlert owner )
            {
                TableItems = items;
                this.owner = owner;

            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                owner.IsCategoryAlertOpen = false;
                owner.alert.RemoveFromSuperview();
				owner.IsFilterAlertPresent = true;
				if (indexPath.Row == 0) {
					owner.owner.FilterButdsfton_TouchUpInside ("ShowAll");

					owner.RemoveFromSuperview ();
					owner.Add(owner.alert);
				}
				if (indexPath.Row > 0) {
					owner.filterAlert = new CustomFilterAlert (owner.height, owner.width, TableItems [indexPath.Row], "sdf", "", owner.owner, owner);
					owner.filterAlert.Frame = new CGRect (0, 20, owner.width, owner.height);
					owner.AddSubview (owner.filterAlert);
				}
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
		private MasterInventoryViewController owner;
		private CustomAlert masterAlert;
		public UITableView table;

		public string Category;
		public CustomFilterAlert(nfloat h,  nfloat w, string category, string title, string message, MasterInventoryViewController owner, CustomAlert masterAlert)
        {
			this.masterAlert = masterAlert;
			this.owner = owner;
			Category = category;
			masterAlert.IsNewCodeAlertPresent = false;
			//SetupInventoryService();
			//InventoryService.LoadItemsFromLocalDB();
			var listCodes = AzureService.InventoryService.ItemCodes.Distinct().ToList().Where(x => x.Category == category);
			var TableList = new List<string>();
			foreach ( var i in listCodes ) {
				TableList.Add( i.Value );
			}

			categories = TableList;
            height = h;
            width = w;
            Initialize();
        }

		//private async void SetupInventoryService()
		//{
		//	_InventoryService = AzureService.InventoryService;
		//	//await _InventoryService.InitializeStoreAsync();
		//}

        public CustomFilterAlert(RectangleF bounds) : base(bounds)
        {
            Initialize();
        }

        private void Initialize()
        {
            alert = new UIView() { BackgroundColor = UIColor.White };
            alert.Frame = new CGRect(width / 3, height / 8.5, width / 3, height / 1.3);

			var titleLabel=new UILabel() {Text = "Item Codes"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			titleLabel.Frame= new CGRect(width/9, 20, 160, 20);

             table =  new UITableView(Bounds);
			string[] tableItems = new string[categories.Count+1];
			tableItems[0]="Show All";

			for (int i = 0; i < categories.Count; i++) {
				tableItems [i + 1] = categories [i];
			}
            table.Source = new TableSource(tableItems, this);
            table.Frame = new CGRect(5, 65, width / 3.1, height / 1.8);
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
			alert.AddSubview (button);
           
            AddSubview(alert);
        }

        private void Button_TouchDown(object sender, EventArgs e)
        {
           alert.RemoveFromSuperview();

			newCodeAlert.CancelCodeButton.TouchDown+=(o, _e)=>{
				newCodeAlert.RemoveFromSuperview();
				AddSubview(alert);
				masterAlert.IsNewCodeAlertPresent=false;
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
				masterAlert.IsNewCodeAlertPresent=false;
					await AzureService.DefaultService.PushAsync();//RMR TODO Refactor might not need this one
																  //AzureService.InventoryService.GetLocalNSItems();
					await AzureService.InventoryService.UpdateItemsFromLocalDB();//RMR TODO might not need this either
				var listCodes = AzureService.InventoryService.ItemCodes.Distinct().ToList().Where(x => x.Category == data[0]);
				var TableList = new List<string>();
				foreach ( var i in listCodes ) {
					TableList.Add( i.Value );
				}
				table.Source=new TableSource(TableList.ToArray(), this);                                                            
				}
				newCodeAlert.RemoveFromSuperview();
				AddSubview(alert);
			};
			AddSubview (newCodeAlert);
			masterAlert.IsNewCodeAlertPresent = true;
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
				if (indexPath.Row == 0) {
					owner.owner.FilterByCategory (owner.Category);
				} else {
					owner.owner.FilterButdsfton_TouchUpInside (TableItems [indexPath.Row]);
				}
				owner.masterAlert.RemoveFromSuperview ();
				owner.RemoveFromSuperview ();
				owner.masterAlert.AddSubview (owner.masterAlert.alert);
				tableView.DeselectRow (indexPath, true); 
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