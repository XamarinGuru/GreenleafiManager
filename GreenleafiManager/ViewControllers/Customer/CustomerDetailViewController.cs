using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;

namespace GreenleafiManager
{
	partial class CustomerDetailViewController : UIViewController
	{
		//Static layout data
		private UITapGestureRecognizer tap;
		private bool registeredScroll = false;

		public static LoadingOverlay loadingOverlay;

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
		UITextField firstNameTextField;
		UITextField lastNameTextField;
		UITextField phoneTextField;
		UITextField emailTextField;
		UITextField streetTextField;
		UITextField cityTextField;
		UITextField provinceTextField;
		UITextField zipTextField;
		UITextField countryTextField;

		private readonly KeyboardHandler kbHandler;
		private NSObject keyboardUp;
		private NSObject keyboardDown;
		UIScrollView scrollView;

		public MasterCustomerViewController MasterViewController
		{
			get;
			set;
		}
		public Customer Customer
		{
			get;
			set;
		}

		public string EditOrCreate
		{
			get;
			set;
		}

		public CustomerDetailViewController(IntPtr handle) : base(handle)
		{
			var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveCustomer);
			saveButton.AccessibilityLabel = "saveButton";
			NavigationItem.RightBarButtonItem = saveButton;
			//if (kbHandler == null)
			//	kbHandler = new KeyboardHandler();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			scrollView = new UIScrollView(this.View.Frame);
			scrollView.ContentSize = new CGSize(this.View.Frame.Width, this.View.Frame.Height);

			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds, "processing...");

			this.NavigationController.SetNavigationBarHidden(false, false);
			View.MultipleTouchEnabled = true;
			View.UserInteractionEnabled = true;
			defaultLabelWidth = (nfloat)this.View.Bounds.Size.Width * 0.75f;
			defaultTextFieldWidth = (nfloat)this.View.Bounds.Size.Width * 0.75f;

			SetupViewUIItems();
			ConfigureView();

			this.View.Add(scrollView);
			var btn = new UIBarButtonItem("< Customers", UIBarButtonItemStyle.Plain, (sender, e) =>
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
					 else {
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
		public void ShowLoading()
		{
			View.Add(loadingOverlay);
		}
		private void SetupViewUIItems()
		{
			BuildCustomerNameUI();
			BuildCustomerPhoneUI(initialLeftMargin, lastNameTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildCustomerEmailUI(initialLeftMargin, phoneTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildCustomerAddressUI(initialLeftMargin, emailTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);

			firstNameTextField.InputAccessoryView = new EnhancedToolbar(firstNameTextField, null, lastNameTextField);
			lastNameTextField.InputAccessoryView = new EnhancedToolbar(lastNameTextField, firstNameTextField, phoneTextField);
			phoneTextField.InputAccessoryView = new EnhancedToolbar(phoneTextField, lastNameTextField, emailTextField);
			emailTextField.InputAccessoryView = new EnhancedToolbar(emailTextField, phoneTextField, streetTextField);
			streetTextField.InputAccessoryView = new EnhancedToolbar(streetTextField, emailTextField, cityTextField);
			cityTextField.InputAccessoryView = new EnhancedToolbar(cityTextField, streetTextField, provinceTextField);
			provinceTextField.InputAccessoryView = new EnhancedToolbar(provinceTextField, cityTextField, zipTextField);
			zipTextField.InputAccessoryView = new EnhancedToolbar(zipTextField, cityTextField, countryTextField);
			countryTextField.InputAccessoryView = new EnhancedToolbar(countryTextField, zipTextField, null);
		}
		private void BuildCustomerNameUI()
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

		}
		private void BuildCustomerPhoneUI(nfloat initialX, nfloat initialY)
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

		private void BuildCustomerEmailUI(nfloat initialX, nfloat initialY)
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

		private void BuildCustomerAddressUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.Text = "Address";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			scrollView.AddSubview(label);

			streetTextField = new UITextField(new CGRect(initialX, label.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			streetTextField.Placeholder = "Street Address";
			streetTextField.AdjustsFontSizeToFitWidth = true;
			streetTextField.TextAlignment = UITextAlignment.Left;
			streetTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			streetTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(streetTextField);

			cityTextField = new UITextField(new CGRect(initialX, streetTextField.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			cityTextField.Placeholder = "City";
			cityTextField.AdjustsFontSizeToFitWidth = true;
			cityTextField.TextAlignment = UITextAlignment.Left;
			cityTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			cityTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(cityTextField);

			provinceTextField = new UITextField(new CGRect(initialX, cityTextField.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			provinceTextField.Placeholder = "State";
			provinceTextField.AdjustsFontSizeToFitWidth = true;
			provinceTextField.TextAlignment = UITextAlignment.Left;
			provinceTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			provinceTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(provinceTextField);

			zipTextField = new UITextField(new CGRect(initialX, provinceTextField.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			zipTextField.Placeholder = "Zip";
			zipTextField.AdjustsFontSizeToFitWidth = true;
			zipTextField.TextAlignment = UITextAlignment.Left;
			zipTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			zipTextField.Font = GlobalUISettings.TextFieldFont;
			zipTextField.ToNumeric();
			scrollView.AddSubview(zipTextField);

			countryTextField = new UITextField(new CGRect(initialX, zipTextField.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			countryTextField.Placeholder = "Country";
			countryTextField.AdjustsFontSizeToFitWidth = true;
			countryTextField.TextAlignment = UITextAlignment.Left;
			countryTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			countryTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(countryTextField);

		}
		private void ConfigureView()
		{
			firstNameTextField.ShouldReturn += TextFieldShouldReturn;
			lastNameTextField.ShouldReturn += TextFieldShouldReturn;
			streetTextField.ShouldReturn += TextFieldShouldReturn;
			cityTextField.ShouldReturn += TextFieldShouldReturn;
			provinceTextField.ShouldReturn += TextFieldShouldReturn;
			zipTextField.ShouldReturn += TextFieldShouldReturn;
			countryTextField.ShouldReturn += TextFieldShouldReturn;
			emailTextField.ShouldReturn += TextFieldShouldReturn;
			phoneTextField.ShouldReturn += TextFieldShouldReturn;
			SetupFields();
		}

		public void SetupFields()
		{
			firstNameTextField.Text = Customer.FirstName;
			lastNameTextField.Text = Customer.LastName;
			emailTextField.Text = Customer.Email;
			phoneTextField.Text = Customer.Phone;

			streetTextField.Text = Customer.Address;
			cityTextField.Text = Customer.City;
			provinceTextField.Text = Customer.Province;
			zipTextField.Text = Customer.Zip;
			countryTextField.Text = Customer.Country;
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


		async void SaveCustomer(object sender, EventArgs args)
		{

			ShowLoading();
			var customer = new Customer();
			customer.Id = Customer.Id;
			customer.FirstName = firstNameTextField.Text;
			customer.LastName = lastNameTextField.Text;
			customer.Address = streetTextField.Text;
			customer.City = cityTextField.Text;
			customer.Province = provinceTextField.Text;
			customer.Zip = zipTextField.Text;
			customer.Country = countryTextField.Text;
			customer.Phone = phoneTextField.Text;
			customer.Email = emailTextField.Text;
			if (!ValidateBeforeSavingOrCreating(customer))
			{
				loadingOverlay.Hide();
				return;
			}

			this.NavigationItem.HidesBackButton = true;
			this.NavigationItem.LeftBarButtonItem = null;
			this.NavigationItem.RightBarButtonItem = null;

			await AzureService.CustomerService.InsertOrSave(customer);

			var alert = UIAlertController.Create("Customer", "Customer " + EditOrCreate + " successfuly", UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) =>
			{
				if (MasterViewController != null)
					MasterViewController.SelectedItem = customer;
				this.NavigationController.PopViewController(true);
			}));
			PresentViewController(alert, animated: true, completionHandler: null);
			loadingOverlay.Hide();
		}

		private bool ValidateBeforeSavingOrCreating(Customer obj)
		{
			if (string.IsNullOrEmpty(obj.FirstName))
			{
				var alert = UIAlertController.Create("Customer", "First name is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}

			if (string.IsNullOrEmpty(obj.LastName))
			{
				var alert = UIAlertController.Create("Customer", "Last name is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}

			if (string.IsNullOrEmpty(obj.Address))
			{
				var alert = UIAlertController.Create("Customer", "Address is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			if (string.IsNullOrEmpty(obj.Country))
			{
				var alert = UIAlertController.Create("Customer", "Country is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			if (string.IsNullOrEmpty(obj.Zip))
			{
				var alert = UIAlertController.Create("Customer", "Zip is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			if (string.IsNullOrEmpty(obj.City))
			{
				var alert = UIAlertController.Create("Customer", "City is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			return true;

		}

		private bool haveChangedBeenMade()
		{
			if (firstNameTextField.Text != Customer.FirstName ||
				lastNameTextField.Text != Customer.LastName ||
				emailTextField.Text != Customer.Email ||
				phoneTextField.Text != Customer.Phone ||

			   	streetTextField.Text != Customer.Address ||
				cityTextField.Text != Customer.City ||
				provinceTextField.Text != Customer.Province ||
				zipTextField.Text != Customer.Zip ||
				countryTextField.Text != Customer.Country)
			{
				return true;
			}
			return false;
		}
	}
}
