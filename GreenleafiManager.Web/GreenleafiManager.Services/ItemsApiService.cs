using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GreenleafiManager.Services
{
    public class ItemsApiService
    {
        public async Task<List<ItemModel>> GetItems(string SKU)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://greenleafmobileservice.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
                var response = await client.GetAsync($"tables/item?$filter=Sku eq '{SKU}'");


                var json = string.Empty;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Could not get items.");
                else
                {
                    json = await response.Content.ReadAsStringAsync();
                    List<ItemModel> result = JsonConvert.DeserializeObject<List<ItemModel>>(json);
                    return result;
                }
            }
        }
    }
}
