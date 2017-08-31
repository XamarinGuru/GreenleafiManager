using System;
using Infragistics;
using Foundation;
using UIKit;
using CoreGraphics;
using System.Linq;

namespace GreenleafiManager
{
	public class InvoiceItemCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("InvoiceItemCell");
		public static readonly UINib Nib;
		public UIView seprator;
		UITableView tblView;
		UIViewController controller;
		bool IsEdit;
		public InvoiceItemCell(UITableView _tblView, UIViewController _controller, bool _IsEdit)
		{
			tblView = _tblView;
			controller = _controller;
			IsEdit = _IsEdit;
		}

		public void UpdateCell(Item objItem)
		{
			
			if (objItem != null)
			{
				nfloat vFullWidth = tblView.Frame.Width;
				nfloat columnW = vFullWidth / 20;
				var label1 = new UILabel(new CGRect(0, 0, columnW * 2, 40));
				label1.Text = objItem?.GlItemCode;
				label1.ToGreenLeafLabel();
				label1.UserInteractionEnabled = true;
				label1.AddGestureRecognizer(TapGestureToGrids(objItem));
				ContentView.Add(label1);

				var label2 = new UILabel(new CGRect(label1.Frame.X + label1.Frame.Width - 1, 0, columnW * 2 , 40));
				label2.Text = objItem?.MetalCode;
				label2.ToGreenLeafLabel();
				label2.UserInteractionEnabled = true;
				label2.AddGestureRecognizer(TapGestureToGrids(objItem));
				ContentView.Add(label2);

				var label3 = new UILabel(new CGRect(label2.Frame.X + label2.Frame.Width - 1, 0, columnW * 2, 40));
				label3.Text = objItem?.Sku;
				label3.ToGreenLeafLabel();
				label3.UserInteractionEnabled = true;
				label3.AddGestureRecognizer(TapGestureToGrids(objItem));
				ContentView.Add(label3);

				var label4 = new UILabel(new CGRect(label3.Frame.X + label3.Frame.Width - 1, 0, columnW * 3, 40));
				label4.Text = objItem.ItemSalePrice.ToString("C");
				label4.ToGreenLeafLabel();
				label4.UserInteractionEnabled = true;
				label4.AddGestureRecognizer(TapGestureToGrids(objItem));
				ContentView.Add(label4);

				var label5 = new UILabel(new CGRect(label4.Frame.X + label4.Frame.Width - 1, 0, columnW * 11 - 40, 40));
				if (string.IsNullOrEmpty(objItem?.InvoiceDescription))
					label5.Text = objItem?.Description;
				else
					label5.Text = objItem?.InvoiceDescription;
				label5.Lines = 2;
				label5.ToGreenLeafLabel();
				label5.UserInteractionEnabled = true;
				label5.AddGestureRecognizer(TapGestureToGrids(objItem));
				ContentView.Add(label5);

				var btnCross = new UIButton(new CGRect(label5.Frame.X + label5.Frame.Width + 1, 0, 40, 40));
				btnCross.SetTitle("X", UIControlState.Normal);
				//btnCross.Layer.BorderWidth = 1;
				//btnCross.Layer.BorderColor = UIColor.Gray.CGColor;
				btnCross.ToGreenLeafButton();
				btnCross.SetTitleColor(UIColor.Black, UIControlState.Normal);
				btnCross.TouchUpInside += (sender, e) =>
				{
					if (controller.Class.Name == "InvoiceEditViewController")
					{
						UIAlertController alert = UIAlertController.Create("Delete item from invoice?", "Are you sure you want to remove this item from this invoice?" +
																		   "Hit the save button to save changes.", UIAlertControllerStyle.Alert);
						alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
						{
						}));
						alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, async (actionOK) =>
						{
							objItem.RemoveFromInvoice = true;
							((InvoiceEditViewController)controller).itemListToRemove.Add(objItem);
							foreach (var item in ((InvoiceEditViewController)controller).itemListToRemove)
							{
								((InvoiceEditViewController)controller).objInvoice.Items.Remove(item);
								((InvoiceEditViewController)controller).itemsList.Remove(item);
								AzureService.InvoiceService.ItemsList.Remove(item);
								((InvoiceEditViewController)controller).SetupAmounts();
							}
							if (objItem.AddNewFromInvoice)
							{
								//RMR DJ this is bad can't call async and not await it
								await AzureService.InventoryService.MarkItemSoldAsync(objItem, false);
								AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == objItem.Id).IsSold = false;
								((InvoiceEditViewController)controller).itemListToRemove.Remove(objItem);
							}

							var list = ((InvoiceEditViewController)controller).itemsList.Where(x => x.AddNewFromInvoice).ToList();
							list.AddRange(((InvoiceEditViewController)controller).objInvoice.Items);
							tblView.Source = new InvoiceItemTableSource(list, controller);
							tblView.ReloadData();
						}));
						((InvoiceEditViewController)controller).PresentViewController(alert, true, null);

					}
					else if (controller.Class.Name == "InvoiceCreateViewController")
					{
						UIAlertController alert = UIAlertController.Create("Delete item from invoice?", "Are you sure you want to remove this item from this invoice?" +
																		   "Hit the save button to save changes.", UIAlertControllerStyle.Alert);
						alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
						{
						}));
						alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, async (actionOK) =>
						{
							objItem.AddNewFromInvoice = false;
							objItem.RemoveFromInvoice = false;
							((InvoiceCreateViewController)controller).txtOrderTotal.Text = Convert.ToString(Convert.ToDouble(((InvoiceCreateViewController)controller).txtOrderTotal.Text) - objItem.ItemSalePrice);
							//RMR DJ this is bad can't call async and not await it
							await AzureService.InventoryService.MarkItemSoldAsync(objItem, false);
							AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == objItem.Id).IsSold = false;
							AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == objItem.Id).AddNewFromInvoice = false;
							AzureService.InventoryService.Items.FirstOrDefault(x => x.Id == objItem.Id).RemoveFromInvoice = false;
							if (((InvoiceCreateViewController)controller).objInvoice != null)
							{
								if (((InvoiceCreateViewController)controller).objInvoice.Items != null)
									((InvoiceCreateViewController)controller).objInvoice.Items.Remove(objItem);
							}

							AzureService.InvoiceService.ItemsList.Remove(objItem);
							((InvoiceCreateViewController)controller).itemsList.Remove(objItem);
							tblView.Source = new InvoiceItemTableSource(((InvoiceCreateViewController)controller).itemsList, controller);
							tblView.ReloadData();
						}));
						((InvoiceCreateViewController)controller).PresentViewController(alert, true, null);


					}
				};
				ContentView.Add(btnCross);
			}
		}

		private UITapGestureRecognizer TapGestureToGrids(Item objItem)
		{
			UITapGestureRecognizer tap = new UITapGestureRecognizer((obj) =>
			{
				if (controller.Class.Name == "InvoiceEditViewController")
				{
					UIAlertController alert = UIAlertController.Create("Edit item", "Do you want to go to the edit item page?", UIAlertControllerStyle.Alert);
					alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
					{
					}));
					alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
					{
						InventoryDetailViewController inventoryDetailViewController = ((InvoiceEditViewController)controller).Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
						inventoryDetailViewController.InventoryItem = new InventoryNS(objItem);
						((InvoiceEditViewController)controller).NavigationController.PushViewController(inventoryDetailViewController, true);
					}));
					((InvoiceEditViewController)controller).PresentViewController(alert, true, null);
				}
				else if (controller.Class.Name == "InvoiceCreateViewController")
				{
					UIAlertController alert = UIAlertController.Create("Edit item", "Do you want to go to the edit item page?", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (actionCancel) =>
					{
					}));
					alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (actionOK) =>
					{
						InventoryDetailViewController inventoryDetailViewController = ((InvoiceCreateViewController)controller).Storyboard.InstantiateViewController("InventoryDetailViewController") as InventoryDetailViewController;
						inventoryDetailViewController.InventoryItem = new InventoryNS(objItem);
						((InvoiceCreateViewController)controller).NavigationController.PushViewController(inventoryDetailViewController, true);
					}));
					((InvoiceCreateViewController)controller).PresentViewController(alert, true, null);
				}
			});
			return tap;
		}
	}
}
