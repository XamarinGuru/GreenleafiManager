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
    public class UpdatePriceGLWorkerService
    {
        private readonly ShopifyAdapter _shopifyAdapter;
        private readonly GreenLeafMobileContext _db;

        public UpdatePriceGLWorkerService(ShopifyAdapter shopifyAdapter, GreenLeafMobileContext db)
        {
            _shopifyAdapter = shopifyAdapter;
            _db = db;
        }

        private decimal calculateDiferenceOnShopifyPrice(Item item)
        {
            return Math.Round(item.TagPrice * (decimal)0.35, 2);
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
}
