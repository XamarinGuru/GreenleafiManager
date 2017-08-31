using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using UIKit;

namespace GreenleafiManager
{
	public partial class LoadingViewController : UIViewController
	{

		InventoryDetailViewController idvc;
		public MasterInventoryViewController ParentMasterInventoryViewController { get; set; }
		public bool IsNew
		{
			get;
			set;
		}
		public InventoryNS InventoryItem
		{
			get;
			set;
		}
		public bool AutoPop { get; set; }
		public LoadingViewController() : base("LoadingViewController", null)
		{
		}
		public LoadingViewController(IntPtr handle) : base(handle)
		{
			var bounds = UIScreen.MainScreen.Bounds;
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavigationItem.HidesBackButton = true;

			// Perform any additional setup after loading the view, typically from a nib.
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			NavigationItem.HidesBackButton = true;

			if (!AutoPop)
			{
				idvc = this.Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
                idvc.InventoryItem = InventoryItem;
                idvc.ParentMasterInventoryViewController = ParentMasterInventoryViewController; 
				GoToItemDetails();
				AutoPop = true;
			}
			else
			{
				AutoPop = false;
				NavigationController.PopViewController(true);
			}
		}
		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
		public void GoToItemDetails()
		{
			this.NavigationController.PushViewController(idvc, true);
		}
		
	}
}

