using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using ObjCRuntime;
using System.Linq;

namespace GreenleafiManager
{
	public partial class UserDetailViewController : UIViewController
	{
		public bool viewPushed { get; set; }
		//Static layout data
		nfloat initialTopMargin = DefaultDetailsLayoutSettings.InitialTopMargin;
		nfloat initialLeftMargin = DefaultDetailsLayoutSettings.InitialLeftMargin;

		nfloat defaultLabelHeight = DefaultDetailsLayoutSettings.DefaultLabelHeight;
		nfloat defaultLabelWidth = DefaultDetailsLayoutSettings.DefaultLabelWidth;

		nfloat defaultTextFieldHeight = DefaultDetailsLayoutSettings.DefaultTextFieldHeight;
		nfloat defaultTextFieldWidth = DefaultDetailsLayoutSettings.DefaultTextFieldWidth;

		nfloat defaultVerticalSpacing = DefaultDetailsLayoutSettings.DefaultVerticalSpacing;
		nfloat defaultItemVerticalSpacing = DefaultDetailsLayoutSettings.DefaultItemVerticalSpacing;

		//Editable fields
		UITextField firstNameTextField;
		UITextField lastNameTextField;
		UITextField usernameTextField;
		UITextField phoneTextField;
		UITextField emailTextField;
		UIButton roleButton;
		UIScrollView scrollView;

		private RolesAlertViewController Alert;

		public MasterUserViewController MasterViewController { get; set; }
		public User User
		{
			get;
			set;
		}

		public string EditOrCreate
		{
			get;
			set;
		}
		public List<string> Roles
		{
			get
			{
				List<Role> rs = AzureService.UserService.Roles;
				List<string> ret = new List<string>();
				foreach (var r in rs)
				{
					ret.Add(r.Name);
				}
				return ret;
			}
		}
		public UserDetailViewController(IntPtr handle) : base(handle)
		{
			var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveUser);
			saveButton.AccessibilityLabel = "saveButton";
			NavigationItem.RightBarButtonItem = saveButton;

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			scrollView = new UIScrollView(this.View.Frame);
			scrollView.ContentSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

			this.NavigationController.SetNavigationBarHidden(false, false);
			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;
			defaultLabelWidth = (nfloat)this.View.Bounds.Size.Width * 0.75f;
			defaultTextFieldWidth = (nfloat)this.View.Bounds.Size.Width * 0.75f;

			Alert = new RolesAlertViewController(View.Bounds.Size.Width, View.Bounds.Height, Roles, "", this);

			SetupViewUIItems();
			ConfigureView();
			this.View.Add(scrollView);

			var btn = new UIBarButtonItem("< Users", UIBarButtonItemStyle.Plain, (sender, e) =>
				 {
					 if (haveChangedBeenMade())
					 {
						 UIAlertController alert = UIAlertController.Create("Leaving the page?", "If you leave the page before saving, all changes will be lost.", UIAlertControllerStyle.Alert);
						 alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
						 {
						 }));
						 alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
						 {
							 this.NavigationController.PopViewController(true);
						 }));
						 PresentViewController(alert, true, null);
					 }
					 else
					 {
						 this.NavigationController.PopViewController(true);
					 }
				 });
			this.NavigationItem.LeftBarButtonItem = btn;
		}
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			this.scrollView.InitKeyboardScrollView();
		}
		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			this.scrollView.UnsubscribeKeyboardScrollView();
		}

		public override void ViewDidLayoutSubviews()
		{
			Alert.Frame = new CGRect(0, 20, this.View.Bounds.Size.Width, this.View.Bounds.Size.Height);
		}

		private void SetupViewUIItems()
		{
			BuildUserNameUI();
			BuildUserPhoneUI(initialLeftMargin, usernameTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildUserEmailUI(initialLeftMargin, phoneTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildUserRoleUI(initialLeftMargin, emailTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);

			//
			firstNameTextField.InputAccessoryView = new EnhancedToolbar(firstNameTextField, null, lastNameTextField);
			lastNameTextField.InputAccessoryView = new EnhancedToolbar(lastNameTextField, firstNameTextField, phoneTextField);
			phoneTextField.InputAccessoryView = new EnhancedToolbar(phoneTextField, lastNameTextField, emailTextField);
			emailTextField.InputAccessoryView = new EnhancedToolbar(emailTextField, phoneTextField, usernameTextField);
			usernameTextField.InputAccessoryView = new EnhancedToolbar(usernameTextField, emailTextField, null);
		}
		private void BuildUserNameUI()
		{
			var label = new UILabel(new CGRect(initialLeftMargin, initialTopMargin, defaultLabelWidth, defaultLabelHeight));
			label.Text = "Name";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			scrollView.AddSubview(label);

			firstNameTextField = new UITextField(new CGRect(initialLeftMargin, label.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			firstNameTextField.Placeholder = "First Name";
			firstNameTextField.AdjustsFontSizeToFitWidth = true;
			firstNameTextField.TextAlignment = UITextAlignment.Left;
			firstNameTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			firstNameTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(firstNameTextField);

			lastNameTextField = new UITextField(new CGRect(initialLeftMargin, firstNameTextField.Frame.Y + defaultTextFieldHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			lastNameTextField.Placeholder = "Last Name";
			lastNameTextField.AdjustsFontSizeToFitWidth = true;
			lastNameTextField.TextAlignment = UITextAlignment.Left;
			lastNameTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			lastNameTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(lastNameTextField);

			usernameTextField = new UITextField(new CGRect(initialLeftMargin, lastNameTextField.Frame.Y + defaultTextFieldHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			usernameTextField.Placeholder = "Username";
			usernameTextField.AdjustsFontSizeToFitWidth = true;
			usernameTextField.TextAlignment = UITextAlignment.Left;
			usernameTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			usernameTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(usernameTextField);

		}
		private void BuildUserPhoneUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.Text = "Phone";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			scrollView.AddSubview(label);

			phoneTextField = new UITextField(new CGRect(initialX, label.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			phoneTextField.Placeholder = "Phone";
			phoneTextField.AdjustsFontSizeToFitWidth = true;
			phoneTextField.TextAlignment = UITextAlignment.Left;
			phoneTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			phoneTextField.Font = GlobalUISettings.TextFieldFont;
			phoneTextField.ToNumeric();
			scrollView.AddSubview(phoneTextField);
		}

		private void BuildUserEmailUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.MinimumFontSize = 12f;
			label.Text = "Email";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			scrollView.AddSubview(label);

			emailTextField = new UITextField(new CGRect(initialX, label.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			emailTextField.Placeholder = "Email";
			emailTextField.AdjustsFontSizeToFitWidth = true;
			emailTextField.TextAlignment = UITextAlignment.Left;
			emailTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			emailTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(emailTextField);
		}

		private void BuildUserRoleUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.MinimumFontSize = 12f;
			label.Text = "Role";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			scrollView.AddSubview(label);

			roleButton = UIButton.FromType(UIButtonType.System);
			roleButton.Frame = new CGRect(initialX, label.Frame.Y + defaultLabelHeight, 200f, defaultLabelHeight);
			var roleText = String.IsNullOrWhiteSpace(User.RoleId) ? "Select Role" : User.RoleName;
			roleButton.SetTitle(roleText, UIControlState.Normal);

			if (String.IsNullOrWhiteSpace(User.RoleId))
			{
				roleButton.TitleLabel.TextColor = UIColor.LightGray;
				roleButton.SetTitleColor(GlobalUISettings.TextPlaceHolderColor, UIControlState.Normal);
			}
			else {
				roleButton.TitleLabel.TextColor = GlobalUISettings.SelectionListButtonColor;
				roleButton.SetTitleColor(GlobalUISettings.SelectionListButtonColor, UIControlState.Normal);
			}

			roleButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			roleButton.TitleLabel.TextAlignment = UITextAlignment.Left;
			roleButton.TitleLabel.Font = GlobalUISettings.SelectionListButtonFont; ;
			roleButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
			roleButton.TitleLabel.MinimumFontSize = 10f;
			roleButton.TouchUpInside += Role_TouchUpInside;

			scrollView.AddSubview(roleButton);
		}


		private void ConfigureView()
		{
			firstNameTextField.ShouldReturn += TextFieldShouldReturn;
			lastNameTextField.ShouldReturn += TextFieldShouldReturn;
			usernameTextField.ShouldReturn += TextFieldShouldReturn;
			emailTextField.ShouldReturn += TextFieldShouldReturn;
			phoneTextField.ShouldReturn += TextFieldShouldReturn;

			Alert = new RolesAlertViewController(View.Bounds.Size.Width, View.Bounds.Height, Roles, "", this);

			SetupFields();
		}

		public void SetupFields()
		{
			firstNameTextField.Text = User.FirstName;
			lastNameTextField.Text = User.LastName;
			emailTextField.Text = User.Email;
			phoneTextField.Text = User.Phone;

			usernameTextField.Text = User.UserName;
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


		async void SaveUser(object sender, EventArgs args)
		{
			//var user = new User();
			//user.Id = User.Id;
			User.FirstName = firstNameTextField.Text;
			User.LastName = lastNameTextField.Text;
			User.UserName = usernameTextField.Text;
			User.Phone = phoneTextField.Text;
			User.Email = emailTextField.Text;
			User.RoleId = AzureService.UserService.Roles.Where(x => x.Name == roleButton.TitleLabel.Text).First().Id;

			if (!ValidateBeforeSavingOrCreating(User))
				return;

			this.NavigationItem.HidesBackButton = true;
			this.NavigationItem.LeftBarButtonItem = null;
			this.NavigationItem.RightBarButtonItem = null;

			await AzureService.UserService.InsertOrSave(User);

			var alert = UIAlertController.Create("User", "User " + EditOrCreate + " successfuly", UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) =>
			{
				MasterViewController.SelectedItem = User;
				this.viewPushed = true;
				this.NavigationController.PopViewController(true);
			}));
			PresentViewController(alert, animated: true, completionHandler: null);

		}

		private bool ValidateBeforeSavingOrCreating(User obj)
		{

			if (string.IsNullOrEmpty(obj.UserName))
			{
				var alert = UIAlertController.Create("User", "Username is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}

			if (string.IsNullOrEmpty(obj.RoleId))
			{
				var alert = UIAlertController.Create("User", "Role is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			return true;

		}

		public void UpdateCode(string code)
		{
			roleButton.SetTitle(code, UIControlState.Normal);
			roleButton.TitleLabel.TextColor = GlobalUISettings.SelectionListButtonColor;
			roleButton.SetTitleColor(GlobalUISettings.SelectionListButtonColor, UIControlState.Normal);
		}
		private void Role_TouchUpInside(object sender, EventArgs e)
		{
			View.AddSubview(Alert);
		}

		private bool haveChangedBeenMade()
		{
			if (User.FirstName != firstNameTextField.Text ||
				User.LastName != lastNameTextField.Text ||
				User.UserName != usernameTextField.Text ||
				User.Phone != phoneTextField.Text ||
				User.Email != emailTextField.Text ||
				User.RoleId != AzureService.UserService.Roles.Where(x => x.Name == roleButton.TitleLabel.Text).First().Id)
			{
				return true;
			}
			return false;
		}
	}
}
