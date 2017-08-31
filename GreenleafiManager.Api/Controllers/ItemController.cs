using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using AutoMapper;
using GreenleafiManager.Api.DataObjects;
using GreenleafiManager.Api.Models;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using ShopifyClient.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;

namespace GreenleafiManager.Api.Controllers {
    //[AuthorizeLevel ( AuthorizationLevel.User )]
    public class ItemController : TableController<Item> {
        private readonly IMapper _mapper;
        private readonly ShopifyAdapter _shopifyAdapter;

        public ItemController ( IMapper mapper ) {
            _mapper = mapper;
            _shopifyAdapter = new ShopifyAdapter( new JsonDataTranslator() );
        }

        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Item>(context, Request);//, Services );
        }

        // GET tables/Item
        public IQueryable<Item> GetAllItem () {
            return Query();
        }

        // GET tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Item> GetItem ( string id ) {
            return Lookup( id );
        }

        // PATCH tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<Item> PatchItem ( string id, Delta<Item> patch ) {
            var updatedItem = await UpdateAsync( id, patch );
            await UpdateShopifyItem( updatedItem );
            return updatedItem;
        }

        // POST tables/Item
        public async Task<IHttpActionResult> PostItem ( Item item ) {
            var itemToInsert = await SaveItemToShopify( item );
            var current = await InsertAsync( itemToInsert );
            return CreatedAtRoute( "Tables", new {id = current.Id}, current );
        }

        // DELETE tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteItem ( string id ) {
            return DeleteAsync( id );
        }

        private async Task<Item> SaveItemToShopify ( Item item ) {
            var shopifyInventory = _mapper.Map<Inventory>( item );
            shopifyInventory.GenerateInventoryTags();
            shopifyInventory.Variants = new List<Variant> {
                _mapper.Map<Variant>( item )
            };

            var createdInventory = await _shopifyAdapter.CreateInventory( shopifyInventory );
            createdInventory.ParseInventoryTags();

            var itemToInsert = _mapper.Map<Item>( createdInventory );
            if ( createdInventory.Variants != null && createdInventory.Variants.Any() ) {
                _mapper.Map( createdInventory.Variants.First(), itemToInsert );
            }
            return itemToInsert;
        }

        private async Task UpdateShopifyItem ( Item updatedItem ) {
            var shopifyInventory = _mapper.Map<Inventory>( updatedItem );
            shopifyInventory.Variants = new List<Variant> {
                _mapper.Map<Variant>( updatedItem )
            };
            await _shopifyAdapter.UpdateInventory( shopifyInventory );
        }
    }
}