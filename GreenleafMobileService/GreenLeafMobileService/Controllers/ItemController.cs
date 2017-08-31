using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using AutoMapper;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Models;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using ShopifyClient.Models;
using Microsoft.Azure.Mobile.Server;
using System.Threading;
using System;
using GreenLeafMobileService.Helper;
using System.Data.SqlClient;

namespace GreenLeafMobileService.Controllers {
    //[AuthorizeLevel ( AuthorizationLevel.User )]
    public class ItemController : TableController<Item> {
        //private readonly IMapper _mapper;
        private readonly ShopifyAdapter _shopifyAdapter;

        public ItemController()
        {
            try {
                _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public ItemController ( IMapper mapper ) {
            _shopifyAdapter = new ShopifyAdapter(new JsonDataTranslator());
        }

        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new GreenLeafMobileContext();
            DomainManager = new EntityDomainManager<Item>(context, Request);//, Services );
        }
        
        // GET tables/Item
        public IQueryable<Item> GetAllItems () {
            var q = Query();
            try
            {
                //var updateTask = UpdateAllDBItems(q);                
                //updateTask.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return q;
            
        }

        // GET tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Item> GetItem ( string id ) {
            
            return Lookup( id );
        }

        // PATCH tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<Item> PatchItem ( string id, Delta<Item> patch ) {
            try {

                Item item = null;
                using (var context = new GreenLeafMobileContext())
                {
                    item = context.Items.Where(x => x.Id == id).FirstOrDefault();

                    if (item == null)
                        return item;
                    if (!String.IsNullOrWhiteSpace(item.OriginalId) && item.OriginalId != "0")
                    {
                        item = await UpdateAsync(id, patch);
                        await UpdateShopifyItem(item);
                    }
                    else
                    {
                        item = await SaveItemToShopify(item);
                        context.Items.Where(x => x.Id == id).FirstOrDefault().OriginalId = item.OriginalId;
                        item = await UpdateAsync(id, patch);
                    }
                }
                return item;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // POST tables/Item
        public async Task<IHttpActionResult> PostItem ( Item item ) {
            try
            {
                var itemToInsert = await SaveItemToShopify(item);
                itemToInsert.LastCheckedDate = DateTime.UtcNow;
                var current = await InsertAsync(itemToInsert);
                return CreatedAtRoute("Tables", new { id = current.Id }, current);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // DELETE tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteItem ( string id ) {
            return DeleteAsync( id );
        }
        private async Task UpdateAllDBItems(IQueryable<Item> q)
        {
            try
            {
                var items = q.Where(x => !x.Deleted).ToList();
                foreach (var i in items)
                {
                    var itemInserted = await SaveItemToShopify(i);
                    UpdateShopifyId(itemInserted);
                    Thread.Sleep(1000);                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void UpdateShopifyId(Item item)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager
                              .ConnectionStrings["MS_TableConnectionString"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("Update Items SET [OriginalId] = '{0}' WHERE ID = '{1}'", item.OriginalId, item.Id);

                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<Item> SaveItemToShopify ( Item item ) {
            try {
                // update the item show in storage
                var shopifyInventory = Converter.Convert(item);
                shopifyInventory.Tags = GetShopifyTags(item);//.GenerateInventoryTags();
                shopifyInventory.Variants = new List<Variant> { Converter.ConvertToVariant(item) };
                var createdInventory = await _shopifyAdapter.CreateInventory(shopifyInventory);
               // createdInventory.ParseInventoryTags();
                var itemToInsert = Converter.Convert(createdInventory);
                return itemToInsert;
            }
            catch(Exception ex)
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
               
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return "";
        }

        private async Task UpdateShopifyItem ( Item updatedItem ) {
            // update the item show in storage
            var shopifyInventory = Converter.Convert(updatedItem);
            shopifyInventory.Tags = GetShopifyTags(updatedItem);
            await _shopifyAdapter.UpdateInventory( shopifyInventory );
        }
    }
}