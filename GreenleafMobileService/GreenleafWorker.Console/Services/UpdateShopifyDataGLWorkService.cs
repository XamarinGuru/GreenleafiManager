using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Helper;
using GreenLeafMobileService.Models;
using ShopifyClient;
using ShopifyClient.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GreenleafWorker.Console.Services
{
    public class UpdateShopifyDataGLWorkerService
    {
        private readonly ShopifyAdapter _shopifyAdapter;
        private readonly GreenLeafMobileContext _db;

        public UpdateShopifyDataGLWorkerService(ShopifyAdapter shopifyAdapter, GreenLeafMobileContext db)
        {
            _shopifyAdapter = shopifyAdapter;
            _db = db;
        }

        private decimal calculateDiferenceOnShopifyPrice(Item item)
        {
            return Math.Round(item.TagPrice * (decimal)0.35, 0);
        }
        private string CalculateDefaultShopifyTitle(Item item)
        {
            string infoTransformForTitle = "";

            var code = _db.ItemCodes.Where(x => x.Code == item.GlItemCode).FirstOrDefault();
            if (code == null)
            {
                infoTransformForTitle = "Item " + item.Sku;
            }
            else
            {
                bool info2HasCT = item.Info2.Contains("CT");


                //Info1
                if (!String.IsNullOrWhiteSpace(item.Info1))
                {
                    if (info2HasCT)
                    {
                        infoTransformForTitle = item.Info1;
                    }
                    else if (item.Info1.Contains("CT"))
                    {
                        infoTransformForTitle = item.Info1.Substring(0, item.Info1.IndexOf("CT") + 2);
                    }
                    else
                    {
                        infoTransformForTitle = StringExtensions.ToTitleCase(item.Info1);
                    }
                }

                infoTransformForTitle += code.Value;

                //Info2
                if (!String.IsNullOrWhiteSpace(item.Info2))
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
                            infoTransformForTitle = item.Info2;
                        else
                            infoTransformForTitle += " " + item.Info2;

                    }
                    else
                    {
                        if (infoTransformForTitle == "")
                            infoTransformForTitle = StringExtensions.ToTitleCase(item.Info2);
                        else
                            infoTransformForTitle += " " + StringExtensions.ToTitleCase(item.Info2);
                    }
                }

                //Info3
                if (!String.IsNullOrWhiteSpace(item.Info3))
                {
                    if (infoTransformForTitle == "")
                        infoTransformForTitle = StringExtensions.ToTitleCase(item.Info3);
                    else
                        infoTransformForTitle += " " + StringExtensions.ToTitleCase(item.Info3);
                }

            }
            return infoTransformForTitle;
        }
        private string CalculateDefaultShopifyDescription(Item item)
        {
            string defaultValue = "";
            defaultValue = "";
            var code = _db.ItemCodes.Where(x => x.Code == item.GlItemCode).FirstOrDefault();
            if (code == null)
            {
                defaultValue = "Item " + item.Sku;
            }
            else
            {
                var metalCode = _db.MetalCodes.Where(x => x.Value == item.MetalCode).FirstOrDefault();
                if (metalCode != null)
                {
                    var metalText = metalCode.Value;
                    if (!String.IsNullOrWhiteSpace(metalText) && metalText != "MetalCode")
                    {
                        metalText = metalText.Replace("Y/G", "Yellow Gold").Replace("W/G", "White Gold").Replace("P/G", "Rose Gold").Replace("TT/G", "Two Tone Gold").Replace("TRI/G", "Tri Color Gold").Replace("-", " ");
                        defaultValue += metalText;

                    }
                }


                bool info2HasCT = item.Info2.Contains("CT");

                string infoTransformForTitle = "";
                //Info1
                if (!String.IsNullOrWhiteSpace(item.Info1))
                {
                    if (item.Info1.Contains("CT"))
                    {
                        infoTransformForTitle = "\n" + item.Info1;
                    }
                    else
                    {
                        infoTransformForTitle = "\n" + StringExtensions.ToTitleCase(item.Info1);
                    }
                }
                //Info2
                if (!String.IsNullOrWhiteSpace(item.Info2))
                {
                    if (info2HasCT)
                    {
                        if (infoTransformForTitle == "")
                            infoTransformForTitle = "\n" + item.Info2;
                        else
                            infoTransformForTitle += "\n" + item.Info2;

                    }
                    else
                    {
                        if (infoTransformForTitle == "")
                            infoTransformForTitle = "\n" + StringExtensions.ToTitleCase(item.Info2);
                        else
                            infoTransformForTitle += "\n" + StringExtensions.ToTitleCase(item.Info2);
                    }
                }

                //Info3
                if (!String.IsNullOrWhiteSpace(item.Info3))
                {
                    if (infoTransformForTitle == "")
                        infoTransformForTitle = StringExtensions.ToTitleCase(item.Info3);
                    else
                        infoTransformForTitle += ", " + StringExtensions.ToTitleCase(item.Info3);
                }

                defaultValue += infoTransformForTitle;

                defaultValue += "\nItem Number:" + item.Sku;

                defaultValue += "\nIn Stock";

                defaultValue += "\nAppraisal & Box Available";

                if (code.Value.Contains("Ring") && !code.Value.Contains("Earring"))
                {
                    defaultValue += "\nFinger Size";

                    defaultValue += "\nIf you're unsure about your finger size, we will help you determine the right fit for you with a complimentary ring sizer.";
                }
            }

            return defaultValue;
        }
        private string CalculateShopifyHTML(string description)
        {
            var html = "<p>";
            html += description == null ? "" : description.Replace("\n", "</p><p>");
            html += "</p>";
            return html;
        }

        public async Task Execute()
        {
            try
            {
                //var localDbItems = await _db.Items.Where(x => x.Sku == "123471").ToListAsync();
                var localDbItems = await _db.Items.ToListAsync();
                var shopifyItems = await _shopifyAdapter.GetInventories();
                for (int i = 0; i < localDbItems.Count(); i++)
                {
                    if (shopifyItems.Any(x => localDbItems[i].OriginalId == x.OriginalId.ToString()))
                    {
                        localDbItems[i].ShopifyPrice = calculateDiferenceOnShopifyPrice(localDbItems[i]);
                        localDbItems[i].ShopifyDescription = CalculateDefaultShopifyDescription(localDbItems[i]);
                        localDbItems[i].ShopifyTitle = CalculateDefaultShopifyTitle(localDbItems[i]);
                        //RMR is html being updated correctly? 

                        var tempString = localDbItems[i].ShopifyTitle;
                        var tempString2 = localDbItems[i].ShopifyDescription;

                        var shopifyItem = shopifyItems.First(x => localDbItems[i].OriginalId == x.OriginalId.ToString());
                        if (shopifyItem.Variants.Any())
                            shopifyItem.Variants.First().Price = localDbItems[i].ShopifyPrice;
                        else
                            shopifyItems.First(x => localDbItems[i].OriginalId == x.OriginalId.ToString()).Variants = new List<Variant> { Converter.ConvertToVariant(localDbItems[i]) };

                        _db.Items.Attach(localDbItems[i]);
                        _db.Entry(localDbItems[i]).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                        await _shopifyAdapter.UpdateInventory(shopifyItem);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
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
