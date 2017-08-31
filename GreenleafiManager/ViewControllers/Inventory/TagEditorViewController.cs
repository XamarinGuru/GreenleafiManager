//using Foundation;
//using System;
//using System.CodeDom.Compiler;
//using UIKit;
//using Infragistics;
//using System.Linq;
//using System.Threading.Tasks;

//namespace GreenleafiManager
//{
//	partial class TagEditorViewController : UIViewController
//	{
//		public InventoryNS InventoryItem {
//			get;
//			set;
//		}

//		public TagEditorViewController (IntPtr handle) : base (handle)
//		{

//			var saveButton = new UIBarButtonItem (UIBarButtonSystemItem.Save, SaveItem);
//			saveButton.AccessibilityLabel = "saveButton";
//			NavigationItem.RightBarButtonItem = saveButton; 
//		}
//		public override void ViewDidLoad ()
//		{
//			base.ViewDidLoad ();
//			if (InventoryItem != null) {
////				var vendorCodes = InventoryService.VendorIds.Distinct().ToList().OrderBy(x=>x.VendorName);

//				VendorCodeTextField.Text = InventoryItem.GlCode;// vendorCodes.Where(x=>x.Id == InventoryItem.VendorCode).First().VendorCode;
//				ItemCodeTextField.Text = InventoryItem.GlItemCode;
//				GLCostTextField.Text = InventoryItem.GlCost.ToString ();
//				UpdateGLShortCode ();
//			}

//		}

//		async void SaveItem (object sender, EventArgs args)
//		{	
//			await AzureService.InventoryService.InsertOrSave(InventoryItem.ConvertToInventory());	
//			AzureService.InventoryService.NeedLocalRefresh = true;
//		}

//		partial void ChangeVendorCodeButton_TouchUpInside (UIButton sender)
//		{
//			UIAlertController vendorCodeAlert = UIAlertController.Create ("Vendor Code", "Select a Vendor", UIAlertControllerStyle.Alert);

//			var vendorCodes = AzureService.InventoryService.VendorIds.Distinct().ToList().OrderBy(x=>x.VendorName);

//			foreach(var rc in vendorCodes)
//			{
//				vendorCodeAlert.AddAction (UIAlertAction.Create (rc.VendorName, UIAlertActionStyle.Default, action => {

//					VendorCodeTextField.Text = rc.VendorCode;
//					InventoryItem.VendorCode = rc.VendorCode;
//					UpdateGLShortCode();
//					GLShortCodeTextField.BecomeFirstResponder();

//				}));
//			}


//			PresentViewController (vendorCodeAlert, animated: true, completionHandler: null);
//		}

//		partial void ItemCodeChangeButton_TouchUpInside (UIButton sender)
//		{
//			UIAlertController itemCodeAlert = UIAlertController.Create ("Item Code", "Select an Item Code", UIAlertControllerStyle.Alert);

//			var itemCodes = AzureService.InventoryService.ItemCodes.Distinct().ToList().OrderBy(x=>x.Value);

//			foreach(var ic in itemCodes)
//			{
//				itemCodeAlert.AddAction (UIAlertAction.Create (ic.Value, UIAlertActionStyle.Default, action => {

//					ItemCodeTextField.Text = ic.Code;
//					InventoryItem.GlItemCode = ic.Code;
//					UpdateGLShortCode();
//					GLShortCodeTextField.BecomeFirstResponder();

//				}));
//			}


//			PresentViewController (itemCodeAlert, animated: true, completionHandler: null);
//		}

//		public void UpdateGLShortCode()
//		{
//			InventoryItem.SetGLShortCode ();
//			GLShortCodeTextField.Text = InventoryItem.GlShortCode;
//		}

//		partial void GlCostEdited (UITextField sender)
//		{
//			InventoryItem.GlCost = String.IsNullOrWhiteSpace(GLCostTextField.Text) ? 0.00 : Double.Parse(GLCostTextField.Text);
//			UpdateGLShortCode ();
//			InventoryItem.SetTagPrice();
//			GLShortCodeTextField.BecomeFirstResponder();
//		}

//		partial void GlCostChanged (UITextField sender)
//		{
//			InventoryItem.GlCost = String.IsNullOrWhiteSpace(GLCostTextField.Text) ? 0.00 : Double.Parse(GLCostTextField.Text);
//			InventoryItem.SetTagPrice();
//			UpdateGLShortCode ();
//		}
//	}
//}
