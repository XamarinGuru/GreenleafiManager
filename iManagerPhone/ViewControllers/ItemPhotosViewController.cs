using System;
using System.Drawing;
using Foundation;
using UIKit;
using Infragistics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace GreenleafiManagerPhone
{
	public class PhotoGridDelegate : IGGridViewDelegate
	{
		ItemPhotosViewController _vc;

		public PhotoGridDelegate(ItemPhotosViewController vc) :base()
		{
			_vc = vc;

		}
		public override void InitializeCell (IGGridView gridView, IGGridViewCell cell)
		{
			//base.InitializeCell (gridView, cell);
			//			UILongPressGestureRecognizer lpGesture = new UILongPressGestureRecognizer (() => {
			//				IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)gridView.DataSource;
			//							//InventoryNS item = (InventoryNS) dsh.ResolveDataObjectForRow (new Infragistics.IGRowPath (_row.RowIndex, (System.nint)_row.SectionIndex));
			//
			//				var pi = dsh.ResolveDataObjectForRow(new IGRowPath(cell.Path.RowIndex,0,IGGridViewFixedRowDirection.IGGridViewFixedRowDirectionNone));//new Infragistics.IGRowPath (path.RowIndex, (System.nint)path.SectionIndex));
			//				pi = null;
			//			});
			//				
			//			cell.AddGestureRecognizer(lpGesture);
		}
		public override void PageChanged (IGGridView gridView, IGCellPath path)
		{
			_vc.PageChanged (path);

		}

	}

	public class ThumbGridDelegate : IGGridViewDelegate
	{
		IGRowPath _row;
		IGGridView _gridView;
		ItemPhotosViewController _vc;
		PhotoInfo _activePI;

		public ThumbGridDelegate(ItemPhotosViewController vc) :base()
		{
			_vc = vc;
		}

		public override void InitializeCell (IGGridView gridView, IGGridViewCell cell)
		{
			cell.SelectedColor = UIColor.FromWhiteAlpha (1, .9f);
		}

		public override void DidSelectCell (IGGridView gridView, IGCellPath path)
		{
			_vc.ThumbSelected (path);
		}
		[Export ("gridView:viewForSlideRowRight:")]
		public override UIKit.UIView ResolveSlideRowRightView (IGGridView gridView, IGRowPath path)
		{
			_row = path;
			_gridView = gridView;

			IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper)gridView.DataSource;
			//InventoryNS item = (InventoryNS) dsh.ResolveDataObjectForRow (new Infragistics.IGRowPath (_row.RowIndex, (System.nint)_row.SectionIndex));
			var pi = dsh.ResolveDataObjectForRow(new IGCellPath(path.RowIndex, path.SectionIndex, (nint)0));//new Infragistics.IGRowPath (path.RowIndex, (System.nint)path.SectionIndex));
			_activePI = (PhotoInfo)pi;

			UILabel label = new UILabel();
			label.BackgroundColor = (new UIColor((float)(76.0/255.0), (float)(217.0/255.0), (float)(100.0/255.0), (float)(1.0))) ;
			label.TextColor = UIColor.White;
			label.TextAlignment = UITextAlignment.Center;
			label.Text = _activePI.Position == "1" ? "Currently Thumbnail" : "Set As Thumbnail" ;
			label.LineBreakMode = UILineBreakMode.WordWrap;
			label.Lines = 0; 
			label.UserInteractionEnabled = true;
			UITapGestureRecognizer tapGestureRecognizer = new UITapGestureRecognizer(SetAsThumbnail);
			label.AddGestureRecognizer(tapGestureRecognizer);
			return label;
		}
		[Export("setAsThumbnail")]
		public async void SetAsThumbnail()
		{
			//IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper) parentController._grid.WeakDataSource;

			var foundImage = ((InventoryDetailViewController)_vc.ParentInventoryDetailViewController).InventoryItem.Images.Where(x=>x.LocalPath == _activePI.ImagePath).FirstOrDefault();
			if (foundImage != null) {
				//_vc.ParentInventoryDetailViewController.InventoryService.NeedLocalRefresh = true;
				var updatedImage = await SetImageAsThumbnail(foundImage);
				if (updatedImage != null) {
					foundImage.position = updatedImage.position;
					foundImage.updated_at = updatedImage.updated_at;

					_activePI.Position = "1";
					var curentlySelectedCell = _gridView.PathForSelectedCell;
					_gridView.ReloadData ();
					_gridView.ScrollToCell (new IGCellPath (_row.RowIndex, _row.SectionIndex, (nint)0), IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
					_gridView.SelectCell (curentlySelectedCell, false, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle);
				}
			}
		}
		//		[Export ("gridView:viewForSlideRowLeft:")]
		//		public override UIKit.UIView ResolveSlideRowLeftView (Infragistics.IGGridView gridView, Infragistics.IGRowPath path)
		//		{
		//			//_row = path;
		//			//_gridView = gridView;
		//
		//			UILabel label = new UILabel();
		//			label.BackgroundColor = (new UIColor((float)(255.0/255.0), (float)(59.0/255.0), (float)(48.0/255.0), (float)(1.0)));// UIColor.Red;
		//			label.TextColor = UIColor.White;
		//			label.TextAlignment = UITextAlignment.Center;
		//			label.Text = "Delete";
		//			label.UserInteractionEnabled = true;
		//			UITapGestureRecognizer tapGestureRecognizer = new UITapGestureRecognizer(DeleteRow);
		//			label.AddGestureRecognizer(tapGestureRecognizer);
		//			return label;
		//		}

		[Export("DeleteRow")]
		public void DeleteRow()
		{
			//			IGGridViewDataSourceHelper dsh = (IGGridViewDataSourceHelper) parentController._grid.WeakDataSource;
			//			InventoryNS item = (InventoryNS) dsh.ResolveDataObjectForRow (new Infragistics.IGRowPath (_row.RowIndex, (System.nint)_row.SectionIndex));
			//
			//			parentController.ShowUpdatingOverlay ();
			//
			//			var service = QSAzureService.InventoryService;
			//			service.DeleteItem (item.ConvertToInventory ());
			//
			//			parentController.HideUpdatingOverlay ();
			//
			//			service.NeedLocalRefresh = true;
			//			parentController.ViewDidAppear (true);
		}
		public async Task<ProductImage> SetImageAsThumbnail(ProductImage image)
		{
			ClearOldThumbnailSelection ();
			var sa = new ShopifyAdapter ();
			image.position = "1";
			var updatedImage = await sa.UpdateShopifyProductImage (image);
			return updatedImage;
		}
		public async void ClearOldThumbnailSelection()
		{
			var thumbImages = ((InventoryDetailViewController)_vc.ParentInventoryDetailViewController).InventoryItem.Images.Where (x => x.position == "1");

			var sa = new ShopifyAdapter ();
			foreach (var i in thumbImages) {
				i.position = "";
				var updatedImage = await sa.UpdateShopifyProductImage (i);
				var foundImage = ((InventoryDetailViewController)_vc.ParentInventoryDetailViewController).InventoryItem.Images.Where (x => x.id == updatedImage.id).FirstOrDefault ();
				if (foundImage != null) {
					foundImage.position = updatedImage.position;
					foundImage.updated_at = updatedImage.updated_at;
				}
			}
		}
	}

	public class PhotoInfo : NSObject
	{
		[Export("imagePath")]
		public string ImagePath 
		{
			get;
			set;
		}

		[Export("image")]
		public UIImage Image 
		{
			get 
			{
				UIImage img = UIImage.FromBundle (this.ImagePath);
				return img;
			}

		}

		[Export("thumb")]
		public UIImage Thumb 
		{
			get 
			{
				return UIImage.FromBundle (this.ImagePath);
			}

		}

		[Export("position")]
		public string Position 
		{
			get;
			set;
		}

		public string id{ get; set; }
		public string product_id{ get; set; }
		public string updated_at{ get; set; }
		public string created_at{ get; set; }
	}


	public partial class ItemPhotosViewController : UIViewController
	{
		IGGridView _photoGridView;
		IGGridViewSingleRowSingleFieldDataSourceHelper _photoDS;

		IGGridView _thumbsGridView;
		IGGridViewSingleRowSingleFieldDataSourceHelper _singleRowThumbDS;
		IGGridViewSingleFieldMultiColumnDataSourceHelper _thumbsDS;

		int _numberOfColumns, _maxNumberOfColumns, _portraitColumnCount;

		UIView _resizeThumb;
		PointF _originPoint;

		float _columnSize, _resizeThumbSize,_resizeThumbOffset ;

		List<PhotoInfo> _photos;

		SizeF _landscapeSize, _portraitSize;
		nint _SelectedPhotoRowNumber = 0;
		//bool _HidingThumbs = false;

		public InventoryDetailViewController ParentInventoryDetailViewController;

		public List<PhotoInfo> Photos {
			get{ return _photos; }
			set{ _photos = value; 
				if(_photos == null || _photos.Count == 0) {
					var pi = new PhotoInfo (){ImagePath = "Images/no-thumb.png" };
					_photos = new List<PhotoInfo> ();
					_photos.Add (pi);
				}

			}
		}

		public ItemPhotosViewController (IntPtr handle) : base (handle)
		{
		}
		public ItemPhotosViewController () : base ("PhotoViewerViewController", null)
		{

		}



		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = UIColor.FromWhiteAlpha (.1f, 1);

			float thumbGridHeight = 150; 

			_columnSize = thumbGridHeight;

			_numberOfColumns = 1;
			_resizeThumbSize = 50;
			_maxNumberOfColumns = 1;
			_portraitColumnCount = 6;
			_resizeThumbOffset = 20;


			_landscapeSize = new SizeF((float)UIScreen.MainScreen.Bounds.Width, (float)UIScreen.MainScreen.Bounds.Height);
			_portraitSize = new SizeF((float)UIScreen.MainScreen.Bounds.Height, (float)UIScreen.MainScreen.Bounds.Width);

			_photoGridView = new IGGridView (new RectangleF (0, 0, (float)this.View.Frame.Size.Width, (float)(this.View.Frame.Size.Height)), IGGridViewStyle.IGGridViewStyleSingleCellPaging);

			_photoGridView.Delegate = new PhotoGridDelegate(this);
			_photoGridView.SelectionType = IGGridViewSelectionType.IGGridViewSelectionTypeNone;
			_photoGridView.AlwaysBounceVertical = false;
			_photoGridView.AllowHorizontalBounce = false;


			//_photoGridView.ColumnSpacing = 15f;
			_photoGridView.RowSpacing = 25f;

			//_photoGridView.Frame = new RectangleF(50f, 0f, 974f, 753f);

			this.View.AddSubview (_photoGridView);

			_resizeThumb = new UIView ();
			_resizeThumb.BackgroundColor = UIColor.Clear;
			UIPanGestureRecognizer pan = new UIPanGestureRecognizer (this, new ObjCRuntime.Selector ("pan:"));
			_resizeThumb.AddGestureRecognizer (pan);
			this.View.AddSubview (_resizeThumb);

			_thumbsGridView = new IGGridView (new RectangleF (0, (float)_photoGridView.Frame.Height, (float)this.View.Frame.Size.Width, (float)thumbGridHeight), IGGridViewStyle.IGGridViewStyleDefault);
			_thumbsGridView.RowSeparatorHeight = 0; 
			_thumbsGridView.Delegate = new ThumbGridDelegate(this);
			_thumbsGridView.SelectionType = IGGridViewSelectionType.IGGridViewSelectionTypeCell;
			_thumbsGridView.HeaderHeight = 0; 
			_thumbsGridView.ColumnWidth = IGColumnWidth.CreateNumericColumnWidth (thumbGridHeight);
			_thumbsGridView.RowHeight = thumbGridHeight;
			_thumbsGridView.AllowHorizontalBounce = true;
			_thumbsGridView.AlwaysBounceHorizontal = true;
			_thumbsGridView.AlwaysBounceVertical = false;
			_thumbsGridView.SwipeRowAction = IGGridViewSwipeRowAction.IGGridViewSwipeRowActionSlideRowRight;

			this.View.AddSubview (_thumbsGridView);

			//			_photos = new List<PhotoInfo> ();
			//			for(int i = 1; i <= 26; i++)
			//			{
			//				PhotoInfo photoInfo = new PhotoInfo ();
			//				photoInfo.ImagePath = string.Format ("Photos/photo{0}.jpeg", i);
			//				photoInfo.ThumbPath = string.Format ("Thumbnails/thumb{0}.jpeg", i);
			//				_photos.Add (photoInfo);
			//			}


			IGGridViewImageColumnDefinition col = new IGGridViewImageColumnDefinition ("image", IGGridViewImageColumnDefinitionPropertyType.IGGridViewImageColumnDefinitionPropertyTypeImage);
			col.EnableZooming = true;

			_photoDS = new IGGridViewSingleRowSingleFieldDataSourceHelper (col);
			_photoDS.Data = _photos.ToArray();

			_photoGridView.DataSource = _photoDS;

			// changedd: IGGridViewImageColumnDefinition thumbCol = new IGGridViewImageColumnDefinition ("image", IGGridViewImageColumnDefinitionPropertyType.IGGridViewImageColumnDefinitionPropertyTypeImage);
			IGGridViewImageColumnDefinition thumbCol = new IGGridViewImageColumnDefinition ("thumb", IGGridViewImageColumnDefinitionPropertyType.IGGridViewImageColumnDefinitionPropertyTypeImage);
			_singleRowThumbDS = new IGGridViewSingleRowSingleFieldDataSourceHelper (thumbCol);
			_singleRowThumbDS.Data = _photos.ToArray();

			_thumbsDS = new IGGridViewSingleFieldMultiColumnDataSourceHelper (thumbCol);
			_thumbsDS.NumberOfColumns = 1;
			#region New changes

			var thumbPhotos = new List<PhotoInfo> ();
			foreach (var item in _photos) {
				thumbPhotos.Add (item);
			}
			foreach (var item in thumbPhotos) {
				item.ImagePath = item.ImagePath.Replace (".jpg", "_thumb.jpg");
			}
			_thumbsDS.Data = thumbPhotos.ToArray();
			#endregion

			_thumbsDS.Data = _photos.ToArray();
			_thumbsGridView.DataSource = _thumbsDS;

		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			FakePan ();
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);
			this.UpdateLayoutForOrientation (this.InterfaceOrientation, 0);
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);
			this.UpdateLayoutForOrientation (toInterfaceOrientation,  duration);
		}

		public void ThumbSelected(IGCellPath path)
		{
			IGGridViewDataSourceHelper ds = (IGGridViewDataSourceHelper)_thumbsGridView.DataSource;
			if (_numberOfColumns > 0) {

				path = ds.NormalizePath (path);

				path = _photoDS.DenormalizePath (path);
				_SelectedPhotoRowNumber = path.ColumnIndex;// _photoDS.DenormalizePath (path);
				_photoGridView.ScrollToCell (path, IGGridViewScrollPosition.IGGridViewScrollPositionNone, false);
			} 
		}

		public void PageChanged(IGCellPath path)
		{
			if(_thumbsGridView.DataSource == _thumbsDS)
			{
				if (_numberOfColumns > 0) {
					path = _photoDS.NormalizePath (path);
					path = _thumbsDS.DenormalizePath (path);
				}
			}
			//			if (_HidingThumbs) {
			//				_HidingThumbs = false;
			//			} else 
			{
				_SelectedPhotoRowNumber = path.ColumnIndex;
			}
			_thumbsGridView.SelectCell (path, false, IGGridViewScrollPosition.IGGridViewScrollPositionNone);
			_thumbsGridView.ScrollToCell(path, IGGridViewScrollPosition.IGGridViewScrollPositionNone, true);
			this.ThumbSelected(path);

		}

		public void UpdateLayoutForOrientation(UIInterfaceOrientation interfaceOrientation,  double duration)
		{
			float size = _columnSize* _numberOfColumns;

			if(interfaceOrientation == UIInterfaceOrientation.LandscapeLeft || interfaceOrientation == UIInterfaceOrientation.LandscapeRight)
			{
				UIView.Animate (duration, new Action (() =>
					{
						_thumbsGridView.Frame  = new RectangleF(0, 0, size, _landscapeSize.Height);
						_resizeThumb.Frame = new RectangleF(size,0, _resizeThumbSize, _landscapeSize.Height);
						_photoGridView.Frame = new RectangleF(size + _resizeThumbSize, 0, _landscapeSize.Width - (size + _resizeThumbSize) - 15, _landscapeSize.Height-15);
					}));


				List<UIView> subViews = new List<UIView>(_resizeThumb.Subviews);
				foreach(UIView view in subViews)
					view.RemoveFromSuperview();

				float left = _resizeThumbOffset;
				for(int i = 0; i < 3; i++)
				{
					UIView block = new UIView ();
					block.BackgroundColor = UIColor.Gray;
					_resizeThumb.AddSubview (block);
					block.Frame = new RectangleF(left, _landscapeSize.Height/2 - (_resizeThumbOffset/2), 2, _resizeThumbOffset);	
					left += 5;
				}
				this.UpdateDataForOrientation(true);
			}
			else
			{
				UIView.Animate (duration, new Action (() =>
					{
						_thumbsGridView.Frame  = new RectangleF(0, _portraitSize.Height - size, _portraitSize.Width, size);
						_resizeThumb.Frame = new RectangleF(0, _portraitSize.Height - (size + _resizeThumbSize), _portraitSize.Width, _resizeThumbSize);
						_photoGridView.Frame = new RectangleF(0, 0, _portraitSize.Width, _portraitSize.Height - (size + _resizeThumbSize));
					}));

				List<UIView> subViews = new List<UIView>(_resizeThumb.Subviews);
				foreach(UIView view in subViews)
					view.RemoveFromSuperview();

				float top = _resizeThumbOffset; 
				for(int i = 0; i < 3; i++)
				{
					UIView block = new UIView ();
					block.BackgroundColor = UIColor.Gray;
					_resizeThumb.AddSubview (block);
					block.Frame = new RectangleF(_portraitSize.Width/2 - (_resizeThumbOffset/2), top, _resizeThumbOffset, 2);
					top += 5;
				}
				this.UpdateDataForOrientation(false);
			}
		}
		private void FakePan()
		{
			var newX = 0;
			_numberOfColumns = 0;

			UIView.Animate (0.5f, new Action (() =>
				{
					_resizeThumb.Frame = new RectangleF((float)newX,(float)_resizeThumb.Frame.Y, (float)_resizeThumb.Frame.Width, (float)_resizeThumb.Frame.Height);
					_photoGridView.Frame = new RectangleF((float)(newX + _resizeThumb.Frame.Width), (float)0, (float)(_landscapeSize.Width - newX - _resizeThumb.Frame.Width -15f), (float)_photoGridView.Frame.Height);
					_thumbsGridView.Frame = new RectangleF(0,0, (float)(_numberOfColumns * _columnSize), (float)_thumbsGridView.Frame.Height);
				}));			
		}
		[Export("pan:")]
		public void Pan(UIPanGestureRecognizer gesture)
		{
			if(gesture.State == UIGestureRecognizerState.Began)
				_originPoint = (PointF)_resizeThumb.Frame.Location;

			PointF currentPoint = (PointF)gesture.TranslationInView (_resizeThumb.Superview);

			if(this.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || this.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
			{
				float newX = _originPoint.X + currentPoint.X;
				if (newX < _columnSize)
					newX = 0;//_columnSize;
				else if (newX > (_maxNumberOfColumns * _columnSize))
					newX = (_maxNumberOfColumns * _columnSize);
				else if (newX == _columnSize)
					newX = 0;

				if (newX == 0)
					_numberOfColumns = 0;
				else
					_numberOfColumns = ((int)(newX/_columnSize));

				float duration = 0;
				if(gesture.State == UIGestureRecognizerState.Ended)
				{
					newX = _numberOfColumns * _columnSize;
					duration = .5f;

					UIView.Animate (duration, new Action (() =>
						{
							_resizeThumb.Frame = new RectangleF((float)newX,(float)_resizeThumb.Frame.Y, (float)_resizeThumb.Frame.Width, (float)_resizeThumb.Frame.Height);
							_photoGridView.Frame = new RectangleF((float)(newX + _resizeThumb.Frame.Width), (float)0, (float)(_landscapeSize.Width - newX - _resizeThumb.Frame.Width -15f), (float)_photoGridView.Frame.Height);
							_thumbsGridView.Frame = new RectangleF(0,0, (float)(_numberOfColumns * _columnSize), (float)_thumbsGridView.Frame.Height);
						}));

					//					//Update Viewed thumbs and photos
					ScrollToPhotoAndThumb();

					//					IGCellPath p = new IGCellPath ((nint)0, (nint)0, _SelectedPhotoRowNumber,IGGridViewFixedColumnDirection.IGGridViewFixedColumnDirectionNone);
					//
					//					if (newX == 0) {	
					//						_photoGridView.ScrollToCell (p, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
					//						//_photoGridView.ScrollToCell (p, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
					//
					//					} else {
					//						_photoGridView.ScrollToCell (p, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
					//
					//						IGGridViewDataSourceHelper ds = (IGGridViewDataSourceHelper)_thumbsGridView.DataSource;
					//						var path = ds.NormalizePath (p);
					//
					//						path = ds.NormalizePath (path);
					//
					//						//path = _photoDS.DenormalizePath (path);
					//
					//						_thumbsGridView.SelectCell (path, false, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle);
					//					}
				}

				if(_numberOfColumns > 0 && _numberOfColumns != _thumbsDS.NumberOfColumns)
				{
					this.UpdateDataForOrientation (true);
				}
				//_DisplayedPathForThumbs
				//IGCellPath p = new IGCellPath(4,0);


			}
			//			else
			//			{
			//				float newY = ((float)(this.View.Frame.Height - (_originPoint.Y + currentPoint.Y)) - _resizeThumbSize);
			//				if(newY < _columnSize)
			//					newY = _columnSize; 
			//				else if(newY > (_maxNumberOfColumns * _columnSize))
			//					newY = (_maxNumberOfColumns * _columnSize); 
			//
			//				_numberOfColumns = ((int)(newY/_columnSize));
			//
			//				float duration = 0;
			//				if(gesture.State == UIGestureRecognizerState.Ended)
			//				{
			//					newY = _numberOfColumns * _columnSize;
			//					duration = .3f;
			//				}
			//
			//				int previousColumns = (int)((this.View.Frame.Height - (_resizeThumb.Frame.Location.Y + _resizeThumbSize))/_columnSize);
			//
			//				UIView.Animate (duration, new Action (() =>
			//					{
			//						_resizeThumb.Frame = new RectangleF(0,  (float)(this.View.Frame.Height - (newY + _resizeThumbSize)), (float)_resizeThumb.Frame.Width, (float)_resizeThumbSize);
			//						_photoGridView.Frame = new RectangleF(0, 0, (float)_photoGridView.Frame.Width, (float)(_portraitSize.Height - (newY + _resizeThumbSize)));
			//						_thumbsGridView.Frame  = new RectangleF(0, (float)(this.View.Frame.Height - newY), (float)this.View.Frame.Width, newY);
			//					}));
			//
			//				if(_numberOfColumns != previousColumns)
			//					this.UpdateDataForOrientation (false);
			//
			//			}
		}

		public void UpdateDataForOrientation(bool isLandscape)
		{
			//			IGCellPath path;// = _DisplayedPathForThumbs;// null;
			//		
			//			if (_numberOfColumns > 0) {
			//				//path = _thumbsGridView.PathForSelectedCell;//.SelectedCellPath();
			//
			//				if (path == null)
			//					path = new IGCellPath ((System.nint)0, (System.nint)0, (System.nint)0);
			//
			//				IGGridViewDataSourceHelper ds = (IGGridViewDataSourceHelper)_thumbsGridView.DataSource;
			//				path = ds.NormalizePath (path);
			//			} else {
			//				//path = _photoGridView.PathForSelectedCell;//.SelectedCellPath();
			//
			//				if (path == null)
			//					path = new IGCellPath ((System.nint)0, (System.nint)0, (System.nint)0);
			//
			//				IGGridViewDataSourceHelper ds = (IGGridViewDataSourceHelper)_photoGridView.DataSource;
			//				//path = ds.NormalizePath (path);
			//			}
			if(isLandscape)
			{
				_thumbsGridView.DataSource = _thumbsDS;
				_thumbsGridView.ColumnWidth = IGColumnWidth.CreatePercentColumnWidth (1);
				_thumbsDS.NumberOfColumns = _numberOfColumns;
				_thumbsDS.InvalidateData ();


				_thumbsGridView.AllowHorizontalBounce = false;
				_thumbsGridView.AlwaysBounceHorizontal = false;
				_thumbsGridView.AlwaysBounceVertical = true;
			}
			else
			{
				if(_numberOfColumns == 1)
				{
					_thumbsGridView.DataSource = _singleRowThumbDS;
					_thumbsGridView.ColumnWidth =  IGColumnWidth.CreateNumericColumnWidth (_portraitSize.Width/_portraitColumnCount);
					_thumbsGridView.AllowHorizontalBounce = true;
					_thumbsGridView.AlwaysBounceHorizontal = true;
					_thumbsGridView.AlwaysBounceVertical = false;
				}
				else
				{
					_thumbsGridView.DataSource = _thumbsDS;
					_thumbsGridView.ColumnWidth = IGColumnWidth.CreatePercentColumnWidth (1);
					_thumbsDS.NumberOfColumns = _portraitColumnCount;
					_thumbsDS.InvalidateData ();

					_thumbsGridView.AllowHorizontalBounce = false;
					_thumbsGridView.AlwaysBounceHorizontal = false;
					_thumbsGridView.AlwaysBounceVertical = true;
				}
			}

			_thumbsGridView.UpdateData ();
			ScrollToPhotoAndThumb();


			//			IGGridViewDataSourceHelper currentDS = (IGGridViewDataSourceHelper)_thumbsGridView.DataSource;
			//			path = currentDS.DenormalizePath (path);
			//
			//			_thumbsGridView.SelectCell (path, false, IGGridViewScrollPosition.IGGridViewScrollPositionNone);
			//			_thumbsGridView.ScrollToCell (path, IGGridViewScrollPosition.IGGridViewScrollPositionNone, false);
		}

		private void ScrollToPhotoAndThumb()
		{
			//Update Viewed thumbs and photos
			IGCellPath photoPath = new IGCellPath ((nint)0, (nint)0, _SelectedPhotoRowNumber,IGGridViewFixedColumnDirection.IGGridViewFixedColumnDirectionNone);
			IGCellPath thumbPath = new IGCellPath (_SelectedPhotoRowNumber, (nint)0, (nint)0,IGGridViewFixedColumnDirection.IGGridViewFixedColumnDirectionNone);

			if (_numberOfColumns > 0) {
				_photoGridView.ScrollToCell (photoPath, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);

				IGGridViewDataSourceHelper ds = (IGGridViewDataSourceHelper)_thumbsGridView.DataSource;
				_thumbsGridView.SelectCell (thumbPath, false, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle);
				_thumbsGridView.ScrollToCell (thumbPath, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
			}
			else {
				_photoGridView.ScrollToCell (photoPath, IGGridViewScrollPosition.IGGridViewScrollPositionMiddle, false);
			}
		}
	}
}