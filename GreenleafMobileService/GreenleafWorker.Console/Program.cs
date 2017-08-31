using GreenLeafMobileService.Models;
using GreenleafWorker.Console.Services;
using ShopifyClient;
using ShopifyClient.DataTranslators;
using System;
using System.Threading.Tasks;

namespace GreenleafWorker.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello world");

            //var service = new UpdatePriceGLWorkerService(
            //       new ShopifyAdapter(new ShopifyClient.DataTranslators.JsonDataTranslator())
            //     , new GreenLeafMobileContext());

            //var service = new DeleteDuplicatedItemsGLWorkerService(
            //       new ShopifyAdapter(new ShopifyClient.DataTranslators.JsonDataTranslator())
            //     , new GreenLeafMobileContext());

            var service = new AddItemsThatHavePicturesToShopify(
                   new ShopifyAdapter(new ShopifyClient.DataTranslators.JsonDataTranslator())
                 , new GreenLeafMobileContext());


            Task.WaitAll(service.Execute());

            System.Console.WriteLine("Finished");
            System.Console.WriteLine("Hit any key to exit");
            System.Console.ReadKey();
        }
    }
}