using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;

namespace GreenleafiManager
{
	public partial class SalesReportViewController : ReportViewController
	{
		//Static layout data
		nfloat initialTopMargin = DefaultDetailsLayoutSettings.InitialTopMargin;
		nfloat initialLeftMargin = DefaultDetailsLayoutSettings.InitialLeftMargin;
		nfloat initialRightMargin = DefaultDetailsLayoutSettings.InitialRightMargin;

		nfloat defaultLabelHeight = DefaultDetailsLayoutSettings.DefaultLabelHeight;
		nfloat defaultLabelWidth = DefaultDetailsLayoutSettings.DefaultLabelWidth;

		nfloat defaultTextFieldHeight = DefaultDetailsLayoutSettings.DefaultTextFieldHeight;
		nfloat defaultTextFieldWidth = DefaultDetailsLayoutSettings.DefaultTextFieldWidth;

		nfloat defaultVerticalSpacing = DefaultDetailsLayoutSettings.DefaultVerticalSpacing;
		nfloat defaultItemVerticalSpacing = DefaultDetailsLayoutSettings.DefaultItemVerticalSpacing;


		//Editable fields
		UITextField endDateTextField;
		UITextField startDateTextField;

		UIButton locationButton;
		UIButton userButton;
		UIButton runReportButton;

		private LocationsAlertViewController LocationSelectionAlert;
		private UsersAlertViewController UserSelectionAlert;

		private SalesReportResultViewController ReportResultsAlert;
		private NSDateFormatter dateFormatter;
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

		public List<string> Users
		{
			get
			{
				List<User> users = AzureService.UserService.Users;
				List<string> ret = new List<string>();
				foreach (var u in users)
				{
					ret.Add(u.UserName);
				}
				return ret;
			}
		}


		public SalesReportViewController(IntPtr handle) : base(handle)
		{

		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();
			dateFormatter = new NSDateFormatter()
			{
				DateFormat = "MM/dd/yyyy"
			};
			this.NavigationController.SetNavigationBarHidden(false, false);
			ReportResultsAlert = new SalesReportResultViewController(View.Bounds.Size.Width, View.Bounds.Height, "", this);
			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;

			//await _LocationService.InitializeStoreAsync();
			await AzureService.LocationService.UpdateLocationsFromLocalDB();

			// _UserService.InitializeStoreAsync();
			await AzureService.UserService.UpdateUsersFromLocalDB();

			SetupViewUIItems();
			ConfigureView();

			CreatePickerStartDate();
			CreatePickerFinishDate();

		}

		private void SetupViewUIItems()
		{
			BuildLocationUI(initialLeftMargin, initialTopMargin);
			BuildUsersUI(initialLeftMargin, locationButton.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildDateUI(initialLeftMargin, userButton.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildRunReportButtonUI(initialLeftMargin, endDateTextField.Frame.Y + defaultTextFieldHeight * 2 + defaultItemVerticalSpacing);
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
		private void BuildUsersUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.MinimumFontSize = 12f;
			label.Text = "User";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			View.AddSubview(label);

			userButton = UIButton.FromType(UIButtonType.System);
			userButton.Frame = new CGRect(initialX, label.Frame.Y + defaultLabelHeight, 200f, defaultLabelHeight);
			var roleText = "Select a user";
			userButton.SetTitle(roleText, UIControlState.Normal);

			userButton.TitleLabel.TextColor = UIColor.LightGray;
			userButton.SetTitleColor(GlobalUISettings.TextPlaceHolderColor, UIControlState.Normal);

			userButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			userButton.TitleLabel.TextAlignment = UITextAlignment.Left;
			userButton.TitleLabel.Font = GlobalUISettings.SelectionListButtonFont; ;
			userButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
			userButton.TitleLabel.MinimumFontSize = 10f;
			userButton.TouchUpInside += User_TouchUpInside;

			View.AddSubview(userButton);
		}
		private void BuildDateUI(nfloat initialX, nfloat initialY)
		{
			var startDateLabel = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			startDateLabel.Text = "Start Date";
			startDateLabel.TextAlignment = UITextAlignment.Left;
			startDateLabel.TextColor = GlobalUISettings.LabelTextColor;
			startDateLabel.Font = GlobalUISettings.LabelFont;
			View.AddSubview(startDateLabel);

			startDateTextField = new UITextField(new CGRect(initialLeftMargin,
															startDateLabel.Frame.Y + defaultLabelHeight + defaultVerticalSpacing,
															defaultTextFieldWidth,
															defaultTextFieldHeight));
			startDateTextField.Placeholder = "Select a starting date";
			startDateTextField.AdjustsFontSizeToFitWidth = true;
			startDateTextField.TextAlignment = UITextAlignment.Left;
			startDateTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			startDateTextField.Font = GlobalUISettings.TextFieldFont;
			View.AddSubview(startDateTextField);

			var endDateLabel = new UILabel(new CGRect(initialLeftMargin + defaultLabelWidth * 2, initialY, defaultLabelWidth, defaultLabelHeight));
			endDateLabel.Text = "End Date";
			endDateLabel.TextAlignment = UITextAlignment.Left;
			endDateLabel.TextColor = GlobalUISettings.LabelTextColor;
			endDateLabel.Font = GlobalUISettings.LabelFont;
			View.AddSubview(endDateLabel);

			endDateTextField = new UITextField(new CGRect(initialLeftMargin + defaultLabelWidth * 2,
														  endDateLabel.Frame.Y + defaultLabelHeight + defaultVerticalSpacing,
														  defaultTextFieldWidth,
														  defaultTextFieldHeight));
			endDateTextField.Placeholder = "Select an ending date";
			endDateTextField.AdjustsFontSizeToFitWidth = true;
			endDateTextField.TextAlignment = UITextAlignment.Left;
			endDateTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			endDateTextField.Font = GlobalUISettings.TextFieldFont;
			View.AddSubview(endDateTextField);


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
			startDateTextField.ShouldReturn += TextFieldShouldReturn;
			endDateTextField.ShouldReturn += TextFieldShouldReturn;

			LocationSelectionAlert = new LocationsAlertViewController(View.Bounds.Size.Width, View.Bounds.Height, Locations, "", this);
			UserSelectionAlert = new UsersAlertViewController(View.Bounds.Size.Width, View.Bounds.Height, Users, "", this);

		}


		private void CreatePickerStartDate()
		{
			var modalPicker = new ModalPickerViewController(ModalPickerType.Date, "Select A Date", this)
			{
				HeaderBackgroundColor = GlobalUISettings.BackgroundColor,
				HeaderTextColor = UIColor.Black,
				TransitioningDelegate = new ModalPickerTransitionDelegate(),
				ModalPresentationStyle = UIModalPresentationStyle.Custom
			};

			modalPicker.DatePicker.Mode = UIDatePickerMode.Date;


			modalPicker.OnModalPickerDismissed += (s, a) =>
			{
				startDateTextField.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
			};

			startDateTextField.TouchDown += (sender, e) =>
			{
				PresentViewController(modalPicker, true, null);
			};
		}

		private void CreatePickerFinishDate()
		{
			var modalPicker = new ModalPickerViewController(ModalPickerType.Date, "Select A Date", this)
			{
				HeaderBackgroundColor = GlobalUISettings.BackgroundColor,
				HeaderTextColor = UIColor.Black,
				TransitioningDelegate = new ModalPickerTransitionDelegate(),
				ModalPresentationStyle = UIModalPresentationStyle.Custom
			};

			modalPicker.DatePicker.Mode = UIDatePickerMode.Date;


			modalPicker.OnModalPickerDismissed += (s, a) =>
			{
				endDateTextField.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
			};

			endDateTextField.TouchDown += (sender, e) =>
			{
				PresentViewController(modalPicker, true, null);
			};
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
		private void User_TouchUpInside(object sender, EventArgs e)
		{
			UserSelectionAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
			View.AddSubview(UserSelectionAlert);
		}
		private void RunReport_TouchUpInside(object sender, EventArgs e)
		{
			string locId = null;
			string userId = null;
			var userDisplay = userButton.TitleLabel.Text;
			var locDisplay = locationButton.TitleLabel.Text;
			if (locationButton.TitleLabel.Text != null && !locationButton.TitleLabel.Text.ToLower().Contains("select a") && AzureService.LocationService.Locations != null)
			{
				var location = AzureService.LocationService.Locations.Where(x => x.Name == locationButton.TitleLabel.Text).FirstOrDefault();
				if (location != null)
					locId = location.Id;
			}
			else {
				locDisplay = "All Locations";
			}

			if (userButton.TitleLabel.Text != null && !userButton.TitleLabel.Text.ToLower().Contains("select a"))
			{
				var user = AzureService.UserService.Users.Where(x => x.UserName == userButton.TitleLabel.Text).FirstOrDefault();
				if (user != null)
					userId = user.Id;
			}
			else {
				//userDisplay = "All Users";
				this.ShowMessageAlert("User Required", "Please select user to proceed.");
				return;
			}
			if (string.IsNullOrEmpty(startDateTextField.Text) || string.IsNullOrEmpty(endDateTextField.Text))
			{
				//TIDO: Pass default 
				this.ShowMessageAlert("Dates Required", "Please select start/end date to proceed.");
				return;
			}
			var endDate = DateTime.Parse(endDateTextField.Text);

			if (endDate > DateTime.Now)
				endDateTextField.Text = dateFormatter.ToString(DateTime.Now.ToNSDate());

			var results = AzureService.ReportsService.SalesReport(locId, userId, DateTime.Parse(startDateTextField.Text), DateTime.Parse(endDateTextField.Text));

			SalesReportResultsViewController srrvc = this.Storyboard.InstantiateViewController("SalesReportResultsViewController") as SalesReportResultsViewController;

			srrvc.salesReportModel = results;

			this.NavigationController.PushViewController(srrvc, true);
			//ReportResultsAlert.Frame = new CoreGraphics.CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
			//ReportResultsAlert.AddResults(locDisplay, userDisplay,
			//							  startDateTextField.Text, endDateTextField.Text,
			//                              results.ItemsSold.ToString("N0"), results.TotalSalePrice.ToString("N0"), DateTime.Now.ToShortDateString());
			//View.AddSubview(ReportResultsAlert);
		}
		public override void UpdateLocationCode(string code)
		{
			locationButton.SetTitle(code, UIControlState.Normal);
			locationButton.TitleLabel.TextColor = GlobalUISettings.SelectionListButtonColor;
			locationButton.SetTitleColor(GlobalUISettings.SelectionListButtonColor, UIControlState.Normal);
		}
		public override void UpdateUserCode(string code)
		{
			userButton.SetTitle(code, UIControlState.Normal);
			userButton.TitleLabel.TextColor = GlobalUISettings.SelectionListButtonColor;
			userButton.SetTitleColor(GlobalUISettings.SelectionListButtonColor, UIControlState.Normal);
		}
	}
}
