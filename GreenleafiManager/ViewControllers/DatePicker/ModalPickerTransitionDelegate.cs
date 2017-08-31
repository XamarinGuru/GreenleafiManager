using System;
using UIKit;

namespace GreenleafiManager
{
	public class ModalPickerTransitionDelegate : UIViewControllerTransitioningDelegate
	{
		public ModalPickerTransitionDelegate()
		{
		}

		public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForPresentedController(UIViewController presented, UIViewController presenting, UIViewController source)
		{
			var controller = new ModalPickerAnimatedTransitioning();
			controller.IsPresenting = true;

			return controller;
		}

		public override IUIViewControllerAnimatedTransitioning GetAnimationControllerForDismissedController(UIViewController dismissed)
		{
			var controller = new ModalPickerAnimatedDismissed();
			controller.IsPresenting = false;

			return controller;
		}

		public override IUIViewControllerInteractiveTransitioning GetInteractionControllerForPresentation(IUIViewControllerAnimatedTransitioning animator)
		{
			return null;
		}

		public override IUIViewControllerInteractiveTransitioning GetInteractionControllerForDismissal(IUIViewControllerAnimatedTransitioning animator)
		{
			return null;
		}

	}
}

