using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Drawing;
using System.Threading.Tasks;
using Infragistics;
using System.Text;
using SystemConfiguration;
using CoreFoundation;
using System.Net;

namespace GreenleafiManager
{

	public static class UIColorExtensions
	{
		public static UIColor FromHex(this UIColor color, int hexValue)
		{
			return UIColor.FromRGB(
				(((float)((hexValue & 0xFF0000) >> 16)) / 255.0f),
				(((float)((hexValue & 0xFF00) >> 8)) / 255.0f),
				(((float)(hexValue & 0xFF)) / 255.0f)
			);
		}
	}
	public enum NetworkStatus
	{
		NotReachable,
		ReachableViaCarrierDataNetwork,
		ReachableViaWiFiNetwork
	}

	public static class Reachability
	{
		public static string HostName = "www.google.com";

		public static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
		{
			// Is it reachable with the current network configuration?
			bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

			// Do we need a connection to reach it?
			bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0
				|| (flags & NetworkReachabilityFlags.IsWWAN) != 0;

			return isReachable && noConnectionRequired;
		}

		// Is the host reachable with the current network configuration
		public static bool IsHostReachable(string host)
		{
			if (string.IsNullOrEmpty(host))
				return false;

			using (var r = new NetworkReachability(host))
			{
				NetworkReachabilityFlags flags;

				if (r.TryGetFlags(out flags))
					return IsReachableWithoutRequiringConnection(flags);
			}
			return false;
		}

		//
		// Raised every time there is an interesting reachable event,
		// we do not even pass the info as to what changed, and
		// we lump all three status we probe into one
		//
		public static event EventHandler ReachabilityChanged;

		static void OnChange(NetworkReachabilityFlags flags)
		{
			var h = ReachabilityChanged;
			if (h != null)
				h(null, EventArgs.Empty);
		}

		//
		// Returns true if it is possible to reach the AdHoc WiFi network
		// and optionally provides extra network reachability flags as the
		// out parameter
		//
		static NetworkReachability adHocWiFiNetworkReachability;

		public static bool IsAdHocWiFiNetworkAvailable(out NetworkReachabilityFlags flags)
		{
			if (adHocWiFiNetworkReachability == null)
			{
				adHocWiFiNetworkReachability = new NetworkReachability(new IPAddress(new byte[] { 169, 254, 0, 0 }));
				adHocWiFiNetworkReachability.SetNotification(OnChange);
				adHocWiFiNetworkReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
			}

			return adHocWiFiNetworkReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
		}

		static NetworkReachability defaultRouteReachability;

		static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
		{
			if (defaultRouteReachability == null)
			{
				defaultRouteReachability = new NetworkReachability(new IPAddress(0));
				defaultRouteReachability.SetNotification(OnChange);
				defaultRouteReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
			}
			return defaultRouteReachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
		}

		static NetworkReachability remoteHostReachability;

		public static NetworkStatus RemoteHostStatus()
		{
			NetworkReachabilityFlags flags;
			bool reachable;

			if (remoteHostReachability == null)
			{
				remoteHostReachability = new NetworkReachability(HostName);

				// Need to probe before we queue, or we wont get any meaningful values
				// this only happens when you create NetworkReachability from a hostname
				reachable = remoteHostReachability.TryGetFlags(out flags);

				remoteHostReachability.SetNotification(OnChange);
				remoteHostReachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
			}
			else {
				reachable = remoteHostReachability.TryGetFlags(out flags);
			}

			if (!reachable)
				return NetworkStatus.NotReachable;

			if (!IsReachableWithoutRequiringConnection(flags))
				return NetworkStatus.NotReachable;

			return (flags & NetworkReachabilityFlags.IsWWAN) != 0 ?
				NetworkStatus.ReachableViaCarrierDataNetwork : NetworkStatus.ReachableViaWiFiNetwork;
		}

		public static NetworkStatus InternetConnectionStatus()
		{
			NetworkReachabilityFlags flags;
			bool defaultNetworkAvailable = IsNetworkAvailable(out flags);
			if (defaultNetworkAvailable && ((flags & NetworkReachabilityFlags.IsDirect) != 0))
				return NetworkStatus.NotReachable;
			else if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
				return NetworkStatus.ReachableViaCarrierDataNetwork;
			else if (flags == 0)
				return NetworkStatus.NotReachable;
			return NetworkStatus.ReachableViaWiFiNetwork;
		}

		public static NetworkStatus LocalWifiConnectionStatus()
		{
			NetworkReachabilityFlags flags;
			if (IsAdHocWiFiNetworkAvailable(out flags))
				if ((flags & NetworkReachabilityFlags.IsDirect) != 0)
					return NetworkStatus.ReachableViaWiFiNetwork;

			return NetworkStatus.NotReachable;
		}
	}
	public static class Extensions
	{
		public static void ToGreenLeafTextField(this UITextField textField)
		{
			if (textField != null)
			{
				textField.BackgroundColor = UIColor.White;
				textField.LeftView = new UIView(new CGRect(0, 0, 5, 20));
				textField.Layer.BorderWidth = 1;
				textField.Layer.BorderColor = UIColor.Gray.CGColor;
				textField.ClipsToBounds = true;

				textField.ClearButtonMode = UITextFieldViewMode.Always;
				textField.LeftViewMode = UITextFieldViewMode.Always;
			}
		}
		public static void ToInventoryItemTextField(this UITextField textField, bool small = false)
		{
			textField.BackgroundColor = UIColor.White;
			//textField.LeftView = new UIView(new CGRect(0, 0, 5, 20));
			//textField.Layer.BorderWidth = 1;
			//textField.Layer.BorderColor = UIColor.Gray.CGColor;
			textField.ClipsToBounds = true;

			textField.BorderStyle = UITextBorderStyle.None;

			textField.Font = small ? GlobalUISettings.InventorySmallTextFieldFont : GlobalUISettings.InventoryTextFieldFont;

			textField.ClearButtonMode = UITextFieldViewMode.Never;
			textField.LeftViewMode = UITextFieldViewMode.Always;
		}
		public static void ToInventoryLabel(this UILabel label)
		{
			if (label != null)
			{
				label.BackgroundColor = UIColor.White;
				label.Font = GlobalUISettings.InvoiceLabelFont;
				label.TextColor = UIColor.Gray;
				label.ClipsToBounds = true;
			}
		}
		public static void ToNumeric(this UITextField textField)
		{
			if (textField != null)
			{
				textField.KeyboardType = UIKeyboardType.NumberPad;
			}
		}

		public static void ToGreenLeafLabel(this UILabel label)
		{
			if (label != null)
			{
				label.BackgroundColor = UIColor.White;
				//label.LeftView = new UIView(new CGRect(0, 0, 5, 20));
				label.Layer.BorderWidth = 1;
				label.TextAlignment = UITextAlignment.Center;
				label.Layer.BorderColor = UIColor.Gray.CGColor;
				label.ClipsToBounds = true;
			}
		}

		public static void ToGreenLeafButton(this UIButton label)
		{
			if (label != null)
			{
				label.BackgroundColor = UIColor.White;
				//label.LeftView = new UIView(new CGRect(0, 0, 5, 20));
				label.Layer.BorderWidth = 1;
				label.SetTitleColor(UIColor.Black, UIControlState.Normal);
				label.Layer.BorderColor = UIColor.Gray.CGColor;
				label.ClipsToBounds = true;
			}
		}

		public static CGColor CGColorFromHex(int hexValue)
		{
			return new CGColor(
				(((float)((hexValue & 0xFF0000) >> 16)) / 255.0f),
				(((float)((hexValue & 0xFF00) >> 8)) / 255.0f),
				(((float)(hexValue & 0xFF)) / 255.0f)
			);
		}
		public static byte[] ToNSData(this UIImage image)
		{

			if (image == null)
			{
				return null;
			}
			NSData data = null;

			try
			{
				data = image.AsPNG();
				return data.ToArray();
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				if (image != null)
				{
					image.Dispose();
					image = null;
				}
				if (data != null)
				{
					data.Dispose();
					data = null;
				}
			}
		}
		public static string ImageToBase64(this byte[] data)
		{
			// Convert byte[] to Base64 String
			string base64String = Convert.ToBase64String(data);
			return base64String;
		}
		public static UIImage ToImage(this byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			UIImage image = null;
			try
			{

				byte[] byteData = new byte[data.Length * sizeof(byte)];
				Buffer.BlockCopy(data, 0, byteData, 0, byteData.Length);

				image = new UIImage(NSData.FromArray(byteData));
				data = null;
			}
			catch (Exception)
			{
				return null;
			}
			return image;
		}
		public static UIImage RotateImage(this UIImage image)
		{
			UIImage imageToReturn = null;
			if (image.Orientation == UIImageOrientation.Up)
			{
				imageToReturn = image;
			}
			else
			{
				CGAffineTransform transform = CGAffineTransform.MakeIdentity();

				switch (image.Orientation)
				{
					case UIImageOrientation.Down:
					case UIImageOrientation.DownMirrored:
						transform.Rotate((float)Math.PI);
						transform.Translate(image.Size.Width, image.Size.Height);
						break;

					case UIImageOrientation.Left:
					case UIImageOrientation.LeftMirrored:
						transform.Rotate((float)Math.PI / 2);
						transform.Translate(image.Size.Width, 0);
						break;

					case UIImageOrientation.Right:
					case UIImageOrientation.RightMirrored:
						transform.Rotate(-(float)Math.PI / 2);
						transform.Translate(0, image.Size.Height);
						break;
					case UIImageOrientation.Up:
					case UIImageOrientation.UpMirrored:
						break;
				}

				switch (image.Orientation)
				{
					case UIImageOrientation.UpMirrored:
					case UIImageOrientation.DownMirrored:
						transform.Translate(image.Size.Width, 0);
						transform.Scale(-1, 1);
						break;

					case UIImageOrientation.LeftMirrored:
					case UIImageOrientation.RightMirrored:
						transform.Translate(image.Size.Height, 0);
						transform.Scale(-1, 1);
						break;
					case UIImageOrientation.Up:
					case UIImageOrientation.Down:
					case UIImageOrientation.Left:
					case UIImageOrientation.Right:
						break;
				}

				//now draw image
				using (var context = new CGBitmapContext(IntPtr.Zero,
					(int)image.Size.Width,
					(int)image.Size.Height,
					image.CGImage.BitsPerComponent,
					image.CGImage.BytesPerRow,
					image.CGImage.ColorSpace,
					image.CGImage.BitmapInfo))
				{
					context.ConcatCTM(transform);
					switch (image.Orientation)
					{
						case UIImageOrientation.Left:
						case UIImageOrientation.LeftMirrored:
						case UIImageOrientation.Right:
						case UIImageOrientation.RightMirrored:
							// Grr...
							context.DrawImage(new RectangleF(PointF.Empty, new SizeF((float)image.Size.Height, (float)image.Size.Width)), image.CGImage);
							break;
						default:
							context.DrawImage(new RectangleF(PointF.Empty, new SizeF((float)image.Size.Width, (float)image.Size.Height)), image.CGImage);
							break;
					}

					using (var imageRef = context.ToImage())
					{
						imageToReturn = new UIImage(imageRef);
					}
				}
			}

			return imageToReturn;
		}

		public static void ToButtonFormat(this UIButton button, string title)
		{
			if (button != null)
			{
				button.SetTitle(title, UIControlState.Normal);
				button.SetTitleColor(UIColor.Gray, UIControlState.Normal);
				button.Layer.BorderWidth = 1;
				button.Layer.BorderColor = UIColor.Gray.CGColor;
				button.ClipsToBounds = true;
				button.BackgroundColor = UIColor.White;
			}
		}
		public static NSDate ToNSDate(this DateTime date)
		{
			if (date.Kind == DateTimeKind.Unspecified)
				date = DateTime.SpecifyKind(date, DateTimeKind.Local /* DateTimeKind.Local or DateTimeKind.Utc, this depends on each app */);
			return (NSDate)date;
		}
		public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new CGSize(width, height));
			sourceImage.Draw(new CGRect(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		//Alert
		public static void ShowMessageAlert(this UIViewController controller, string title, string message)
		{
			var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
			controller?.PresentViewController(alert, animated: true, completionHandler: null);
		}
	}
}

