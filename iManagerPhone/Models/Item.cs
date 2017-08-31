using System;
using Newtonsoft.Json;
using Foundation;
using UIKit;
using System.Linq;
using System.Collections.Generic;
using SQLite;

namespace GreenleafiManagerPhone
{

	public class Item
	{
		//public bool IsNew { get; set; }

		//[PrimaryKey, Unique]
		public string Id { get; set; }

		[JsonProperty(PropertyName = "OriginalId")]
		public string ShopifyId { get; set; }
		//public DateTime CreatedAt { get; set; }

		//public DateTime UpdatedAt { get; set; }

		[JsonProperty(PropertyName = "Description")]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "IsSold")]
		public bool IsSold { get; set; }

		[JsonProperty(PropertyName = "ShowOnWebsite")]
		public bool ShowOnWebsite { get; set; }

		[JsonProperty(PropertyName = "GlCode")]
		public string GlCode { get { return "GL"; } }

		[JsonProperty(PropertyName = "GlCost")]
		public double? GlCost { get; set; }

		[JsonProperty(PropertyName = "VendorCode")]
		public string VendorCode { get; set; }

		[JsonProperty(PropertyName = "GlItemCode")]//missingÏ
		public string GlItemCode { get; set; }

		[JsonProperty(PropertyName = "SecretCode")]
		public string SecretCode { get; set; }

		[JsonProperty(PropertyName = "TagPrice")]
		public Double TagPrice { get; set; }

		[JsonProperty(PropertyName = "InvoiceDescription")]
		public string InvoiceDescription { get; set; }

		[JsonProperty(PropertyName = "ShopifyDescription")]
		public string ShopifyDescription { get; set; }

		[JsonProperty(PropertyName = "ShopifyTitle")]
		public string ShopifyTitle { get; set; }

		[JsonProperty(PropertyName = "ShopifyPrice")]
		public Double ShopifyPrice { get; set; }

		[JsonProperty(PropertyName = "LastCheckedDate")]
		public DateTime? LastCheckedDate { get; set; }

		[JsonIgnore]
		public double ItemSalePrice
		{
			get
			{
				return Price > 0 ? Price : TagPrice;
			}
		}

		[JsonProperty(PropertyName = "Price")]
		public Double Price { get; set; }

		[JsonProperty(PropertyName = "MetalCode")]
		public string MetalCode { get; set; }

		[JsonProperty(PropertyName = "Info1")]
		public string Info1 { get; set; }

		[JsonProperty(PropertyName = "Info2")]
		public string Info2 { get; set; }

		[JsonProperty(PropertyName = "Info3")]
		public string Info3 { get; set; }

		[JsonProperty(PropertyName = "Sku")]
		public string Sku { get; set; }

		//[JsonProperty(PropertyName = "SearchString")]//missing
		[JsonIgnore]
		public string SearchString { get; set; }

		[JsonProperty(PropertyName = "LocationId")]
		public string LocationId { get; set; }

		[JsonIgnore]
		public string LocationName
		{
			get
			{
				var service = AzureService.LocationService;
				if (LocationId == null)
					return "";
				var loc = service.Locations.Where(x => x.Id == LocationId).FirstOrDefault();
				if (loc == null)
					return "";
				return loc.Name;
			}
			set
			{
				var service = AzureService.LocationService;
				var loc = service.Locations.Where(x => x.Name == value).FirstOrDefault();
				if (loc != null)
					LocationId = loc.Id;
			}
		}

		[JsonIgnore, Ignore]
		public List<ProductImage> Images { get; set; }


		public Item()
		{ 
			AddNewFromInvoice = false;
			RemoveFromInvoice = false;
		}

		public static string Reverse(string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}

		public void SetDefaultImageFilenames()
		{
			if (Images == null)
				return;
			for (int i = 0; i < Images.Count(); i++)
			{

				if (String.IsNullOrWhiteSpace(Images[i].filename))
				{

					Images[i].filename = String.Format("{0}{1}.jpg", Sku, i == 0 ? "" : "_" + i);

				}
			}
		}
		public void AddImageData()
		{
			Images = new List<ProductImage>();

			var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			int index = 0;
			bool imageFound = false;
			do
			{
				string jpgFilename = System.IO.Path.Combine(documentsDirectory, String.Format("{0}{1}.jpg", Sku, (index > 0 ? "_" + index.ToString() : ""))); // hardcoded filename, overwritten each time
				var localImage = System.IO.File.Exists(jpgFilename) ? UIImage.FromFile(jpgFilename) : null;

				if (localImage != null)
				{
					Images.Add(new ProductImage() { ImageBytes = Extensions.ToNSData(localImage), filename = String.Format("{0}{1}.jpg", Sku, (index > 0 ? "_" + index.ToString() : "")) });
					imageFound = true;
					//AzureService.InventoryService.NeedLocalRefresh = true;
				}
				else {
					imageFound = false;
				}
				index++;
			} while (imageFound);
		}
		[JsonIgnore]
		public bool AddNewFromInvoice { get; set; }
		[JsonIgnore]
		public bool RemoveFromInvoice { get; set; }


	}

	public class InventoryNS : NSObject
	{
		public InventoryNS()
		{
			Images = new List<ProductImage>();

		}

		public InventoryNS(Item inv, bool ignoreImages = false)
		{
			Id = inv.Id;
			SetDescription();
			Sold = inv.IsSold;
			GlCost = inv.GlCost.Value;
			VendorCode = inv.VendorCode;
			GlItemCode = inv.GlItemCode;
			SecretCode = inv.SecretCode;
			TagPrice = inv.TagPrice;
			MetalCode = inv.MetalCode;
			Info1 = inv.Info1;
			Info2 = inv.Info2;
			Info3 = inv.Info3;
			Sku = inv.Sku;
			InvoiceDescription = inv.InvoiceDescription;
			LastCheckedDate = inv.LastCheckedDate.HasValue ? inv.LastCheckedDate.Value.ToLocalTime().ToString("d") :null ;
			ShowInStore = inv.ShowOnWebsite;
			Location = inv.LocationName;

			ShopifyPrice = inv.ShopifyPrice;
            ShopifyDescription = inv.ShopifyDescription == null ? "" : inv.ShopifyDescription.Replace("</p><p>","\n").Replace("<p>","").Replace("</p>","");
			ShopifyTitle = inv.ShopifyTitle;
			ShopifyId = inv.ShopifyId;

			SetGLShortCode();
			SetSearchString();

			Images = new List<ProductImage>();

		}
		public override bool Equals(object obj)
		{
			if (((InventoryNS)obj).Id == Id)
				return true;
			return false;
		}
		public string ShopifyId { get; set; }
		[Export("Id")]
		public string Id { get; set; }

		[Export("DisplayDescription")]
		public string DisplayDescription { get; set; }

		[Export("Sold")]
		public bool Sold { get; set; }

		[Export("ShowInStore")]
		public bool ShowInStore { get; set; }

		string _ShopifyDescription;
		[Export("ShopifyDescription")]
		public string ShopifyDescription { 
			get
			{
			
                if (String.IsNullOrWhiteSpace(_ShopifyDescription))
				{
                    //RMR here fix item info
                    _ShopifyDescription = "";
					var code = AzureService.InventoryService.ItemCodes.Where(x => x.Code == GlItemCode).FirstOrDefault();
					if (code == null)
					{
						_ShopifyDescription = "Item " + Sku;
					}
					else
					{

                        string cleanInfo1 = ReplaceStoneCodesForDescription(Info1);
                        string cleanInfo2 = ReplaceStoneCodesForDescription(Info2);
                        string cleanInfo3 = ReplaceStoneCodesForDescription(Info3);


                        var metalCode = AzureService.InventoryService.MetalCodes.Where(x => x.Value == MetalCode).FirstOrDefault();
						if (metalCode != null)
						{
                            var metalText = metalCode.Value;
							if (!String.IsNullOrWhiteSpace(metalText) && metalText != "MetalCode")
							{
                                metalText = metalText.Replace("Y/G", "Yellow Gold").Replace("W/G", "White Gold").Replace("P/G", "Rose Gold").Replace("TT/G", "Two Tone Gold").Replace("TRI/G", "Tri Color Gold").Replace("-", " ");
								_ShopifyDescription += metalText;
                                
                            }
						}
                       

						bool cleanInfo2HasCT = cleanInfo2.Contains("CT");

						string infoTransformForTitle = "";
                        //cleanInfo1
                        if (!String.IsNullOrWhiteSpace(cleanInfo1))
                        {
                            if (cleanInfo1.Contains("CT"))
                            {
                                infoTransformForTitle = "\n" + cleanInfo1;
                            }
                            else
                            {
                                infoTransformForTitle = "\n" + StringExtensions.ToTitleCase(cleanInfo1);
                            }
                        }
                        //cleanInfo2
                        if (!String.IsNullOrWhiteSpace(cleanInfo2))
						{
							if (cleanInfo2HasCT)
							{
								if (infoTransformForTitle == "")
									infoTransformForTitle = "\n" + cleanInfo2;
								else
									infoTransformForTitle += "\n" + cleanInfo2;

							}
							else
							{
								if (infoTransformForTitle == "")
									infoTransformForTitle = "\n" + StringExtensions.ToTitleCase(cleanInfo2);
								else
									infoTransformForTitle += "\n" + StringExtensions.ToTitleCase(cleanInfo2);
							}
						}

						//cleanInfo3
						if (!String.IsNullOrWhiteSpace(cleanInfo3))
						{
							if (infoTransformForTitle == "")
								infoTransformForTitle = StringExtensions.ToTitleCase(cleanInfo3);
							else
								infoTransformForTitle += "\n" + StringExtensions.ToTitleCase(cleanInfo3);
						}

                        _ShopifyDescription += infoTransformForTitle;

						_ShopifyDescription += "\nItem Number:" + Sku;

						_ShopifyDescription += "\nIn Stock";

						_ShopifyDescription += "\nAppraisal & Box Available";

						if (code.Value.Contains("Ring") && !code.Value.Contains("Earring"))
						{
							_ShopifyDescription += "\nFinger Size";

							_ShopifyDescription += "\nIf you're unsure about your finger size, we will help you determine the right fit for you with a complimentary ring sizer.";
						}
					}
				}
				return _ShopifyDescription;

			}
			set
			{
				_ShopifyDescription = value;
			} 
		}
        private string ReplaceStoneCodesForDescription(string infoString)
        {
            string updateString = infoString;

            //Amethyst
            updateString = updateString.Replace("AMT", "Amethyst").Replace("AME", "Amethyst");
            //Asscher Cut
            updateString = updateString.Replace("ASS", "Asscher Cut").Replace("ASSCHER CUT", "Asscher Cut");
            //Aquamarine
            updateString = updateString.Replace("AQ", "Aquamarine").Replace("AQUA", "Aquamarine");
            //Baguette Cut
            updateString = updateString.Replace("BGT", "Baguette Cut");
            //Blue Topaz
            updateString = updateString.Replace("BTP", "Blue Topaz");
            //Amethyst
            updateString = updateString.Replace("BZ", "Blue Zircon");
            //Black White
            updateString = updateString.Replace("BL/WH", "Black White");
            //Citrine
            updateString = updateString.Replace("CITRINE", "Citrine");
            //Diamond
            updateString = updateString.Replace("DIA", "Diamond");
            //Big Diamond
            updateString = updateString.Replace("BG DIA", "Big Diamond").Replace("BIGDIA", "Big Diamond").Replace("B/DIA", "Big Diamond").Replace("BIG", "Big Diamond");
            //Small Diamond
            updateString = updateString.Replace("SM DIA", "Small Diamond").Replace("SMALLDIA", "Small Diamond").Replace("S/DIA", "Small Diamond").Replace("SMALL", "Small Diamond");
            //Yellow Diamond
            updateString = updateString.Replace("YLW/D", "Yellow Diamond").Replace("Y/D", "Yellow Diamond");
			//White Diamond
			updateString = updateString.Replace("WHT/D", "White Diamond").Replace("W/D", "White Diamond");
			//Black Diamond
			updateString = updateString.Replace("BLK/D", "Black Diamond").Replace("BLK/DIA", "Black Diamond").Replace("B/D", "Black Diamond").Replace("BLKDIA", "Black Diamond").Replace("BLKD", "Black Diamond");
			//Emerald
			updateString = updateString.Replace("EM", "Emerald").Replace("EMER", "Emerald").Replace("EMERALD", "Emerald");
            //Gold
            updateString = updateString.Replace(" G ", " Gold ").Replace("GOLD", "Gold");
            //Gold Nugget
            updateString = updateString.Replace("GN", "Gold Nugget").Replace("G/NUG", "Gold Nugget");
            //Morganite
            updateString = updateString.Replace("MOR", "Morganite");
            //Peridot
            updateString = updateString.Replace("PERID", "Peridot");
            //Ruby
            updateString = updateString.Replace("RUBY", "Ruby").Replace("R/B", "Ruby").Replace("RUBIES", "Ruby");
            //Sapphire
            updateString = updateString.Replace("SAP", "Sapphire");
            //Tri Color Gold
            updateString = updateString.Replace("TRI/GOLD", "Tri Color Gold").Replace("TRI/GOLD", "Tri Color Gold");
            //Tanzanite
            updateString = updateString.Replace("TZ", "Tanzanite").Replace("TTZ", "Tanzanite");
            //Tourmaline
            updateString = updateString.Replace("TM", "Tourmaline");
            //Topaz
            updateString = updateString.Replace("TP", "Topaz");
            //Zircon
            updateString = updateString.Replace("ZR", "Zircon");


            return updateString;
        }
        public string ShopifyDescriptionHTML
		{
            get
            {
                var html = "<p>";
                html += _ShopifyDescription == null ? "" : _ShopifyDescription.Replace("\n", "</p><p>");
                html += "</p>";
                return html;
            }
		}
		string _ShopifyTitle;
		[Export("ShopifyTitle")]
		public string ShopifyTitle
		{
			get
			{
				if (String.IsNullOrWhiteSpace(_ShopifyTitle))
				{
					var code = AzureService.InventoryService.ItemCodes.Where(x => x.Code == GlItemCode).FirstOrDefault();
					if (code == null)
					{
						_ShopifyTitle = "Item " + Sku;
					}
					else
					{
                        bool info2HasCT = this.Info2.Contains("CT");

                        string infoTransformForTitle = "";
                        //Info1
                        if (!String.IsNullOrWhiteSpace(Info1))
                        {
                            //if (info2HasCT)
                            //{
                            //    infoTransformForTitle = Info1;
                            //}
                            //else
                                if (Info1.Contains("CT"))
                            {
                                infoTransformForTitle = Info1.Substring(0, Info1.IndexOf("CT") + 2);
                            }
                            else
                            {
                                infoTransformForTitle = StringExtensions.ToTitleCase(Info1);
                            }
                        }

                        infoTransformForTitle += " " + code.Value;

                        //Info2
                        if (!String.IsNullOrWhiteSpace(Info2))
                        {
                            if (info2HasCT)
                            {
                                //string CTWeight = Info2.Substring(0, Info2.IndexOf("CT") + 2);
                                //string type = StringExtensions.ToTitleCase(Info2.Substring(Info2.IndexOf("CT") + 2));
                                //if (infoTransformForTitle == "")
                                //    infoTransformForTitle = CTWeight + type;
                                //else
                                //infoTransformForTitle += ", " + CTWeight + type;
                                if (infoTransformForTitle == "")
                                    infoTransformForTitle = Info2;
                                else
                                    infoTransformForTitle += " " + Info2;

                            }
                            else
                            {
                                if (infoTransformForTitle == "")
                                    infoTransformForTitle = StringExtensions.ToTitleCase(Info2);
                                else
                                    infoTransformForTitle += " " + StringExtensions.ToTitleCase(Info2);
                            }
                        }

                        //Info3
                        if (!String.IsNullOrWhiteSpace(Info3))
                        {
                            if (infoTransformForTitle == "")
                                infoTransformForTitle = StringExtensions.ToTitleCase(Info3);
                            else
                                infoTransformForTitle += " " + StringExtensions.ToTitleCase(Info3);
                        }
                        _ShopifyTitle = infoTransformForTitle;
                        
					}
				}
				return _ShopifyTitle;

			}
			set
			{
				_ShopifyTitle = value;
			}
		}

		Double _ShopifyPrice;

		[Export("ShopifyPrice")]
		public Double ShopifyPrice
		{
			get
			{
				if (_ShopifyPrice <= 0)
				{
					Math.Round(_ShopifyPrice = TagPrice * 0.35, 0);
				}
				return _ShopifyPrice;

			}
			set
			{
				_ShopifyPrice = value;
			}
		}
					

		[Export("GlCode")]
		public string GlCode { get { return "GL"; } }

		[Export("GlCost")]
		public Double GlCost { get; set; }

		[Export("VendorCode")]
		public string VendorCode { get; set; }

		[Export("GlItemCode")]
		public string GlItemCode { get; set; }

		[Export("glshortcode")]
		public string GlShortCode { get; set; }

		[Export("SecretCode")]
		public string SecretCode { get; set; }

		[Export("TagPrice")]
		public Double TagPrice { get; set; }

		[JsonIgnore]
		public Double SalePrice { get; set; }

		[Export("MetalCode")]
		public string MetalCode { get; set; }

		[Export("Info1")]
		public string Info1 { get; set; }

		[Export("Info2")]
		public string Info2 { get; set; }

		[Export("Info3")]
		public string Info3 { get; set; }

		[Export("Sku")]
		public string Sku { get; set; }

			[Export("Location")]
		public string Location { get; set; }

		[Export("LastCheckedDate")]
		public string LastCheckedDate { get; set; }

		[Export("SearchString")]
		public string SearchString { get; set; }

		[Export("InvoiceDescription")]
		public string InvoiceDescription { get; set; }

		public List<ProductImage> Images { get; set; }

		[Export("ThumbnailImage")]
		public UIImage ThumbnailImage
		{
			get
			{
				if (Images != null && Images.Count > 0)
				{
					ProductImage image = Images.Where(x => x.position == "1").FirstOrDefault();
					if (image == null)
					{
						image = Images.First();
					}
					return Extensions.ToImage(image.ImageBytes);
				}
				else {
					var defaultImage = UIImage.FromFile("Images/no-thumb.png");
					return defaultImage;
				}

			}
		}
		private List<GridImage> _GridImages;
		public List<GridImage> GridImages
		{
			get
			{
				if (_GridImages == null)
				{
					LoadImagesFromItem();
					ReorderGridItems();
				}
				return _GridImages;
			}
		}
		public void ReorderGridItems()
		{
			_GridImages = _GridImages.OrderBy(x => x.SortOrder).ToList();
		}
		public void LoadImagesFromItem()
		{
			_GridImages = new List<GridImage>();
			Images = AzureService.InventoryService.GetImagesFromAzure(Sku);
			if (Images == null)
				return;
			foreach (var pi in Images)
			{
				GridImage gi = new GridImage();
				gi.Image = Extensions.ToImage(pi.ImageBytes);
				gi.OriginalId = pi.OriginalId;
				gi.PictureName = pi.PictureName;
				gi.SortOrder = pi.position == "-1" ? (_GridImages.Count+1).ToString() : pi.position;
				_GridImages.Add(gi);
			}

		}

		public void SetDescription()
		{
			string result = AzureService.InventoryService.MetalCodes.Where(c => c.Code == MetalCode).Any() ? AzureService.InventoryService.MetalCodes.Where(c => c.Code == MetalCode).First().Value : MetalCode;
			result = String.Format("{0} {1}", result, (AzureService.InventoryService.ItemCodes.Where(c => c.Code == GlItemCode).Any() ? AzureService.InventoryService.ItemCodes.Where(c => c.Code == GlItemCode).First().Value : GlItemCode));
			result = String.Format("{0} {1}", result, Info1);
			result = String.Format("{0} {1}", result, Info2);
			result = String.Format("{0} {1}", result, Info3);
			result = String.Format("{0} {1}", result, Sku);
			DisplayDescription = result;
		}

		public void SetSearchString()
		{

			SetDescription();

			this.SearchString = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}",
				DisplayDescription,
				(AzureService.InventoryService.GLCodes.Where(c => c.Code == GlCode).Any() ? AzureService.InventoryService.GLCodes.Where(c => c.Code == GlCode).First().Value : GlCode),
				GlCost,
				(AzureService.InventoryService.ItemCodes.Any(c => c.Code == GlItemCode) ? AzureService.InventoryService.ItemCodes.Where(c => c.Code == GlItemCode).First().Value : GlItemCode),
				(AzureService.InventoryService.VendorIds.Where(c => c.VendorCode == VendorCode).Any() ? AzureService.InventoryService.VendorIds.Where(c => c.VendorCode == VendorCode).First().VendorName : VendorCode),
				SecretCode,
				TagPrice,
				(AzureService.InventoryService.MetalCodes.Where(c => c.Code == MetalCode).Any() ? AzureService.InventoryService.MetalCodes.Where(c => c.Code == MetalCode).First().Value : MetalCode),
				Info1,
				Info2,
				Info3,
				Sku,
                GlItemCode);
		}

		public Item ConvertToInventory()
		{
			Item inv = new Item();
			inv.Id = Id;
			SetDescription();
			inv.Description = DisplayDescription;
			inv.IsSold = Sold;
			inv.GlCost = GlCost;
			inv.VendorCode = VendorCode;
			inv.GlItemCode = GlItemCode;
			inv.SecretCode = SecretCode;
			inv.TagPrice = TagPrice;
			inv.MetalCode = MetalCode;
			inv.Info1 = Info1;
			inv.Info2 = Info2;
			inv.Info3 = Info3;
			inv.Sku = Sku;
			inv.SearchString = SearchString;
			inv.Images = Images;
			inv.InvoiceDescription = InvoiceDescription;
			inv.LocationName = Location;
			inv.ShopifyId = ShopifyId;
			inv.ShopifyPrice = ShopifyPrice;
			inv.ShopifyDescription = ShopifyDescriptionHTML;
			inv.ShopifyTitle = ShopifyTitle;
			inv.ShowOnWebsite = ShowInStore;

			return inv;
		}

		public void SetGLShortCode()
		{
			GlShortCode = String.Format("{0} {1}{2}", this.GlCode, Reverse(this.GlCost.ToString()), GlItemCode);
		}
		public void SetTagPrice()
		{
			TagPrice = Math.Ceiling(GlCost * 7.8);
		}
		public static string Reverse(string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}



	}

	public class ItemCode
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "Code")]
		public string Code { get; set; }

		[JsonProperty(PropertyName = "Value")]
		public string Value { get; set; }

		[JsonProperty(PropertyName = "Category")]
		public string Category { get; set; }

		[JsonProperty(PropertyName = "ShopifyTags")]
		public string ShopifyTags { get; set; }
	}

	public class StoneCode
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "Code")]
		public string Code { get; set; }

		[JsonProperty(PropertyName = "Value")]
		public string Value { get; set; }
	}

	public class MetalCode
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "Code")]
		public string Code { get; set; }

		[JsonProperty(PropertyName = "Value")]
		public string Value { get; set; }
	}

	public class GLCode
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "Code")]
		public string Code { get; set; }

		[JsonProperty(PropertyName = "Value")]
		public string Value { get; set; }
	}

	public class VendorId
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "VendorCode")]
		public string VendorCode { get; set; }

		[JsonProperty(PropertyName = "VendorName")]
		public string VendorName { get; set; }
	}
	public class GridImage
	{
		public string OriginalId
		{
			get;
			set;
		}
		public UIImage Image { get; set; }
		public string SortOrder
		{
			get;
			set;
		}

		public string PictureName { get; set; }
	}

	public class ItemsPictures
	{
		public ItemsPictures()
		{
			Id = Guid.NewGuid();
			MainPic = false;
		}

		public string PictureURL { get; set; }

		public string PictureName { get; set; }
		public Guid Id { get; set; }
		public string OriginalId { get; set; }
		public string ShopifyImageId { get; set; }
		public bool MainPic { get; set; }
		public string position { get; set; }
	}

	public class PicturePost
	{
		public string itemId { get; set; }
		public string picturebase64 { get; set; }
	}
	public class PicturePositionPut
	{
		public string pictureName { get; set; }
		public string position { get; set; }
	}
	public class PictureDelete
	{
		public string pictureBlobName { get; set; }
	}
	//this should go in a common class
	//RMR TODO

	public static class StringExtensions
	{
		public static string ToTitleCase(this string s)
		{

			var upperCase = s.ToUpper();
			var words = upperCase.Split(' ');

			var minorWords = new String[] {"ON", "IN", "AT", "OFF", "WITH", "TO", "AS", "BY",//prepositions
                                   "THE", "A", "OTHER", "ANOTHER",//articles
                                   "AND", "BUT", "ALSO", "ELSE", "FOR", "IF"};//conjunctions

			var acronyms = new String[] {"UK", "USA", "US",//countries
                                   "BBC",//TV stations
                                   "TV", "MM","S/DIA"};//others

			//The first word.
			//The first letter of the first word is always capital.
			if (acronyms.Contains(words[0]))
			{
				words[0] = words[0].ToUpper();
			}
			else
			{
				words[0] = words[0].ToPascalCase();
			}

			//The rest words.
			for (int i = 0; i < words.Length; i++)
			{
				if (minorWords.Contains(words[i]))
				{
					words[i] = words[i].ToLower();
				}
				else if (acronyms.Contains(words[i]))
				{
					words[i] = words[i].ToUpper();
				}
				else
				{
					words[i] = words[i].ToPascalCase();
				}
			}

			return string.Join(" ", words);

		}

		public static string ToPascalCase(this string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
			}
			else
			{
				return String.Empty;
			}
		}
	}
}
