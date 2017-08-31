using System;
using System.Collections.Generic;
using System.Drawing;
using UIKit;

namespace GreenleafiManager
{
	public static class KeyboardHelper
	{
		public static float ScrollPoint;
		public static void ScrollControlIntoView(UIScrollView scrollViewer, UIControl field)
		{
			PointF nextPoint = new PointF((float)scrollViewer.Frame.X, (float)field.Frame.Top - ScrollPoint);
			scrollViewer.SetContentOffset(nextPoint, true);
		}

		public static void ScrollIntoView(UIScrollView scrollViewer, CoreGraphics.CGRect frame)
		{
			PointF nextPoint = new PointF(0, (float)frame.Top + 80);
			scrollViewer.SetContentOffset(nextPoint, true);
		}

		public static void ScrollIntoViewPartial(UIScrollView scrollViewer, CoreGraphics.CGRect frame)
		{
			PointF nextPoint = new PointF(0, (float)frame.Top - ScrollPoint);
			scrollViewer.SetContentOffset(nextPoint, true);
		}

		public static void InitialiseFormFields(UIScrollView scrollViewer, List<UIControl> formFields)
		{
			System.Diagnostics.Debug.Assert(scrollViewer != null);
			System.Diagnostics.Debug.Assert(formFields != null);

			if (formFields.Count > 1)
			{
				for (int index = 0; index < formFields.Count - 1; index++)
				{
					UIControl field = formFields[index];
					UITextField textField = field as UITextField;

					UIControl nextField = formFields[index + 1];
					field.EditingDidBegin += delegate (object sender, EventArgs e)
					{
						ScrollControlIntoView(scrollViewer, field);
					};

					if (textField != null)
					{
						textField.ShouldReturn = delegate
						{
							field.ResignFirstResponder();

							if (nextField is IUITextInputTraits)
								nextField.BecomeFirstResponder();

							KeyboardHelper.ScrollControlIntoView(scrollViewer, nextField);
							return true;
						};
					}
				}
			}
			UIControl lastField = formFields[formFields.Count - 1];
			UITextField lastTextField = lastField as UITextField;
			lastField.EditingDidBegin += delegate (object sender, EventArgs e)
			{
				ScrollControlIntoView(scrollViewer, lastField);
			};

			if (lastTextField != null)
			{
				lastTextField.ShouldReturn = delegate
				{
					lastField.ResignFirstResponder();

					KeyboardHelper.ScrollControlIntoView(scrollViewer, formFields[0]);
					return true;
				};
			}
		}
	}
}