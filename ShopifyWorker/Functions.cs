using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GreenLeafMobileService;
using GreenLeafMobileService.DataObjects;
using GreenLeafMobileService.Repositories;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using Microsoft.Azure.WebJobs;

namespace ShopifyWorker {
    public class Functions {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        private static readonly UnitOfWork UnitOfWork = new UnitOfWork();
        private static readonly IMapper Mapper = MappingConfig.RegisterMappings().CreateMapper();
        private static readonly ShopifyAdapter ShopifyAdapter = new ShopifyAdapter( new JsonDataTranslator() );

        public static void ProcessQueueMessage ( [QueueTrigger ( "queue" )] string message, TextWriter log ) {
            log.WriteLine( message );
        }

        public static async Task ProcessLocations ( [TimerTrigger ( "01:00:00", UseMonitor = true )] TimerInfo timerInfo, TextWriter log ) {
            try {
                
                var shopifyLocations = await ShopifyAdapter.GetLocations();

                log.WriteLine( $"Retrieved {shopifyLocations.Count} locations from shopify" );

                var locationEntites = Mapper.Map<List<Location>>( shopifyLocations );

                if ( shopifyLocations.Any() ) {
                    log.WriteLine( $"Start saving {locationEntites.Count} locations to db" );

                    UnitOfWork.LocationRepository.SetDefaultColumns( locationEntites );
                    UnitOfWork.LocationRepository.UpsertBulk( locationEntites );

                    log.WriteLine( "Saved locations to db" );
                }
            } catch ( Exception ex ) {
                log.WriteLine( $"{ex.Message}\r\n{ex.InnerException}\r\n{ex.StackTrace}" );
            }
        }

        public static async Task ProcessCustomers ( [TimerTrigger ( "01:00:00", UseMonitor = true )] TimerInfo timerInfo, TextWriter log ) {
            try {
                var shopifyCustomers = await ShopifyAdapter.GetCustomers();

                log.WriteLine( $"Retrieved {shopifyCustomers.Count} customers from shopify" );

                var customerEntities = Mapper.Map<List<Customer>>( shopifyCustomers );

                if ( customerEntities.Any() ) {
                    log.WriteLine( $"Start saving {customerEntities.Count} customers to db" );

                    UnitOfWork.CustomerRepository.SetDefaultColumns( customerEntities );
                    UnitOfWork.CustomerRepository.UpsertBulk( customerEntities );

                    log.WriteLine( "Saved customers to db" );
                }
            } catch ( Exception ex ) {
                log.WriteLine( $"{ex.Message}\r\n{ex.InnerException}\r\n{ex.StackTrace}" );
            }
        }

        public static async Task ProcessItems ( [TimerTrigger ( "01:00:00", UseMonitor = true )] TimerInfo timerInfo, TextWriter log ) {
            try {
                var shopifyItems = await ShopifyAdapter.GetInventories();

                log.WriteLine( $"Retrieved {shopifyItems.Count} items from shopify" );

                var locationsToAssing = UnitOfWork.LocationRepository.GetAll();

                var itemEntities = new List<Item>();

                shopifyItems.ForEach( shopifyItem => {
                    var entity = Mapper.Map<Item>( shopifyItem );
                    entity.LocationId = locationsToAssing.FirstOrDefault( l => l.Name == shopifyItem.Location )?.Id;
                    itemEntities.Add( entity );
                } );

                if ( itemEntities.Any() ) {
                    log.WriteLine( $"Start saving {itemEntities.Count} items to db" );

                    UnitOfWork.ItemRepository.SetDefaultColumns( itemEntities );
                    UnitOfWork.ItemRepository.UpsertBulk( itemEntities );

                    log.WriteLine( "Saved items to db" );
                }
            } catch ( Exception ex ) {
                log.WriteLine( $"{ex.Message}\r\n{ex.InnerException}\r\n{ex.StackTrace}" );
            }
        }
    }
}