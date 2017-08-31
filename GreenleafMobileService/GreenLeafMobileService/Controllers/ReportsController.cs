using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using GreenLeafMobileService.Helper;
using Microsoft.Azure.Mobile.Server;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using System.Linq;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using ShopifyClient.Models;

namespace GreenLeafMobileService.Controllers
{
    public class ReportsController : TableController<Item>//<Location>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            //var context = new GreenLeafMobileContext();
            //DomainManager = new EntityDomainManager<Item>(context, Request);//, Services );
        }
        [HttpGet]
        public async Task<CurrentStockReportModel> CurrentStockReport(string locationId)
        {
            var context = new GreenLeafMobileContext();
            var list = await context.Items.Where(x => x.LocationId == locationId && !x.IsSold).ToListAsync();

            var totalofItems = list.Count;
            var totalvaluesofItems = list.Sum(x => x.TagPrice);

            return new CurrentStockReportModel(totalofItems, totalvaluesofItems);
        }
        [HttpGet]
        public async Task<SalesReportModel> SalesReport(string startDate, string endDate, string userId, string locationId = null)
        {
            var srm = new SalesReportModel();
            try
            {
                DateTime parsedStartDate;
                DateTime parsedEndDate;

                DateTime.TryParse(startDate, out parsedStartDate);
                DateTime.TryParse(endDate, out parsedEndDate);
                if (parsedEndDate == null || parsedStartDate == null)
                {
                    srm.Errors = "Invalid Dates";
                    return srm;
                }
                
                var context = new GreenLeafMobileContext();
                
                var invoiceList = locationId == null ?
                    await context.Invoices.Where(x => (x.Date.Contains(parsedStartDate.Year.ToString()) || x.Date.Contains(parsedEndDate.Year.ToString())) && x.UserId == userId).ToListAsync() :
                    await context.Invoices.Where(x => x.LocationId == locationId && (x.Date.Contains(parsedStartDate.Year.ToString()) || x.Date.Contains(parsedEndDate.Year.ToString())) && x.UserId == userId).ToListAsync();
                List<Invoice> filteredInvoices = new List<Invoice>();
                foreach (var i in invoiceList)
                {
                    DateTime iStartDate;
                    DateTime iEndDate;

                    DateTime.TryParse(startDate, out iStartDate);
                    DateTime.TryParse(endDate, out iEndDate);
                    if (iEndDate != null && iStartDate != null)
                    {
                        if(iStartDate >= parsedStartDate && iEndDate <= parsedEndDate)
                        {
                            filteredInvoices.Add(i);
                        }
                    }
                }

                srm.StartDate = parsedStartDate;
                srm.EndDate = parsedEndDate;
                var salesPerson = context.Users.Where(x => x.Id == userId).FirstOrDefault();
                srm.SalesPersonName = salesPerson == null ? "" : salesPerson.FirstName + salesPerson.LastName;
                srm.TotalIncludingTax = filteredInvoices.Sum(x => x.OrderTotal);

                var invoiceIds = filteredInvoices.Select(x => x.Id).ToList();
                var invoiceItemsList = await context.InvoiceItems.Where(x => x.Invoice_Id != null && invoiceIds.Contains(x.Invoice_Id)).ToListAsync();
                var itemIds = invoiceItemsList.Select(x => x.Item_Id).ToList();
                var itemList = context.Items.Where(x => itemIds.Contains(x.Id)).ToList();
                srm.TotalCost = itemList.Sum(x => x.GlCost.Value);
                srm.TotalSalePrice = itemList.Sum(x => x.Price);
                srm.ItemsSold = itemList.Count();
                srm.TotalTax = 0;
                foreach (var i in filteredInvoices)
                {
                    srm.TotalTax += GetTaxForInvoice(i);
                }
                srm.TotalIncludingTax = srm.TotalSalePrice + srm.TotalTax;

            }
            catch(Exception ex)
            {
                srm.Errors = ex.Message;
            }
            return srm;
        }
        private decimal GetTaxForInvoice(Invoice invoice)
        {
            var context = new GreenLeafMobileContext();
            decimal tax = 0;
            var cust = context.Customers.Where(x => x.Id == invoice.CustomerId).FirstOrDefault();
            var loc = context.Locations.Where(x => x.Id == invoice.LocationId).FirstOrDefault();
          
            if (cust == null || loc == null)
            {
                return 0;
            }
            if (invoice.IsMailOrder && loc.Name == "LA" && cust != null && cust.Province != null &&
                (cust.Province.Trim().Replace(".", "").ToLower() != "ca" && cust.Province.Trim().ToLower() != "california"))
            {
                return 0;
            }

            decimal total = invoice.OrderTotal;
            decimal taxRate = Convert.ToDecimal(loc.Tax);
            tax = total * taxRate;
            tax = Math.Round(tax, 2);

            return tax;
        }
        [HttpGet]
        public async Task<string> GetNextSKU()
        {
            List<Item> items = new List<DataObjects.Item>();
            using (var context = new GreenLeafMobileContext())
            {
                items = await context.Items.ToListAsync();
            }
            var maxTopSku = (from max in items
                             where !String.IsNullOrEmpty(max.Sku)
                             select Convert.ToInt32(max.Sku)).Max();
            var nextSku = maxTopSku + 1;
            return nextSku.ToString();
        }

        [HttpGet]
        public async Task<Item> GetItemBySku(string Sku)
        {
            try
            {
                List<Item> items = new List<DataObjects.Item>();
                using (var context = new GreenLeafMobileContext())
                {
                    items = await context.Items.ToListAsync();
                }
                Item item = items.Where(x => x.Sku == Sku).FirstOrDefault();

                return item;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<Item> SaveOrUpdateItem(Item item)
        {
            try
            {
                using (var context = new GreenLeafMobileContext())
                {
                    List<Item> items = new List<DataObjects.Item>();
                    items = await context.Items.ToListAsync();

                    Item foundItem = items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (foundItem != null)
                    {
                        foundItem.GlCode = item.GlCode;
                        foundItem.GlCost = item.GlCost;
                        foundItem.GlItemCode = item.GlItemCode;
                        foundItem.Info1 = item.Info1;
                        foundItem.Info2 = item.Info2;
                        foundItem.Info3 = item.Info3;
                        foundItem.InvoiceDescription = item.InvoiceDescription;
                        foundItem.InvoiceId = item.InvoiceId;
                        foundItem.IsSold = item.IsSold;
                        foundItem.LocationId = item.LocationId;
                        foundItem.MetalCode = item.MetalCode;
                        foundItem.OriginalId = item.OriginalId;
                        foundItem.Price = item.Price;
                        foundItem.SecretCode = item.SecretCode;
                        foundItem.Sku = item.Sku;
                        foundItem.TagPrice = item.TagPrice;
                        foundItem.UpdatedAt = DateTime.UtcNow;
                        //foundItem.VariantId = item.VariantId;
                        foundItem.VendorCode = item.VendorCode;
                        foundItem.LastCheckedDate = DateTime.UtcNow;
                        foundItem.ShopifyDescription = item.ShopifyDescription;
                        foundItem.ShopifyTitle = item.ShopifyTitle;
                        foundItem.ShopifyPrice = item.ShopifyPrice;
                        foundItem.OriginalId = await UpdateShopifyItem(foundItem);
                        context.SaveChanges();
                        item = foundItem; 
                    }
                    else
                    {
                        item.LastCheckedDate = DateTime.UtcNow;
                        item.Id = Guid.NewGuid().ToString();
                        var newShopifyItem = await SaveItemToShopify(item);
                        item.OriginalId = newShopifyItem.OriginalId;
                        item.VariantId = newShopifyItem.VariantId;
                        context.Items.Add(item);
                        context.SaveChanges();
                        items = await context.Items.ToListAsync();
                        item = items.Where(x => x.Id == item.Id).FirstOrDefault();
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private async Task<Item> SaveItemToShopify(Item item)
        {
            try
            {
                var _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
                // update the item show in storage
                var shopifyInventory = Converter.Convert(item);
                shopifyInventory.Tags = GetShopifyTags(item);
                shopifyInventory.Variants = new List<Variant> { Converter.ConvertToVariant(item) };
                var createdInventory = await _shopifyAdapter.CreateInventory(shopifyInventory);
                //createdInventory.ParseInventoryTags();
                var itemToInsert = Converter.Convert(createdInventory);
                return itemToInsert;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetShopifyTags(Item item)
        {
            try
            {
                using (var context = new GreenLeafMobileContext())
                {

                    string sTags = context.ItemCodes.Where(x => x.Code == item.GlItemCode).Select(y => y.ShopifyTags).FirstOrDefault();
                    return sTags;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private async Task<string> UpdateShopifyItem(Item updatedItem)
        {
            try
            {
                var _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
                // update the item show in storage
                var shopifyInventory = Converter.Convert(updatedItem);
                shopifyInventory.Tags = GetShopifyTags(updatedItem);//GenerateInventoryTags();
                var updatedShopifyItem = await _shopifyAdapter.UpdateInventory(shopifyInventory);
                if (updatedShopifyItem.Variants != null && updatedShopifyItem.Variants.FirstOrDefault() != null)
                {
                    var vPrice = updatedShopifyItem.Variants.First().Price;
                    if ( vPrice != updatedItem.ShopifyPrice)
                    {
                        VariantRoot varRoot = new VariantRoot();
                        varRoot.variant = Converter.ConvertToVariant(updatedItem);
                        var varId = await UpdateShopifyVariant(varRoot);
                        if (varId == "Error 404")
                        {
                            var newVarId = await AddShopifyVariant(updatedItem);
                            updatedItem.VariantId = newVarId;
                        }

                    }
                }
                else
                {
                    var newVarId = await AddShopifyVariant(updatedItem);
                    updatedItem.VariantId = newVarId;
                }
                return updatedShopifyItem.OriginalId.HasValue ? updatedShopifyItem.OriginalId.ToString() : "";

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404"))
                {
                    updatedItem.OriginalId = null;
                    var newShopifyItem = await SaveItemToShopify(updatedItem);
                    updatedItem.OriginalId = newShopifyItem.OriginalId;
                    updatedItem.VariantId = newShopifyItem.VariantId;
                    return updatedItem.OriginalId;
                }
                throw ex;
            }

        }
        private async Task<string> UpdateShopifyVariant(VariantRoot variantRoot)
        {
            try
            {
                var _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
                var variantId = await _shopifyAdapter.UpdateVariant(variantRoot);
                return variantId;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404"))
                {
                    return "Error 404";
                }
                throw ex;
            }

        }
        private async Task<string> AddShopifyVariant(Item item)
        {
            var _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
            VariantRoot varRoot = new VariantRoot();
            varRoot.variant = Converter.ConvertToVariant(item);
            var variantId = await _shopifyAdapter.AddVariant(varRoot);
            return variantId;
        }

    }
}
