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
    public class DeleteDuplicatedItemsGLWorkerService
    {
        private readonly ShopifyAdapter _shopifyAdapter;
        private readonly GreenLeafMobileContext _db;

        public DeleteDuplicatedItemsGLWorkerService(ShopifyAdapter shopifyAdapter, GreenLeafMobileContext db)
        {
            _shopifyAdapter = shopifyAdapter;
            _db = db;
        }
        public async Task Execute()
        {
            try
            {
                var ToDelete = new List<Item>();
                //var localDbItems = await _db.Items.Where(x => x.Sku == "123471").ToListAsync();
                var localDbItems = await _db.Items.ToListAsync();
                var shopifyItems = await _shopifyAdapter.GetInventories();

                for (int i = 0; i < localDbItems.Count; i++)
                {
                    if (localDbItems.Count(x => x.Sku == localDbItems[i].Sku) > 1)
                    {
                        if (!shopifyItems.Any(x => x.OriginalId.ToString() == localDbItems[i].OriginalId))
                            if (ToDelete.Count(x => x.Sku == localDbItems[i].Sku)
                                != localDbItems.Count(x => x.Sku == localDbItems[i].Sku) - 1)
                                ToDelete.Add(localDbItems[i]);
                    }
                }
                _db.Items.RemoveRange(ToDelete);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
