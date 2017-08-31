using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace GreenleafiManager
{
	public class ReportsService
	{
		AzureService _Parent;
		public ReportsService(AzureService parent){
			_Parent = parent;
		}
		public StockReportModel StockReport(string locationId)
		{
			if (Reachability.IsHostReachable("http://google.com"))
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(_Parent.ApplicationURL);
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
					var response = client.GetAsync(String.Format("tables/Reports/CurrentStockReport/?locationId={0}", locationId)).Result;

					if (!response.IsSuccessStatusCode)
						return new StockReportModel();
					else {
						var json = response.Content.ReadAsStringAsync().Result;
						StockReportModel result = JsonConvert.DeserializeObject<StockReportModel>(json);
						return result;
					}
				}
			}
			else
			{
				throw new Exception("No internet connection avaiable.");

			}
		}

		public SalesReportModel SalesReport(string locationId, string userId, DateTime startDate, DateTime endDate)
		{
			if (Reachability.IsHostReachable("http://google.com"))
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(_Parent.ApplicationURL);
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
					var response = client.GetAsync(String.Format("tables/Reports/SalesReport/?startDate={0}&endDate={1}&userId={3}&locationId={2}", 
					                                             startDate.ToString("MM/dd/yyyy"), 
					                                             endDate.ToString("MM/dd/yyyy"),
					                                             locationId,
					                                             userId)).Result;

					if (!response.IsSuccessStatusCode)
						return new SalesReportModel();
					else {
						var json = response.Content.ReadAsStringAsync().Result;
						SalesReportModel result = JsonConvert.DeserializeObject<SalesReportModel>(json);
						return result;
					}
				}
			}
			else
			{
				throw new Exception("No internet connection avaiable.");

			}
		}

	}
}

