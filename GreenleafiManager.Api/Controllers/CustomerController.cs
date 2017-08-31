using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using AutoMapper;
using GreenleafiManager.Api.Models;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using ShopifyClient.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Customer = GreenleafiManager.Api.DataObjects.Customer;

namespace GreenleafiManager.Api.Controllers {
    //[AuthorizeLevel ( AuthorizationLevel.User )]
    public class CustomerController : TableController<Customer> {
        private readonly IMapper _mapper;
        private readonly ShopifyAdapter _shopifyAdapter;


        public CustomerController ( IMapper mapper ) {
            _mapper = mapper;
            _shopifyAdapter = new ShopifyAdapter( new JsonDataTranslator() );
        }

        protected override void Initialize ( HttpControllerContext controllerContext ) {
            base.Initialize( controllerContext );
            var context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Customer>(context, Request);//, Services );
        }

        // GET tables/Customer
        public IQueryable<Customer> GetAllCustomer () {
            return Query();
        }

        // GET tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Customer> GetCustomer ( string id ) {
            return Lookup( id );
        }

        // PATCH tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<Customer> PatchCustomer ( string id, Delta<Customer> patch ) {
            var updatedCustomer = await UpdateAsync( id, patch );
            await UpdateCustomerInShopify( updatedCustomer );
            return updatedCustomer;
        }

        // POST tables/Customer
        public async Task<IHttpActionResult> PostCustomer ( Customer item ) {
            var customerToInsert = await SaveCustomerToShopify( item );
            var current = await InsertAsync( customerToInsert );
            return CreatedAtRoute( "Tables", new {id = current.Id}, current );
        }

        // DELETE tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCustomer ( string id ) {
            return DeleteAsync( id );
        }

        private async Task<Customer> SaveCustomerToShopify ( Customer customer ) {
            var shopifyCustomer = _mapper.Map<ShopifyClient.Models.Customer>( customer );
            shopifyCustomer.VerifiedEmail = true;

            var address = _mapper.Map<Address>( customer );
            address.AddressOriginalId = null;

            shopifyCustomer.Addresses = new List<Address> {address};
            var createdShopifyCustomer = await _shopifyAdapter.CreateCustomer( shopifyCustomer );

            var customerToInsert = _mapper.Map<Customer>( createdShopifyCustomer );
            _mapper.Map( createdShopifyCustomer.Address, customerToInsert );
            return customerToInsert;
        }

        private async Task UpdateCustomerInShopify ( Customer customer ) {
            var shopifyCustomer = _mapper.Map<ShopifyClient.Models.Customer>( customer );
            await _shopifyAdapter.UpdateCustomer( shopifyCustomer );
            var customerAddress = _mapper.Map<Address>( customer );
            await _shopifyAdapter.UpdateCustomersAddress( int.Parse(customer.OriginalId), customerAddress );
        }
    }
}