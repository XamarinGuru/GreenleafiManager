using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;

namespace GreenleafiManager
{
	public partial class CurrentStockReportViewController : ReportViewController
	{
		//Static layout data
		nfloat initialTopMargin = DefaultDetailsLayoutSettings.InitialTopMargin;
		nfloat initialLeftMargin = DefaultDetailsLayoutSettings.InitialLeftMargin;

		nfloat defaultLabelHeight = DefaultDetailsLayoutSettings.DefaultLabelHeight;
		nfloat defaultLabelWidth = DefaultDetailsLayoutSettings.DefaultLabelWidth;

		//Editable fields
		UIButton locationButton;
		UIButton runReportButton;

		private LocationsAlertViewController LocationSelectionAlert;

		private CurrentStockReportResultViewController ReportResultsAlert;

		public List<string> Locations
		{
			get
			{
				List<Location> locs = AzureService.LocationService.Locations;
				List<string> ret = new List<string>();
				foreach (var l in locs)
				{
					ret.Add(l.Name);
				}
				return ret;
			}
		}
		public CurrentStockReportViewController(IntPtr handle) : base(handle)
		{

		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.NavigationController.SetNavigationBarHidden(false, false);
			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;

			//await _LocationService.InitializeStoreAsync();
			await AzureService.LocationService.UpdateLocationsFromLocalDB();

			LocationSelectionAlert = new LocationsAlertViewController(View.Bounds.Size.Width, View.Bounds.Height, Locations, "", this);

			ReportResultsAlert = new CurrentStockReportResultViewController(View.Bounds.Size.Width, View.Bounds.Height, "", this);


			SetupViewUIItems();
			ConfigureView();
		}

		private void SetupViewUIItems()
		{
			BuildLocationUI(initialLeftMargin, initialTopMargin);
			BuildRunReportButtonUI(locationButton.Frame.X, locationButton.Frame.Y + defaultLabelHeight * 2);

		}
		private void BuildLocationUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.MinimumFontSize = 12f;
			label.Text = "Location";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			View.AddSubview(label);

			locationButton = UIButton.FromType(UIButtonType.System);
			locationButton.Frame = new CGRect(initialX, label.Frame.Y + defaultLabelHeight, 200f, defaultLabelHeight);
			var roleText = "Select a location";
			locationButton.SetTitle(roleText, UIControlState.Normal);

			locationButton.TitleLabel.TextColor = UIColor.LightGray;
			locationButton.SetTitleColor(GlobalUISettings.TextPlaceHolderColor, UIControlState.Normal);

			locationButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			locationButton.TitleLabel.TextAlignment = UITextAlignment.Left;
			locationButton.TitleLabel.Font = GlobalUISettings.SelectionListButtonFont; ;
			locationButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
			locationButton.TitleLabel.MinimumFontSize = 10f;
			locationButton.TouchUpInside += Location_TouchUpInside;

			View.AddSubview(locationButton);
		}
		private void BuildRunReportButtonUI(nfloat initialX, nfloat initialY)
		{

			runReportButton = UIButton.FromType(UIButtonType.System);
			runReportButton.Frame = new CGRect(initialX, initialY, 200f, defaultLabelHeight);
			var roleText = "Run Report";
			runReportButton.SetTitle(roleText, UIControlState.Normal);

			runReportButton.TitleLabel.TextColor = UIColor.LightGray;
			runReportButton.SetTitleColor(GlobalUISettings.ButtonColor, UIControlState.Normal);

			runReportButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			runReportButton.TitleLabel.TextAlignment = UITextAlignment.Left;
			runReportButton.TitleLabel.Font = GlobalUISettings.ButtonFont; ;
			runReportButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
			runReportButton.TitleLabel.MinimumFontSize = 10f;
			runReportButton.TouchUpInside += RunReport_TouchUpInside;

			View.AddSubview(runReportButton);
		}

		private void ConfigureView()
		{
			LocationSelectionAlert = new LocationsAlertViewController(View.Bounds.Size.Width, View.Bounds.Height, Locations, "", this);
		}

		private bool TextFieldShouldReturn(UITextField textfield)
		{
			nint nextTag = textfield.Tag + 1;
			UIResponder nextResponder = this.View.ViewWithTag(nextTag);
			if (nextResponder != null)
			{
				nextResponder.BecomeFirstResponder();
			}
			else {
				// Not found, so remove keyboard.
				textfield.ResignFirstResponder();
				//ResetTheScroller();
			}
			return false;
		}
		private void Location_TouchUpInside(object sender, EventArgs e)
		{
			LocationSelectionAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
			View.AddSubview(LocationSelectionAlert);
		}
		private void RunReport_TouchUpInside(object sender, EventArgs e)
		{
			string locId = null;
			Location location = null;
			if (locationButton.TitleLabel.Text != null && AzureService.LocationService.Locations != null)
			{
				location = AzureService.LocationService.Locations.Where(x => x.Name == locationButton.TitleLabel.Text).FirstOrDefault();
				if (location != null)
					locId = location.Id;
				else
				{
					this.ShowMessageAlert("Location Required", "Please select location.");
					return;
				}
			}

			var results = AzureService.ReportsService.StockReport(locId);
			CurrentStockReportResultsViewController csrrvc = this.Storyboard.InstantiateViewController("CurrentStockReportResultsViewController") as CurrentStockReportResultsViewController;

			csrrvc.StockReportModel = results;
			csrrvc.Location = location == null ? "Invalid Data" : location.Name;

			this.NavigationController.PushViewController(csrrvc, true);
			//ReportResultsAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
			//ReportResultsAlert.AddResults(locationButton.TitleLabel.Text, results.TotalOfItems.ToString("C"), results.TotalValueOfItems.ToString("C"), DateTime.Now.ToString("MM/dd/yyyy"));
			//View.AddSubview(ReportResultsAlert);
		}
		public override void UpdateLocationCode(string code)
		{
			locationButton.SetTitle(code, UIControlState.Normal);
			locationButton.TitleLabel.TextColor = GlobalUISettings.SelectionListButtonColor;
			locationButton.SetTitleColor(GlobalUISettings.SelectionListButtonColor, UIControlState.Normal);
		}

	}

}
