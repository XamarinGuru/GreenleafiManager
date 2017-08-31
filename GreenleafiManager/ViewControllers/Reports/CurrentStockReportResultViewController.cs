using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace GreenleafiManager
{
	public class CurrentStockReportResultViewController: UIView
	{
		public UIView background;

		private nfloat height;
		private nfloat width;

		public bool IsNewCodeAlertPresent;
		private bool IsCategoryAlertOpen;
		public UIView alert;
		//private RoleCustomFilterAlert filterAlert;
		private CurrentStockReportViewController owner;

		public string itemsSold {get;set;}
		public string amountSold {get;set;}
		public string dateRun {get;set;}

		public CurrentStockReportResultViewController ( nfloat w, nfloat h,  string title,  CurrentStockReportViewController owner)
		{
			this.owner = owner;
			height = h;
			width = w;
			Initialize(); 
		}

		public void AddResults(string Location, string items, string amount, string dateRun)
		{
			foreach (var v in this.alert.Subviews)
			{
				v.RemoveFromSuperview();
			}

			var LocationLabel=new UILabel() {Text = Location};
			LocationLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			LocationLabel.Frame= new CGRect(width/24, 20, 320, 20);

			var itemsLabel=new UILabel() {Text = String.Format("Items {0}", items)};
			itemsLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			itemsLabel.Frame= new CGRect(width/24, 50, 320, 20);

			var amountLabel=new UILabel() {Text = String.Format("Value (tag) ${0}",amount)};
			amountLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			amountLabel.Frame= new CGRect(width/24, 80, 320, 20);

			var dateRunLabel=new UILabel() {Text = "Time Run " + dateRun};
			dateRunLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			dateRunLabel.Frame= new CGRect(width/24, 110, 200, 20);

			alert.AddSubview (LocationLabel);
			alert.AddSubview (itemsLabel);
			alert.AddSubview (amountLabel);
			alert.AddSubview (dateRunLabel);
		}

		private void Initialize () {
			//Add background grey color
			background =new UIView() {BackgroundColor=UIColor.Black, Alpha = 0.5f};
			background.Frame=new CGRect(0, 0, width, height);


			alert=new UIView(){BackgroundColor=UIColor.White};
			alert.Frame= new CGRect(width / 3, height / 6.5, width/3, height/1.4);
			alert.Layer.CornerRadius = 10f;
			alert.ExclusiveTouch = false; 

			alert.ExclusiveTouch = false;

			AddSubview( background );
			AddSubview( alert );
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			var touch = touches.AnyObject as UITouch;
			var touchCoordinats = touch.LocationInView(background);
			var a = (touchCoordinats.X > (width / 4));
			var b = touchCoordinats.X < (3 * width / 4);
			var c = (touchCoordinats.Y > (height / 7.5));
			var d = touchCoordinats.Y < (9.3 * height / 12.8);
			if ( !(a && b && c && d)) {

				base.TouchesBegan( touches, evt );
				RemoveFromSuperview();
			}
		}
	}
}

