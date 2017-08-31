using System;
using UIKit;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using System.Linq;

namespace GreenleafiManager
{
	public partial class LocationsAlertViewController : UIView
	{
		public UIView background;

		private nfloat height;
		private List<string> categories; 
		private nfloat width;

		public bool IsNewCodeAlertPresent;
		private bool IsCategoryAlertOpen;
		public UIView alert;
		//private LocationCustomFilterAlert filterAlert;
		private ReportViewController owner;

		private bool IsFilterAlertPresent;
		public LocationsAlertViewController 
		( nfloat w, nfloat h, List<string> _categories, string title,  ReportViewController owner) {
			IsFilterAlertPresent = false;
			this.owner = owner;
			IsCategoryAlertOpen = true;
			categories = _categories;
			height = h;
			width = w;
			Initialize(); 
		}

		public LocationsAlertViewController ( RectangleF bounds ) : base( bounds ) {
			Initialize();
		}

		private void Initialize () {
			
			//Add background grey color
			background =new UIView() {BackgroundColor=UIColor.Black, Alpha = 0.5f};
			background.Frame=new CGRect(0, 0, width, height);


			alert=new UIView(){BackgroundColor=UIColor.White};
			alert.Frame= new CGRect(width / 3, height / 6.5, width/3, height/1.4);
			alert.Layer.CornerRadius = 10f;
			alert.ExclusiveTouch = false; 
			var titleLabel=new UILabel() {Text = "Locations"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			var messageLabel=new UILabel() {Text = "Select a Location"};


			titleLabel.Frame= new CGRect(width/9, 20, 160, 20);
			messageLabel.Frame= new CGRect(width/12, 50, width/3-30, 20);

			var table = new UITableView( Bounds );
			string[] tableItems= categories.ToArray();
			table.Source = new TableSource( tableItems, this );
			table.Frame = new CGRect(5, 90, width / 3.1, height / 1.8);
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
						//filterAlert.newCodeAlert.RemoveFromSuperview();
						IsNewCodeAlertPresent = false;
						//owner.SetAlertOpen( true );
					}
					IsFilterAlertPresent = false;
					//filterAlert.RemoveFromSuperview();
				}

				if ( !IsCategoryAlertOpen )
					AddSubview( alert );
			}
		}

		public void CloseAlert()
		{
			RemoveFromSuperview();
			if (IsFilterAlertPresent)
			{
				if (IsNewCodeAlertPresent)
				{
					//filterAlert.newCodeAlert.RemoveFromSuperview();
					IsNewCodeAlertPresent = false;
					//owner.SetAlertOpen( true );
				}
				IsFilterAlertPresent = false;
				IsCategoryAlertOpen = false;
				//filterAlert.RemoveFromSuperview();
			}
		}

		public class TableSource : UITableViewSource
		{
			private readonly string CellIdentifier = "TableCell";

			private readonly string[] TableItems;

			private LocationsAlertViewController owner;
			public TableSource(string[] items, LocationsAlertViewController owner )
			{
				TableItems = items;
				this.owner = owner;
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				//owner.alert.RemoveFromSuperview();
				owner.owner.UpdateLocationCode(TableItems[indexPath.Row]);
				owner.CloseAlert();

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

		




