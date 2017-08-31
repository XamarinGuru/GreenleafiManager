using System;
using UIKit;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;

namespace GreenleafiManager
{public class NewCodeAlert : UIView{ 

		public UIButton SaveCodeButton;
		public UIButton CancelCodeButton;

		private nfloat height;
		private List<string> categories; 
		private nfloat width;

		private bool IsCategoryAlertOpen;
		private UIView alert;

		UITextView categoryEntry;// = new UITextView ();
		UITextView descriptionEntry;// = new UITextView ();
		UITextView codeEntry;// = new UITextView ();

		public NewCodeAlert ( nfloat w, nfloat h ) {
			IsCategoryAlertOpen = true;
			height = h;
			width = w;
			Initialize(); 
		}

		public List<string> GetDataToSave(){
		
			return new List<string> {
				categoryEntry.Text,
				descriptionEntry.Text,
				codeEntry.Text
			};
		}
		public NewCodeAlert ( RectangleF bounds ) : base( bounds ) {
			Initialize();
		}

		private void Initialize () {
			//Add background grey color
			 

			alert=new UIView(){BackgroundColor=UIColor.White};
			alert.Frame= new CGRect(width /4, height / 7.5, width/2, height/1.8);
			alert.Layer.CornerRadius = 10f;
			alert.Layer.BorderWidth = 3;
			alert.Layer.BorderColor = new CoreGraphics.CGColor (0,0,0,1);
		

			var titleLabel=new UILabel() {Text = "Add new Item Code"};
			titleLabel.Font = UIFont.FromName ("Helvetica-Bold", 20f);
			titleLabel.Frame= new CGRect(width/8, 10, width/4, 20);

			categoryEntry = new UITextView ();
			descriptionEntry = new UITextView ();
			codeEntry = new UITextView ();

			categoryEntry.Layer.CornerRadius = 10f;
			categoryEntry.Layer.BorderWidth = 2;
			categoryEntry.Layer.BorderColor=new CoreGraphics.CGColor( 0,0,0, 1 );

			descriptionEntry.Layer.CornerRadius = 10f;
			descriptionEntry.Layer.BorderWidth = 2;
			descriptionEntry.Layer.BorderColor=new CoreGraphics.CGColor( 0,0,0, 1 );

			codeEntry.Layer.CornerRadius = 10f;
			codeEntry.Layer.BorderWidth = 2;
			codeEntry.Layer.BorderColor=new CoreGraphics.CGColor( 0,0,0, 1 );

 

			var categoryLabel = new UILabel (){Text="Set Item Code category:", TextColor=UIColor.Black};
			var descriptionLabel = new UILabel (){Text="Set Item Code description:", TextColor=UIColor.Black};
			var codeLabel = new UILabel (){Text="Set Item Code abbreviation:", TextColor=UIColor.Black};



			categoryLabel.Frame=new CGRect(40, 40, width/3, 30);
			categoryEntry.Frame=new CGRect(30, 80, width/3, 30);

			descriptionLabel.Frame=new CGRect(40, 120, width/3, 30);
			descriptionEntry.Frame=new CGRect(30, 160, width/3, 30);

			codeLabel.Frame=new CGRect(40, 200, width/3, 30);
			codeEntry.Frame=new CGRect(30, 240, width/3, 30);



			SaveCodeButton = new UIButton (){BackgroundColor=UIColor.Gray};

			SaveCodeButton.Layer.CornerRadius = 15f;
			SaveCodeButton.Frame=new CGRect(60, 300, width/3, 30);
			SaveCodeButton.SetTitle ("Save Code", UIControlState.Normal);

			CancelCodeButton = new UIButton (){BackgroundColor=UIColor.Gray};
			CancelCodeButton.SetTitle ("Cancel", UIControlState.Normal);
			CancelCodeButton.Layer.CornerRadius = 15f;
			CancelCodeButton.Frame=new CGRect(60, 360, width/3, 30);

			alert.AddSubview( titleLabel );
			alert.AddSubview (categoryEntry);
			alert.AddSubview (descriptionEntry);
			alert.AddSubview (codeEntry);

			alert.AddSubview (categoryLabel);
			alert.AddSubview (descriptionLabel);
			alert.AddSubview (codeLabel);

			alert.AddSubview (SaveCodeButton);
			alert.AddSubview (CancelCodeButton);
 
			AddSubview(alert );
		}
	}
}

