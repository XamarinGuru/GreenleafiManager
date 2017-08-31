using System;
using CoreGraphics;
using UIKit;

namespace GreenleafiManager
{
	public class EnhancedToolbar : UIToolbar
	{
		public UIView prevTextFieldOrView { get; set; }
		public UIView currentTextFieldOrView { get; set; }
		public UIView nextTextFieldOrView { get; set; }

		public EnhancedToolbar() : base() { }

		public EnhancedToolbar(UIView current, UIView previous,
			UIView next)
		{
			this.currentTextFieldOrView = current;
			this.prevTextFieldOrView = previous;
			this.nextTextFieldOrView = next;
			SetupToolbar();
		}

		void SetupToolbar()
		{
			//this.currentTextFieldOrView = current;
			//this.prevTextFieldOrView = previous;
			//this.nextTextFieldOrView = next;

			//
			Frame = new CGRect(0.0f, 0.0f, 320, 44.0f);
			TintColor = UIColor.DarkGray;
			Translucent = false;
			Items = new UIBarButtonItem[]
			{
			new UIBarButtonItem("Prev",
				UIBarButtonItemStyle.Bordered, delegate
				{
					prevTextFieldOrView.BecomeFirstResponder();
				}) { Enabled = prevTextFieldOrView != null },
			new UIBarButtonItem("Next",
				UIBarButtonItemStyle.Bordered, delegate
				{
					nextTextFieldOrView.BecomeFirstResponder();
				}) { Enabled = nextTextFieldOrView != null },
		new
		   UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
		new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
		{
			currentTextFieldOrView.ResignFirstResponder();
		})
			};
		}
	}
}
