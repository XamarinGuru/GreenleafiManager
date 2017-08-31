using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using ShopifyClient.DataTranslators;
using ShopifyClient.Http;
using ShopifyClient.Models;
using ShopifyClient.Resources;
using Newtonsoft.Json;

namespace ShopifyClient
{
    public class ShopifyAdapter
    {
        private const int PageSizeLimit = 250;

        private readonly ShopifyApiClient _shopifyApiClient;

        public ShopifyAdapter(IDataTranslator dataTranslator)
        {
            var shopifyAuthorizationState = new ShopifyAuthorizationState(Constants.ShopifyStoreUrl, Constants.ShopifyPassword);
            _shopifyApiClient = new ShopifyApiClient(shopifyAuthorizationState, dataTranslator);
        }

        public async Task<List<Customer>> GetCustomers()
        {
            var customers = new List<Customer>();
            dynamic countResponse = await _shopifyApiClient.GetAsync("/admin/customers/count.json");
            int count = countResponse.count;

            var currentPage = 0;
            while (currentPage * PageSizeLimit < count)
            {
                var parameters = new NameValueCollection {
                    {"limit", PageSizeLimit.ToString()},
                    {"page", (++currentPage).ToString()}
                };
                var rootObject = await _shopifyApiClient.GetAsync<CustomerRoot>("/admin/customers.json", parameters);
                rootObject.Customers.ForEach(async cust =>
                {
                    if (cust.LastOrderId.HasValue)
                    {
                        cust.LastOrderDate = await GetOrderDate(cust.LastOrderId.Value);
                    }
                });
                customers.AddRange(rootObject.Customers);
            }
            return customers;
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            var rootObject = new CustomerRoot { Customer = customer };
            var response = await _shopifyApiClient.PostAsync("/admin/customers.json", rootObject);
            return response.Customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            var rootObject = new CustomerRoot { Customer = customer };
            var response = await _shopifyApiClient.PutAsync($"/admin/customers/{customer.OriginalId}.json", rootObject);
            return response.Customer;
        }

        public async Task<Address> UpdateCustomersAddress(long customerId, Address address)
        {
            var rootObject = new AddressRoot { UpdateAddress = address };
            var response = await _shopifyApiClient.PutAsync($"/admin/customers/{customerId}/addresses/{address.AddressOriginalId}.json", rootObject);
            return response.CustomerAddress;
        }

        public async Task<List<Location>> GetLocations()
        {
            var rootObject = await _shopifyApiClient.GetAsync<LocationsRoot>("/admin/locations.json");
            return rootObject.Locations;
        }

        public async Task<List<Inventory>> GetInventories()
        {
            var inventories = new List<Inventory>();
            dynamic countResponse = await _shopifyApiClient.GetAsync("/admin/products/count.json");
            int count = countResponse.count;

            var currentPage = 0;
            while (currentPage * PageSizeLimit < count)
            {
                var parameters = new NameValueCollection {
                    {"limit", PageSizeLimit.ToString()},
                    {"page", (++currentPage).ToString()}
                };
                var rootObject = await _shopifyApiClient.GetAsync<InventoryRoot>("/admin/products.json", parameters);
                inventories.AddRange(rootObject.Inventories);
            }
            return inventories;
        }
        public async Task<List<Inventory>> GetInventorieImages()
        {

            List<Inventory> spProducts = new List<Inventory>();

            string jsonCount = (await _shopifyApiClient.GetAsync("/admin/products/count.json")).ToString();
            string countString = jsonCount.Substring(jsonCount.IndexOf(":") + 1, jsonCount.Length - jsonCount.IndexOf(":") - 2);
            int count = int.Parse(countString.Trim());
            int page = 0;

            while (page * 50 < count)
            {
                if (page + 1 % 39 == 0)
                {
                    await Task.Delay(20 * 1000);
                }
                string jsonRequestURL = String.Format("/admin/products.json?page={0},fields=id,variants,images", page);
                string json = (await _shopifyApiClient.GetAsync(jsonRequestURL)).ToString();
                InventoryRoot pdReply = JsonConvert.DeserializeObject<InventoryRoot>(json);
                spProducts.AddRange(pdReply.Inventories);
                page++;
            }
            return spProducts;

        }

        public async Task<InventoryImage> CreateInventorieImage(string productId, string blob_uri, string fileName)
        {
            var rootObject = new InventoryPictureRoot { image = new InventoryImage { attachment = blob_uri, filename = fileName } };
            var result = await _shopifyApiClient.PostAsync($"/admin/products/{productId}/images.json", rootObject);
            return result.image;
        }
        public async Task DeleteInventorieImage(string productId, string itemId)
        {
            await _shopifyApiClient.DeleteAsync($"/admin/products/{productId}/images/{itemId}.json");
        }
        public async Task<InventoryImage> UpdateInventorieImage(string productId, string imageId, string position)
        {
            var rootObject = new InventoryPictureRoot { image = new InventoryImage { position = position} };
            var result = await _shopifyApiClient.PutAsync($"/admin/products/{productId}/images/{imageId}.json", rootObject);
            return result.image;
        }
        public async Task<Inventory> CreateInventory(Inventory inventory)
        {
            var rootObject = new InventoryRoot { Inventory = inventory };
            var response = await _shopifyApiClient.PostAsync("/admin/products.json", rootObject);
            return response.Inventory;
        }

        public async Task<Inventory> UpdateInventory(Inventory inventory)
        {
            var rootObject = new InventoryRoot { Inventory = inventory };
            var response = await _shopifyApiClient.PutAsync($"/admin/products/{inventory.OriginalId}.json", rootObject);
            return response.Inventory;
        }
        public async Task<string> UpdateVariant(VariantRoot variantRoot)
        {
            var response = await _shopifyApiClient.PutAsync($"/admin/variants/{variantRoot.variant.VariantId}.json", variantRoot);
            return response.variant.VariantId.HasValue ? response.variant.VariantId.Value.ToString() : "";
        }
        public async Task<string> AddVariant(VariantRoot variantRoot)
        {
            var response = await _shopifyApiClient.PostAsync($"/admin/products/{variantRoot.variant.VariantId}/variants.json", variantRoot);
            return response.variant.VariantId.HasValue ? response.variant.VariantId.Value.ToString() : "";
        }
        private async Task<DateTime> GetOrderDate(long id)
        {
            var parameters = new NameValueCollection {
                {"fields", "closed_at"}
            };
            dynamic rootObject = await _shopifyApiClient.GetAsync($"/admin/orders/{id}.json", parameters);
            return rootObject.order.created_at;
        }
    }
}