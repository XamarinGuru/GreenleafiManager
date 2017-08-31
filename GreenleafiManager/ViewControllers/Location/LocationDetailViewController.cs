using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;

namespace GreenleafiManager
{
	partial class LocationDetailViewController : UIViewController
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
		UITextField taxTextField;
		UITextField nameTextField;
		UITextField provinceTextField;
		UITextField cityTextField;
		UITextField phoneTextField;
		UITextField countryTextField;
		UITextField address1TextField;
		UITextField address2TextField;
		UITextField emailTextField;
		UITextField zipTextField;
		UIScrollView scrollView;

		public MasterLocationViewController MasterViewController { get; set; }

		public Location Location
		{
			get;
			set;
		}

		public string EditOrCreate
		{
			get;
			set;
		}
		public LocationDetailViewController(IntPtr handle) : base(handle)
		{
			var saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveLocation);
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

			SetupViewUIItems();
			ConfigureView();
			this.View.Add(scrollView);
			var btn = new UIBarButtonItem("< Locations", UIBarButtonItemStyle.Plain, (sender, e) =>
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
		private void ConfigureView()
		{
			taxTextField.ShouldReturn += TextFieldShouldReturn;
			nameTextField.ShouldReturn += TextFieldShouldReturn;
			provinceTextField.ShouldReturn += TextFieldShouldReturn;
			cityTextField.ShouldReturn += TextFieldShouldReturn;
			phoneTextField.ShouldReturn += TextFieldShouldReturn;
			countryTextField.ShouldReturn += TextFieldShouldReturn;
			address1TextField.ShouldReturn += TextFieldShouldReturn;
			address2TextField.ShouldReturn += TextFieldShouldReturn;
			zipTextField.ShouldReturn += TextFieldShouldReturn;
			provinceTextField.ShouldReturn += TextFieldShouldReturn;

			SetupFields();
		}

		private void SetupViewUIItems()
		{
			BuildCustomerNameUI();
			BuildTaxUI(initialLeftMargin, nameTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildCustomerPhoneUI(initialLeftMargin, taxTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);
			BuildCustomerAddressUI(initialLeftMargin, phoneTextField.Frame.Y + defaultTextFieldHeight + defaultItemVerticalSpacing);

			nameTextField.InputAccessoryView = new EnhancedToolbar(nameTextField, null, taxTextField);
			taxTextField.InputAccessoryView = new EnhancedToolbar(taxTextField, nameTextField, phoneTextField);
			phoneTextField.InputAccessoryView = new EnhancedToolbar(phoneTextField, taxTextField, address1TextField);
			address1TextField.InputAccessoryView = new EnhancedToolbar(address1TextField, phoneTextField, address2TextField);
			address2TextField.InputAccessoryView = new EnhancedToolbar(address2TextField, address1TextField, cityTextField);
			cityTextField.InputAccessoryView = new EnhancedToolbar(cityTextField, address1TextField, zipTextField);
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

			nameTextField = new UITextField(new CGRect(initialLeftMargin, label.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			nameTextField.Placeholder = "Name";
			nameTextField.AdjustsFontSizeToFitWidth = true;
			nameTextField.TextAlignment = UITextAlignment.Left;
			nameTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			nameTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(nameTextField);

		}
		private void BuildTaxUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.Text = "Location Tax";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			scrollView.AddSubview(label);

			taxTextField = new UITextField(new CGRect(initialX, label.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			taxTextField.Placeholder = "Tax";
			taxTextField.AdjustsFontSizeToFitWidth = true;
			taxTextField.TextAlignment = UITextAlignment.Left;
			taxTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			taxTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(taxTextField);

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
			scrollView.AddSubview(phoneTextField);
		}
		private void BuildCustomerAddressUI(nfloat initialX, nfloat initialY)
		{
			var label = new UILabel(new CGRect(initialX, initialY, defaultLabelWidth, defaultLabelHeight));
			label.Text = "Address";
			label.TextAlignment = UITextAlignment.Left;
			label.TextColor = GlobalUISettings.LabelTextColor;
			label.Font = GlobalUISettings.LabelFont;
			scrollView.AddSubview(label);

			address1TextField = new UITextField(new CGRect(initialX, label.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			address1TextField.Placeholder = "Street Address";
			address1TextField.AdjustsFontSizeToFitWidth = true;
			address1TextField.TextAlignment = UITextAlignment.Left;
			address1TextField.TextColor = GlobalUISettings.TextFieldFontColor;
			address1TextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(address1TextField);

			address2TextField = new UITextField(new CGRect(initialX, address1TextField.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			address2TextField.Placeholder = "Street Address";
			address2TextField.AdjustsFontSizeToFitWidth = true;
			address2TextField.TextAlignment = UITextAlignment.Left;
			address2TextField.TextColor = GlobalUISettings.TextFieldFontColor;
			address2TextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(address2TextField);

			cityTextField = new UITextField(new CGRect(initialX, address2TextField.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			cityTextField.Placeholder = "City";
			cityTextField.AdjustsFontSizeToFitWidth = true;
			cityTextField.TextAlignment = UITextAlignment.Left;
			cityTextField.TextColor = GlobalUISettings.TextFieldFontColor;
			cityTextField.Font = GlobalUISettings.TextFieldFont;
			scrollView.AddSubview(cityTextField);

			provinceTextField = new UITextField(new CGRect(initialX, cityTextField.Frame.Y + defaultLabelHeight + defaultVerticalSpacing, defaultTextFieldWidth, defaultTextFieldHeight));
			provinceTextField.Placeholder = "City";
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

		public void SetupFields()
		{
			nameTextField.Text = Location.Name;
			phoneTextField.Text = Location.Phone;
			if (!string.IsNullOrEmpty(Location.Tax))
				taxTextField.Text = Location.Tax;
			else
				taxTextField.Text = string.Empty;

			address1TextField.Text = Location.Address1;
			address2TextField.Text = Location.Address2;
			cityTextField.Text = Location.City;
			provinceTextField.Text = Location.Province;
			zipTextField.Text = Location.Zip;
			countryTextField.Text = Location.Country;
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

		async void SaveLocation(object sender, EventArgs args)
		{
			var location = new Location();
			location.Id = Location.Id;
			location.Tax = taxTextField.Text;
			location.Name = nameTextField.Text;
			location.Address1 = address1TextField.Text;
			location.Address2 = address2TextField.Text;
			location.City = cityTextField.Text;
			location.Zip = zipTextField.Text;
			location.Country = countryTextField.Text;
			location.Phone = phoneTextField.Text;
			location.Province = provinceTextField.Text;
			if (!ValidateBeforeSavingOrCreating(location))
				return;
			
			await AzureService.LocationService.InsertOrSave(location);

			this.NavigationItem.HidesBackButton = true;
			this.NavigationItem.LeftBarButtonItem = null;
			this.NavigationItem.RightBarButtonItem = null;

			var alert = UIAlertController.Create("Location", "Location " + EditOrCreate + " successfuly", UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) =>
			{
				MasterViewController.SelectedItem = location;
				this.NavigationController.PopViewController(true);
			}));
			PresentViewController(alert, animated: true, completionHandler: null);

		}
		private bool ValidateBeforeSavingOrCreating(Location obj)
		{

			if (string.IsNullOrEmpty(obj.Name))
			{
				var alert = UIAlertController.Create("Location", "Name is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}

			if (string.IsNullOrEmpty(obj.Address1))
			{
				var alert = UIAlertController.Create("Location", "Address is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			if (string.IsNullOrEmpty(obj.Country))
			{
				var alert = UIAlertController.Create("Location", "Country is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			if (string.IsNullOrEmpty(obj.Zip))
			{
				var alert = UIAlertController.Create("Location", "Zip is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			if (string.IsNullOrEmpty(obj.City))
			{
				var alert = UIAlertController.Create("Location", "City is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}

			if (!string.IsNullOrEmpty(obj.Tax))
			{
				decimal value;
				if (Decimal.TryParse(obj.Tax, out value))
				{
					if (Decimal.Parse(obj.Tax) >= 1)
					{
						var alert = UIAlertController.Create("Location", "Tax must be a value below 1", UIAlertControllerStyle.Alert);
						alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
						PresentViewController(alert, animated: true, completionHandler: null);
						return false;
					}
					if (Decimal.Parse(obj.Tax) <= -1)
					{
						var alert = UIAlertController.Create("Location", "Tax must be a positive number", UIAlertControllerStyle.Alert);
						alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
						PresentViewController(alert, animated: true, completionHandler: null);
						return false;
					}
				}
				else {
					var alert = UIAlertController.Create("Location", "Tax must be a decimal value", UIAlertControllerStyle.Alert);
					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
					PresentViewController(alert, animated: true, completionHandler: null);
					return false;
				}
			}
			else {
				var alert = UIAlertController.Create("Location", "Tax is required", UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (actionOK) => { return; }));
				PresentViewController(alert, animated: true, completionHandler: null);
				return false;
			}
			return true;

		}
		private bool haveChangedBeenMade()
		{
			if (Location.Id != Location.Id ||
			Location.Tax != taxTextField.Text ||
			Location.Name != nameTextField.Text ||
			Location.Address1 != address1TextField.Text ||
			Location.Address2 != address2TextField.Text ||
			Location.City != cityTextField.Text ||
			Location.Zip != zipTextField.Text ||
			Location.Country != countryTextField.Text ||
			Location.Phone != phoneTextField.Text ||
			Location.Province != provinceTextField.Text)
			{
				return true;
			}
			return false;
		}
	}
}
