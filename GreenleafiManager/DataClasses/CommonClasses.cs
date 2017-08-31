using System;
using System.IO;
using System.Text;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using SQLite;


namespace GreenleafiManager
{
    public class GlobalVariables
    {
        public DateTime LastUpdated
        {
            get;
            set;
        }
        [PrimaryKey]
        public int Id
        {
            get;
            set;
        }
    }
    public static class GlobalUISettings
    {
        public static UIFont LabelFont { get { return UIFont.FromName("Helvetica", 25f); } }
        public static UIFont TitleLabelFont { get { return UIFont.FromName("Helvetica", 25f); } }
        public static UIFont TextFieldFont { get { return UIFont.FromName("Helvetica", 35f); } }
        public static UIFont InvoiceLabelFont { get { return UIFont.FromName("Helvetica", 18f); } }
        public static UIFont InvoiceTextFieldFont { get { return UIFont.FromName("Helvetica", 20f); } }
		public static UIFont InventoryTextFieldFont { get { return UIFont.FromName("Helvetica", 30f); } }
		public static UIFont InventorySmallTextFieldFont { get { return UIFont.FromName("Helvetica", 20f); } }
        public static UIFont ButtonFont { get { return UIFont.FromName("Helvetica", 35f); } }
        public static UIFont SelectionListButtonFont { get { return UIFont.FromName("Helvetica", 35f); } }


        public static UIColor LabelTextColor { get { return UIColor.Clear.FromHex(0x007AFF); } }
        public static UIColor InvoiceLabelTextColor { get { return UIColor.Black; } }
        public static UIColor TextFieldFontColor { get { return UIColor.Black; } }
        public static UIColor TextPlaceHolderColor { get { return UIColor.Clear.FromHex(0xCECED2); } }
        public static UIColor ButtonColor { get { return UIColor.Clear.FromHex(0x007AFF); } }
        public static UIColor SelectionListButtonColor { get { return UIColor.Black; } }

        public static UIColor BackgroundColor { get { return UIColor.Clear.FromHex(0xCECED2); } }
    }
    public static class DefaultDetailsLayoutSettings
    {
        public static nfloat InitialTopMargin { get { return 80f; } }
        public static nfloat InitialLeftMargin { get { return 20f; } }
        public static nfloat InitialRightMargin { get { return 20f; } }

        public static nfloat DefaultLabelHeight { get { return 35f; } }
        public static nfloat DefaultLabelWidth { get { return 200f; } }

        public static nfloat DefaultTextFieldHeight { get { return 45f; } }
        public static nfloat DefaultTextFieldWidth { get { return 200f; } }

        public static nfloat InvoiceLabelHeight { get { return 20f; } }
        public static nfloat InvoiceLabelWidth { get { return 100f; } }

        public static nfloat InvoiceTextFieldHeight { get { return 35f; } }
        public static nfloat InvoiceTextFieldWidth { get { return 75f; } }

        public static nfloat DefaultHorizontalSpacing { get { return 20f; } }
        public static nfloat DefaultVerticalSpacing { get { return 2f; } }
        public static nfloat DefaultItemVerticalSpacing { get { return 20f; } }

        public static nfloat InvoiceHorizontalSpacing { get { return 20f; } }
        public static nfloat InvoiceVerticalSpacing { get { return 20f; } }
        public static nfloat InvoiceDeatilVerticalSpacing { get { return 15; } }

    }

    public static class DefaultMenuLayoutSettings
    {

        public static nfloat DefaultLabelHeight { get { return 40f; } }
        public static nfloat DefaultLabelWidth { get { return 220f; } }


        public static nfloat DefaultHorizontalSpacing { get { return 50f; } }
        public static nfloat DefaultVerticalSpacing { get { return 20f/2; } }

    }
    public static class Utils
    {
        public static string Pluralize(string input)
        {
            var words = input.Split(' ');
            var lastWord = words[words.Length - 1];
            string pluralLastWord;

            string lastChar = lastWord.Substring(lastWord.Length - 2);
            if (lastChar == "h")
            {
                lastChar = lastWord.Substring(lastWord.Length - 2);
            }
            switch (lastChar)
            {
                case "y":
                    pluralLastWord = lastWord.Substring(0, lastWord.Length - 1) + "ies";
                    break;
                case "o":
                    pluralLastWord = lastWord.Substring(0, lastWord.Length - 1) + "oes";
                    break;
                case "s":
                case "sh":
                case "ch":
                    pluralLastWord = lastWord + "es";
                    break;
                default:
                    pluralLastWord = lastWord + "s";
                    break;
            }

            string pluralizedStrings = "";
            if (words.Length == 1)
                return pluralLastWord;
            for (int i = 0; i < (words.Length - 1); i++)
            {
                pluralizedStrings = String.Format("{0} {1}", pluralizedStrings, words[i]);
            }
            pluralizedStrings = String.Format("{0} {1}", pluralizedStrings, pluralLastWord);
            return pluralizedStrings;

        }
    }

    public class LoadingOverlay : UIView
    {
        // control declarations
        UIActivityIndicatorView activitySpinner;
        public UILabel loadingLabel;

        public LoadingOverlay(CGRect frame, string Title = "") : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.All;

            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(
                centerX - (labelWidth / 2),
                centerY + 20,
                labelWidth,
                labelHeight
            ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.White;
            loadingLabel.Text = String.IsNullOrEmpty(Title) ? "Loading Data..." : Title;
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(loadingLabel);

        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
            );
        }
    }
    public class UpdatingOverlay : UIView
    {
        // control declarations
        UIActivityIndicatorView activitySpinner;
        UILabel loadingLabel;

        public UpdatingOverlay(CGRect frame) : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.All;

            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(
                centerX - (labelWidth / 2),
                centerY + 20,
                labelWidth,
                labelHeight
            ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.White;
            loadingLabel.Text = "Updating Data...";
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(loadingLabel);

        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
            );
        }
    }
    public class SavingOverlay : UIView
    {
        // control declarations
        UIActivityIndicatorView activitySpinner;
        UILabel loadingLabel;

        public SavingOverlay(CGRect frame) : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.All;

            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(
                centerX - (labelWidth / 2),
                centerY + 20,
                labelWidth,
                labelHeight
            ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.White;
            loadingLabel.Text = "Saving Item...";
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(loadingLabel);

        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
            );
        }
    }
    public static class Camera
    {
        static UIImagePickerController picker;
        static Action<NSDictionary> _callback;

        static void Init()
        {
            if (picker != null)
                return;

            picker = new UIImagePickerController();
            picker.Delegate = new CameraDelegate();
        }

        class CameraDelegate : UIImagePickerControllerDelegate
        {
            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
            {
                var cb = _callback;
                _callback = null;

                picker.DismissModalViewController(true);
                cb(info);
            }
        }

        public static void TakePicture(UIViewController parent, Action<NSDictionary> callback)
        {
            Init();
            picker.SourceType = UIImagePickerControllerSourceType.Camera;
            _callback = callback;
            parent.PresentModalViewController(picker, true);
        }

        public static void SelectPicture(UIViewController parent, Action<NSDictionary> callback)
        {
            Init();
            picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            _callback = callback;
            parent.PresentModalViewController(picker, true);
        }


		public static void GetmagesFromLocal()
		{
			var paths= NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, true);
			var myPath = paths.ElementAt(0);
			NSFileManager fileManager = NSFileManager.DefaultManager;
			// all files in the path
			NSError _nsError;
			var directoryContents = fileManager.GetDirectoryContent(myPath, out _nsError);// [fileManager contentsOfDirectoryAtPath: myPath error: nil];
			var documentDirectoryFilename = Path.Combine(myPath, "21887357134_8048142275.png");

		}
    }

}

