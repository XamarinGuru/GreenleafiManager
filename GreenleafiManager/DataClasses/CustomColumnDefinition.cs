using System;
using Infragistics;
using Foundation;
using UIKit;
using System.Linq;
using System.Drawing;
using CoreGraphics;
using System.Threading.Tasks;

namespace GreenleafiManager
{
	public class FrontTagColumnDefinition : IGGridViewColumnDefinition
	{
		public FrontTagColumnDefinition(string key) : base(key)
		{
		}

		public override IGGridViewCell CreateCell(IGGridView gridView, IGCellPath path, IGGridViewDataSourceHelper dataSource)
		{
			TagFrontCell cell = (TagFrontCell)gridView.DequeueReusableCell("LabelCell");
			if (cell == null)
				cell = new TagFrontCell("LabelCell");

			InventoryNS invItem = (InventoryNS)dataSource.ResolveDataObjectForRow(path);

			cell.TextLabel.Text = invItem?.GlShortCode;
			cell.SecretCode.Text = invItem?.SecretCode;
			cell.TagPriceLabel.Text = invItem != null ? Math.Ceiling(invItem.TagPrice).ToString("C") : "0";
			cell.Sku = invItem?.Sku;

			return cell;
		}

		public class TagFrontCell : IGGridViewCell
		{
			public UILabel SecretCode
			{
				get;
				set;
			}

			public UILabel TagPriceLabel
			{
				get;
				set;
			}
			public string Sku { get; set; }
			public IGBarcodeView BarcodeView { get; set; }

			public TagFrontCell(string identifier) : base(identifier)
			{
				this.SecretCode = new UILabel();
				this.TagPriceLabel = new UILabel();
				this.AddSubview(this.SecretCode);
				this.AddSubview(this.TagPriceLabel);



				this.TextLabel.TextAlignment = UITextAlignment.Left;
				this.SecretCode.TextAlignment = UITextAlignment.Left;
				this.SecretCode.TextAlignment = UITextAlignment.Left;
			}

			public override void SetupSize(CGSize size)
			{
				nfloat height = size.Height / 5;

				this.TextLabel.Frame = new RectangleF(0f, (float)height * 2, (float)size.Width, (float)height);
				TextLabel.Font = UIFont.FromName("Helvetica", 40f);

				this.SecretCode.Frame = new RectangleF(0f, (float)height * 3, (float)size.Width, (float)height);
				SecretCode.Font = UIFont.FromName("Helvetica", 40f);

				this.TagPriceLabel.Frame = new RectangleF(0f, (float)height * 4, (float)size.Width, (float)height);
				TagPriceLabel.Font = UIFont.FromName("Helvetica", 40f);

				RectangleF barcodeRect = new RectangleF(-20f, 0f, (float)size.Width, (float)height * 2);
				BarcodeView = IGBarcodeView.CreateBarcodeFrame(IGBarcodeType.IGBarcodeTypeCode128, barcodeRect);
				BarcodeView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;
				BarcodeView.BackgroundColor = UIColor.Clear;
				BarcodeView.SetValue(String.IsNullOrWhiteSpace(Sku) ? "0" : Sku);
				BarcodeView.ShowText = false;

				this.AddSubview(BarcodeView);
			}
			public override void CellAttached()
			{
				base.CellAttached();
				if (BarcodeView != null)
				{
					BarcodeView.SetValue(String.IsNullOrWhiteSpace(Sku) ? "0" : Sku);
				}

			}
		}
	}
	public class BackTagColumnDefinition : IGGridViewColumnDefinition
	{
		public BackTagColumnDefinition(string key) : base(key)
		{
		}

		public override IGGridViewCell CreateCell(IGGridView gridView, IGCellPath path, IGGridViewDataSourceHelper dataSource)
		{
			TagBackCell cell = (TagBackCell)gridView.DequeueReusableCell("LabelCell");

			if (cell == null)
				cell = new TagBackCell("LabelCell");

			InventoryNS invItem = (InventoryNS)dataSource.ResolveDataObjectForRow(path);

			cell.TextLabel.Text = invItem?.MetalCode;
			cell.InfoLine1Label.Text = invItem?.Info1;
			cell.InfoLine2Label.Text = invItem?.Info2;
			cell.InfoLine3Label.Text = invItem?.Info3;
			cell.SkuLabel.Text = invItem?.Sku;

			return cell;
		}
		public class TagBackCell : IGGridViewCell
		{
			public UILabel InfoLine1Label
			{
				get;
				set;
			}
			public UILabel InfoLine2Label
			{
				get;
				set;
			}
			public UILabel InfoLine3Label
			{
				get;
				set;
			}
			public UILabel SkuLabel
			{
				get;
				set;
			}
			public string Sku { get; set; }

			public TagBackCell(string identifier) : base(identifier)
			{
				this.InfoLine1Label = new UILabel();
				this.InfoLine2Label = new UILabel();
				this.InfoLine3Label = new UILabel();
				this.SkuLabel = new UILabel();

				this.AddSubview(this.InfoLine1Label);
				this.AddSubview(this.InfoLine2Label);
				this.AddSubview(this.InfoLine3Label);
				this.AddSubview(this.SkuLabel);


				this.TextLabel.TextAlignment = UITextAlignment.Left;
				this.InfoLine1Label.TextAlignment = UITextAlignment.Left;
				this.InfoLine2Label.TextAlignment = UITextAlignment.Left;
				this.InfoLine3Label.TextAlignment = UITextAlignment.Left;
				this.SkuLabel.TextAlignment = UITextAlignment.Left;
			}

			public override void SetupSize(CGSize size)
			{
				nfloat height = size.Height / 5;

				this.TextLabel.Frame = new RectangleF(0f, 0f, (float)size.Width, (float)height);
				TextLabel.Font = UIFont.FromName("Helvetica", 40f);

				this.InfoLine1Label.Frame = new RectangleF(0f, (float)height * 1, (float)size.Width, (float)height);
				InfoLine1Label.Font = UIFont.FromName("Helvetica", 40f);

				this.InfoLine2Label.Frame = new RectangleF(0f, (float)height * 2, (float)size.Width, (float)height);
				InfoLine2Label.Font = UIFont.FromName("Helvetica", 40f);

				this.InfoLine3Label.Frame = new RectangleF(0f, (float)height * 3, (float)size.Width, (float)height);
				InfoLine3Label.Font = UIFont.FromName("Helvetica", 40f);

				this.SkuLabel.Frame = new RectangleF(0f, (float)height * 4, (float)size.Width, (float)height);
				SkuLabel.Font = UIFont.FromName("Helvetica", 40f);
			}
		}
	}

	public class ImageColumnDefinition : IGGridViewImageColumnDefinition
	{
		MasterInventoryViewController _controller;
		public ImageColumnDefinition(string key, IGGridViewImageColumnDefinitionPropertyType columnType, MasterInventoryViewController controller) : base(key, columnType)
		{
			_controller = controller;
		}

		public override IGGridViewCell CreateCell(IGGridView gridView, IGCellPath path, IGGridViewDataSourceHelper dataSource)
		{
			InventoryNS invItem = (InventoryNS)dataSource.ResolveDataObjectForRow(path);
			if (invItem.Images == null)// || invItem.Images.Count == 0)
			{
				var overlay = new LoadingOverlay(gridView.Bounds);
				gridView.Add(overlay);
				gridView.UserInteractionEnabled = false;
				int next = 1;
				do
				{
					try
					{
						//get next 20, if less than 20 get everyone you can
						var nextInvItem = (InventoryNS)dataSource.ResolveDataObjectForRow(new IGRowPath(path.RowIndex + next, 0));
						nextInvItem.LoadImagesFromDrive();
						next++;
					}
					catch (Exception)
					{
						next = 21;
					}
				}
				while (next < 20);
				overlay.Hide();
				gridView.UserInteractionEnabled = true;
			}
			return base.CreateCell(gridView, path, dataSource);
		}
	}
}

