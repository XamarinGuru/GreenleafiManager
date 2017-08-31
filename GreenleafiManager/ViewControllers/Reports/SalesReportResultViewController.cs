using System;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

namespace GreenleafiManager
{
	public class SalesReportResultViewController : UIView
	{
		public UIView background;

		private nfloat height;
		private nfloat width;

		public bool IsNewCodeAlertPresent;
		private bool IsCategoryAlertOpen;
		public UIView alert;
		//private RoleCustomFilterAlert filterAlert;
		private SalesReportViewController owner;

		public string itemsSold { get; set; }
		public string amountSold { get; set; }
		public string dateRun { get; set; }

		public SalesReportResultViewController(nfloat w, nfloat h, string title, SalesReportViewController owner)
		{
			this.owner = owner;
			height = h;
			width = w;
			Initialize();
		}

		public void AddResults(string Location, string customer, string startDate, string finishDate, string itemsSold, string amountSold, string dateRun)
		{
			for (int i = alert.Subviews.Length - 1; i >= 0; i--)
			{
				alert.Subviews[i].RemoveFromSuperview();
			}
			var LocationLabel = new UILabel() { Text = Location };
			LocationLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
			LocationLabel.Frame = new CGRect(width / 12, 20, 320, 20);

			var CustomerLabel = new UILabel() { Text = customer };
			CustomerLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
			CustomerLabel.Frame = new CGRect(width / 12, 50, 320, 20);

			var startLabel = new UILabel() { Text = startDate };
			startLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
			startLabel.Frame = new CGRect(width / 5, 80, 320, 20);

			var finishLabel = new UILabel() { Text = finishDate };
			finishLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
			finishLabel.Frame = new CGRect(width / 12, 80, 320, 20);

			var itemsSoldLabel = new UILabel() { Text = String.Format("Items sold {0}", itemsSold) };
			itemsSoldLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
			itemsSoldLabel.Frame = new CGRect(width / 12, 110, 320, 20);

			var amountSoldLabel = new UILabel() { Text = String.Format("Amount sold ${0}", amountSold) };
			amountSoldLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
			amountSoldLabel.Frame = new CGRect(width / 12, 140, 320, 20);

			var dateRunLabel = new UILabel() { Text = "Time Run " + dateRun };
			dateRunLabel.Font = UIFont.FromName("Helvetica-Bold", 20f);
			dateRunLabel.Frame = new CGRect(width / 12, 180, 200, 20);

			alert.AddSubview(CustomerLabel);
			alert.AddSubview(LocationLabel);
			alert.AddSubview(startLabel);
			alert.AddSubview(finishLabel);
			alert.AddSubview(itemsSoldLabel);
			alert.AddSubview(amountSoldLabel);
			alert.AddSubview(dateRunLabel);

		}

		private void Initialize()
		{
			//Add background grey color
			background = new UIView() { BackgroundColor = UIColor.Black, Alpha = 0.5f };
			background.Frame = new CGRect(0, 0, width, height);


			alert = new UIView() { BackgroundColor = UIColor.White };
			alert.Frame = new CGRect(width / 3, height / 6.5, width / 3, height / 1.4);
			alert.Layer.CornerRadius = 10f;
			alert.ExclusiveTouch = false;

			alert.ExclusiveTouch = false;

			AddSubview(background);
			AddSubview(alert);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			var touch = touches.AnyObject as UITouch;
			var touchCoordinats = touch.LocationInView(background);
			var a = (touchCoordinats.X > (width / 4));
			var b = touchCoordinats.X < (3 * width / 4);
			var c = (touchCoordinats.Y > (height / 7.5));
			var d = touchCoordinats.Y < (9.3 * height / 12.8);
			if (!(a && b && c && d))
			{

				base.TouchesBegan(touches, evt);
				RemoveFromSuperview();
			}
		}
	}
}

