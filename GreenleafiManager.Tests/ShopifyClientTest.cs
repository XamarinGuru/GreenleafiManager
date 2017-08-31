using System;
using System.Collections.Generic;
using GreenleafiManager.ShopifyClient;
using GreenleafiManager.ShopifyClient.DataTranslators;
using GreenleafiManager.ShopifyClient.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GreenleafiManager.Tests {
    [TestClass]
    public class ShopifyClientTest {
        private ShopifyAdapter _shopifyAdapter;

        [TestInitialize]
        public void Initialize () {
            var dataTranslator = new JsonDataTranslator();
            _shopifyAdapter = new ShopifyAdapter( dataTranslator );
        }

        [TestMethod]
        public void ShopifyLocations_Get_Succeed () {
            try {
                var locations = _shopifyAdapter.GetLocations().Result;
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }

        [TestMethod]
        public void ShopifyCustomers_Get_Succeed () {
            try {
                var customers = _shopifyAdapter.GetCustomers().Result;
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }

        [TestMethod]
        public void ShopifyCustomers_Create_Succeed () {
            var customer = new Customer {
                FirstName = "Test",
                LastName = "Customer",
                Email = "test@test.com",
                VerifiedEmail = true,
                Addresses = new List<Address> {
                    new Address {
                        Address1 = "123 str",
                        City = "Lviv",
                        Province = "Lv",
                        Phone = "123-456-789",
                        Zip = "123",
                        Country = "Ukraine"
                    }
                }
            };

            try {
                var result = _shopifyAdapter.CreateCustomer( customer ).Result;
                Assert.AreNotEqual( 0, result.OriginalId );
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }

        [TestMethod]
        public void ShopifyCustomers_UpdateEmail_Succeed () {
            var customer = new Customer {
                OriginalId = 2981737475,
                Email = "newemail@test.com"
            };

            try {
                var result = _shopifyAdapter.UpdateCustomer( customer ).Result;
                Assert.AreEqual( customer.Email, result.Email );
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }

        [TestMethod]
        public void ShopifyCustomers_UpdateAddress_Succeed () {
            var customerId = 2981737475;
            var address = new Address {
                AddressOriginalId = 3138013251,
                Address1 = "456 str"
            };
            try {
                var result = _shopifyAdapter.UpdateCustomersAddress( customerId, address ).Result;
                Assert.AreEqual( address.Address1, result.Address1 );
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }

        [TestMethod]
        public void ShopifyInventories_Get_Succeed () {
            try {
                var inventories = _shopifyAdapter.GetInventories().Result;
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }

        [TestMethod]
        public void ShopifyInventories_Update_Succeed () {
            var inventory = new Inventory {
                OriginalId = 5474997251,
                Description = "Test update"
            };

            try {
                var result = _shopifyAdapter.UpdateInventory( inventory ).Result;
                Assert.AreEqual( inventory.Description, result.Description );
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }

        [TestMethod]
        public void ShopifyInventories_Create_Succeed () {
            var inventory = new Inventory {
                Title = "Test inventory",
                Description = "Test create",
                Vendor = "Vendor",
                ProductType = "Product type",
                Variants = new List<Variant> {
                    new Variant {
                        Price = 103.65M,
                        Sku = "123",
                        Barcode = "123"
                    }
                }
            };

            try {
                var result = _shopifyAdapter.CreateInventory( inventory ).Result;
                Assert.AreNotEqual( null, result.OriginalId );
            } catch ( Exception e ) {
                Assert.Fail( e.Message );
            }
        }
    }
}