using System;

using UIKit;

namespace GreenleafiManagerPhone
{
	public partial class SavingImagesViewController : UIViewController
	{
		public UIImage Image { get; set; }
		public InventoryNS InventoryItem { get; set; }

		public SavingImagesViewController() : base("SavingImagesViewController", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavigationItem.HidesBackButton = true;
			// Perform any additional setup after loading the view, typically from a nib.
		}
		public SavingImagesViewController(IntPtr handle) : base(handle)
		{
			var bounds = UIScreen.MainScreen.Bounds;
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			this.NavigationItem.HidesBackButton = true;

			// get the original image
			UIImage originalImage = Image;
			if (originalImage != null)
			{
				// do something with the image
				GridImage image = new GridImage();
				image.Image = originalImage;
				image.SortOrder = (InventoryItem.GridImages.Count + 1).ToString();
				image.OriginalId = InventoryItem.ShopifyId;
				var item = InventoryItem.ConvertToInventory();
				item.LastCheckedDate = DateTime.UtcNow;
				InventoryItem.LastCheckedDate = DateTime.Now.ToString("d");
				AzureService.InventoryService.InsertOrSave(item);

				var picutureName = AzureService.InventoryService.AddImageToAzure(image);
				if (!String.IsNullOrEmpty(picutureName))
				{
					image.PictureName = picutureName;
					InventoryItem.GridImages.Add(image);
				}
			}
			NavigationController.PopViewController(true);

		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

