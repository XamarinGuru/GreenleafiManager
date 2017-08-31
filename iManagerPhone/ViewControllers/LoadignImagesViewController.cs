using System;

using UIKit;

namespace GreenleafiManagerPhone
{
	public partial class LoadignImagesViewController : UIViewController
	{
		public bool AutoPop { get; set; }
		PhotoViewController pvc;

		public InventoryNS InventoryItem
		{
			get;
			set;
		}

		public LoadignImagesViewController() : base("LoadignImagesViewController", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavigationItem.HidesBackButton = true;
			// Perform any additional setup after loading the view, typically from a nib.
		}
		public LoadignImagesViewController(IntPtr handle) : base(handle)
		{
			var bounds = UIScreen.MainScreen.Bounds;
		}
		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (!AutoPop)
			{
				pvc = this.Storyboard.InstantiateViewController("PhotoViewController") as PhotoViewController;
				LoadImages();
				GoToImages();
				AutoPop = true;
			}
			else
			{
				AutoPop = false;
				NavigationController.PopViewController(true);
			}
		}
		public void GoToImages()
		{
			this.NavigationController.PushViewController(pvc, true);
		}
		private void LoadImages()
		{
			try
			{
				var images = InventoryItem.GridImages;
				pvc.InventoryItem = InventoryItem;

			}
			catch (Exception ex)
			{
				//RMR TODO Umm this is bad?
			}
			finally
			{
				//HideLoading();
			}
		}
	}
}

