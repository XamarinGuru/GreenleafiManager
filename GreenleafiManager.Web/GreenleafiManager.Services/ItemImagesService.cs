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
    public class ItemImagesService
    {
        public async Task<List<ItemPictureModel>> GetItemPictures(string SKU)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://greenleafmobileservice.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"api/images/{SKU}");

                var json = string.Empty;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Could not get items.");
                else
                {
                    json = await response.Content.ReadAsStringAsync();
                    List<ItemPictureModel> result = JsonConvert.DeserializeObject<List<ItemPictureModel>>(json);
                    return result.OrderByDescending(x => x.position).ToList();
                }
            }
        }

        public async Task DeleteItemPictures(string fileName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://greenleafmobileservice.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"api/images/delete/{fileName}");

                var json = string.Empty;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Could not delete picture.");
            }
        }

        public async Task UpdateItemPictures(string pictureName, string position)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://greenleafmobileservice.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("pictureName", pictureName),
                        new KeyValuePair<string, string>("position", position)
                    });

                var response = await client.PostAsync($"api/images/updateposition", formContent);

                var json = string.Empty;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Could not delete picture.");
            }
        }

        public async Task<string> CreateItemPictures(string SKU, string picturebase64)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://greenleafmobileservice.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("itemId", SKU),
                        new KeyValuePair<string, string>("picturebase64", picturebase64)
                    });

                var response = await client.PostAsync($"api/images", formContent);

                var json = string.Empty;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Could not delete picture.");

                json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string>(json);
            }
        }
    }
}
