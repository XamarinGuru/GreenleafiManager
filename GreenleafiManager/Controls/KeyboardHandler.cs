using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace GreenleafiManager
{
	public class KeyboardHandler
	{
		private UIView _activeview; // Controller that activated the
									// keyboard
		private nfloat _scrollamount = 0; // amount to scroll 
		private nfloat _scrolledamount = 0; // how much we've
											// scrolled already
		private nfloat _bottom = 0.0f; // bottom point
		private nfloat Offset = 10.0f; // extra offset 
		public UIScrollView View { get; set; } // The UIView for the
											   // keyboard handler
		public void KeyboardUpNotification(NSNotification
			notification)
		{
			// get the keyboard size UIKeyboardFrameEndUserInfoKey
			var val = notification.UserInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue; //NSValue.ValueFromPointer(notification.UserInfo.ValueForKey(UIKeyboard.FrameBeginUserInfoKey).Handle);
			CGSize keyboardFrame = val.RectangleFValue.Size;

			UIEdgeInsets edgInset = new UIEdgeInsets(0.0f, 0.0f, keyboardFrame.Height + 70, 0.0f);
			this.View.ContentInset = edgInset;
			this.View.ScrollIndicatorInsets = edgInset;
		}
		public void KeyboardDownNotification(NSNotification
			notification)
		{
			UIEdgeInsets edgInset = UIEdgeInsets.Zero;//(0.0f, 0.0f, keyboardFrame.Height, 0.0f);
			this.View.ContentInset = edgInset;
			this.View.ScrollIndicatorInsets = edgInset;
		}
	}
	public static class KeyboardViewExtentions
	{
		private static readonly KeyboardHandler kbHandler;
		private static NSObject keyboardUp;
		private static NSObject keyboardDown;
		static KeyboardViewExtentions()
		{
			if (kbHandler == null)
				kbHandler = new KeyboardHandler();
		}
		public static void InitKeyboardScrollView(this UIScrollView view)
		{

			kbHandler.View = view;
			keyboardUp = NSNotificationCenter
				.DefaultCenter
				.AddObserver(UIKeyboard.DidShowNotification,
					kbHandler.KeyboardUpNotification);
			keyboardDown = NSNotificationCenter
				.DefaultCenter
				.AddObserver(UIKeyboard.WillHideNotification,
					kbHandler.KeyboardDownNotification);
		}

		public static void UnsubscribeKeyboardScrollView(this UIView view)
		{
			if (keyboardUp != null && keyboardDown != null)
			{
				NSNotificationCenter
					.DefaultCenter
					.RemoveObserver(keyboardUp);
				NSNotificationCenter
					.DefaultCenter
					.RemoveObserver(keyboardDown);
			}
		}
	}
}
