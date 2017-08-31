using Foundation;
using System;
using System.Linq;
using System.CodeDom.Compiler;
using UIKit;
using Infragistics;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;
using Syncfusion.SfDataGrid;
using CoreGraphics;
using System.Windows.Input;
using System.Collections;

namespace GreenleafiManagerPhone
{
	public partial class PhotoViewController : UIViewController
	{
		//UIKit.UIButton importImage;
		//UIKit.UIButton takePhoto;
		SfDataGrid dataGrid;
		public GridImage SwipedItem { get; set; }
		//private UIView _slideView;
		private UIButton _NewImageButton;
		private UIButton _UploadImageButton;
		UIImagePickerController imagePicker;

		public static LoadingOverlay loadingOverlay;

		public InventoryNS InventoryItem
		{
			get;
			set;
		}

		public PhotoViewController() : base("PhotoViewController", null)
		{
			if (InventoryItem == null)
				throw new Exception("Item Required for images");
		}
		public PhotoViewController(IntPtr handle) : base(handle)
		{
			var bounds = UIScreen.MainScreen.Bounds;

			loadingOverlay = new LoadingOverlay(bounds);
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//_slideView = new UIView(new CoreGraphics.CGRect(0, 0, 100, 100));
		
			var btn2 = new UIBarButtonItem("  Back", UIBarButtonItemStyle.Plain, (sender, e) => { this.NavigationController.PopViewController(true); });
			this.NavigationItem.LeftBarButtonItem = btn2;
			CreateGrid();

		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			dataGrid.IsBusy = true;
			dataGrid.ItemsSource = null;
			dataGrid.ItemsSource = InventoryItem.GridImages;
			dataGrid.IsBusy = false;
		}
		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private void CreateGrid()
		{
			ShowLoading();

			//CreateAddNewButton();
			dataGrid = new SfDataGrid();
			dataGrid.ItemsSource = InventoryItem.GridImages;// AzureService.InventoryService.ad.Customers;
			dataGrid.AllowSorting = true;
			dataGrid.SelectionMode = SelectionMode.Single;
			//			dataGrid.SelectionChanged += DataGrid_SelectionChanged;


			_NewImageButton = new UIButton(new CGRect(0, 60, View.Frame.Width / 2, 55));
			_NewImageButton.ToGreenLeafButton();
			_NewImageButton.SetTitle("Take New Photo", UIControlState.Normal);
			View.AddSubview(_NewImageButton);
			_NewImageButton.TouchUpInside += (sender, e) =>
			{
				Camera.TakePicture(this, (obj) =>
				{
					var image = obj.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
				
					var sivc = this.Storyboard.InstantiateViewController("SavingImagesViewController") as SavingImagesViewController;

					sivc.Image = image;
					sivc.InventoryItem = InventoryItem;

					this.NavigationController.PushViewController(sivc, true);
					//updateImage(image);
				});
			};

			_UploadImageButton = new UIButton(new CGRect(View.Frame.Width / 2, 60, View.Frame.Width / 2, 55));
			_UploadImageButton.SetTitle("Upload New Image", UIControlState.Normal);
			_UploadImageButton.ToGreenLeafButton();
			_UploadImageButton.TouchUpInside += (sender, e) =>
			{
				imagePicker = new UIImagePickerController();
				imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
				imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);

				imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
				imagePicker.Canceled += Handle_Canceled;
				//imagePicker.Delegate = this; 
				this.PresentViewController(imagePicker, true, null);
				//NavigationController.PresentViewController(imagePicker, true,null);
			};
			View.AddSubview(_UploadImageButton);

			SetupGrid();
			View.InsertSubviewBelow(dataGrid, _NewImageButton);
			//await UpdateGridWithLocalData();
			HideLoading();
		}
		protected void FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			Handle_FinishedPickingMedia(sender, e);
		}
		private void SetupGrid()
		{
			dataGrid.HeaderRowHeight = 45;
			dataGrid.RowHeight = 45;

			dataGrid.Frame = new CGRect(0, 115, View.Frame.Width, View.Frame.Height - 115);
			dataGrid.AutoGenerateColumns = false;
			dataGrid.ColumnSizer = ColumnSizer.Star;

			GridTextColumn imageColumn = new GridTextColumn();
			imageColumn.UserCellType = typeof(ImageCell);
			imageColumn.MappingName = "Image";

			dataGrid.Columns.Add(imageColumn);

			//UIButton leftSwipeViewText = new UIButton();
			//leftSwipeViewText.SetTitle("Edit", UIControlState.Normal);
			//leftSwipeViewText.SetTitleColor(UIColor.White, UIControlState.Normal);
			//leftSwipeViewText.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			//leftSwipeViewText.BackgroundColor = UIColor.FromRGB(0, 158, 218);

			//CustomSwipeView leftSwipeView = new CustomSwipeView();
			//leftSwipeView.AddSubview(leftSwipeViewText);

			UIButton rightSwipeViewText = new UIButton();
			rightSwipeViewText.SetTitle("Delete", UIControlState.Normal);
			rightSwipeViewText.SetTitleColor(UIColor.White, UIControlState.Normal);
			rightSwipeViewText.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			rightSwipeViewText.BackgroundColor = UIColor.FromRGB(220, 89, 95);
			rightSwipeViewText.AddGestureRecognizer(new UITapGestureRecognizer(DeleteTapped));

			CustomSwipeView rightSwipeView = new CustomSwipeView();
			rightSwipeView.AddSubview(rightSwipeViewText);

			this.dataGrid.RowHeight = (View.Frame.Height - 115) / 2.125;
			this.dataGrid.HeaderRowHeight = 0;
			this.dataGrid.ShowRowHeader = false;
			this.dataGrid.AllowDraggingRow = true;
			this.dataGrid.QueryRowDragging += SfGrid_QueryRowDragging;
			this.dataGrid.AllowSwiping = true;
			//this.dataGrid.LeftSwipeView = leftSwipeView;
			this.dataGrid.RightSwipeView = rightSwipeView;
			this.dataGrid.SwipeEnded += SfGrid_SwipeEnded;
			this.dataGrid.SwipeStarted += SfGrid_SwipeStarted;
			//this.dataGrid.ItemsSource
		}
		//public void updateImage(UIImage originalImage)
		//{
		//	ShowLoading();
		//	GridImage image = new GridImage();
		//	image.Image = originalImage;
		//	image.OriginalId = InventoryItem.ShopifyId;
		//	image.SortOrder = (InventoryItem.GridImages.Count + 1).ToString();
		//	var newImageId = AzureService.InventoryService.AddImageToAzure(image);
		//	if (!String.IsNullOrEmpty(newImageId))
		//	{
		//		image.OriginalId = newImageId;
		//		InventoryItem.GridImages.Add(image);
		//	}
		//	HideLoading();
		//}
		protected void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			imagePicker.DismissViewController(false, null);
			var sivc = this.Storyboard.InstantiateViewController("SavingImagesViewController") as SavingImagesViewController;

			sivc.Image = e.Info[UIImagePickerController.OriginalImage] as UIImage;
			sivc.InventoryItem = InventoryItem;
			this.NavigationController.PushViewController(sivc, true);

		}
		void Handle_Canceled(object sender, EventArgs e)
		{
			imagePicker.DismissViewController(false, null);

		}
		public void SaveIMage()
		{ 
		
		}
		public class CustomSwipeView : UIView
		{
			public CustomSwipeView()
			{
			}
			public override void LayoutSubviews()
			{
				var start = 0;
				var childWidth = this.Frame.Width;
				foreach (var v in this.Subviews)
				{
					v.Frame = new CGRect(start, 0, childWidth + start, this.Frame.Height);
					start = start + (int)this.Frame.Width;
				}
			}
		}
		private void SfGrid_QueryRowDragging(object sender, QueryRowDraggingEventArgs e)
		{
			//e.To returns the index of the current row.
			//e.From returns the index of the dragged row.
			if (e.Reason == QueryRowDraggingReason.DragEnded)
			{
				var movedItem = InventoryItem.GridImages.Where(x => x.SortOrder == (e.From).ToString()).First();
				movedItem.SortOrder = (e.To).ToString();
				if (e.To > e.From)//Moved down
				{
					foreach (var i in InventoryItem.GridImages.Where(x => Int32.Parse(x.SortOrder) >= (e.To) && x.PictureName != movedItem.PictureName).ToList())
					{
						i.SortOrder = (Int32.Parse(i.SortOrder) -1).ToString();
					}
				}
				else//Moved up
				{
					foreach (var i in InventoryItem.GridImages.Where(x => Int32.Parse(x.SortOrder) <= (e.To) && x.PictureName != movedItem.PictureName).ToList())
					{
						i.SortOrder = (Int32.Parse(i.SortOrder) + 1).ToString();
					}
				}

				InventoryItem.ReorderGridItems();

				for (int i = 0; i < InventoryItem.GridImages.Count; i++)
				{
					InventoryItem.GridImages[i].SortOrder = (i+1).ToString();
				}
				AzureService.InventoryService.UpdateImagePositions(InventoryItem);

			}
		}
		public class ImageCell : GridCell
		{
			UIImageView image;

			public ImageCell()
			{
				image = new UIImageView();
				this.AddSubview(image);
				this.CanRenderUnLoad = false;
			}

			protected override void UnLoad()
			{
				this.RemoveFromSuperview();
			}

			public override void LayoutSubviews()
			{
				base.LayoutSubviews();
				this.image.Frame = new CGRect(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
				this.image.Image = DataColumn.CellValue as UIImage;
				this.image.Image = this.image.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
				this.image.ContentMode = UIViewContentMode.ScaleAspectFit;


			}
		}
		void DeleteTapped(UITapGestureRecognizer gesture)
		{
			if (gesture.State == UIGestureRecognizerState.Ended)
			{
				var alert = UIAlertController.Create("Delete Image", String.Format("Delete Image?"), UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
				alert.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Default, (UIAlertAction obj) => DeleteSwipedItem()));
				PresentViewController(alert, animated: true, completionHandler: null);
			}
		}
		private void SfGrid_SwipeEnded(object sender, SwipeEndedEventArgs e)
		{
			SwipedItem = (GridImage)e.RowData;
		}
		private void SfGrid_SwipeStarted(object sender, SwipeStartedEventArgs e)
		{
			SwipedItem = (GridImage)e.RowData;
		}
		private async void DeleteSwipedItem()
		{
			
			if (!AzureService.InventoryService.DeleteImage(SwipedItem.PictureName))
			{
				var failedAlert = UIAlertController.Create("Failed to Delete", String.Format("Failed To Delete Image {0}", SwipedItem.SortOrder), UIAlertControllerStyle.Alert);
				failedAlert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

				PresentViewController(failedAlert, animated: true, completionHandler: null);
				return;
			}

			InventoryItem.GridImages.Remove(SwipedItem);//InventoryItem.GridImages.Where(x => x.SortOrder = SwipedItem.SortOrder).fir
			dataGrid.IsBusy = true;
			dataGrid.ItemsSource = null;
			dataGrid.ItemsSource = InventoryItem.GridImages;

			dataGrid.IsBusy = false;


			var alert = UIAlertController.Create("Deleted", String.Format("Deleted Image {0}", SwipedItem.SortOrder), UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));

			PresentViewController(alert, animated: true, completionHandler: null);
			//await UpdateGridWithLocalData();
		}

		public void ConfigureView()
		{
			if (InventoryItem == null)
			{
				return;
			}
				

		}
		private void ShowLoading()
		{
			var bounds = UIScreen.MainScreen.Bounds;
			loadingOverlay = new LoadingOverlay(bounds);
			View.Add(loadingOverlay);
		}

		private void HideLoading()
		{
			loadingOverlay.Hide();
		}
	}
}

