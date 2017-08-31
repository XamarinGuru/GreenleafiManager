using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using SQLite;
using System.Linq;
using ModernHttpClient;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GreenleafiManager
{
	public class InvoiceService
	{
		AzureService _Parent;

		private IMobileServiceSyncTable<Invoice> InvoiceTable;
		private IMobileServiceSyncTable<PaymentType> PaymentTypeTable;
		private IMobileServiceSyncTable<Payment> PaymentTable;
		private IMobileServiceSyncTable<InvoiceItems> InvoiceItemsTable;

		public List<Invoice> Invoices { get; private set; }
		public List<PaymentType> PaymentTypes { get; private set; }
		public List<Payment> Payments { get; private set; }
		public List<InvoiceItems> InvoiceItems { get; private set; }
		public List<Item> ItemsList = new List<Item>();
		//public double SalesTax = 0.04166;
		public bool IsInvoiceSelected { get; set; }
		public Customer Customer { get; set; }

		public InvoiceService(AzureService parent)
		{
			_Parent = parent;
			SetupTables();
		}
		public void SetupTables()
		{
			InvoiceTable = _Parent.Client.GetSyncTable<Invoice>();
			PaymentTable = _Parent.Client.GetSyncTable<Payment>();
			PaymentTypeTable = _Parent.Client.GetSyncTable<PaymentType>();
			InvoiceItemsTable = _Parent.Client.GetSyncTable<InvoiceItems>();
		}
		public async Task ClearAllData()
		{
			await InvoiceItemsTable.PurgeAsync("allInvoiceItems", "", true, new System.Threading.CancellationToken());
			await PaymentTable.PurgeAsync("allPayments", "", true, new System.Threading.CancellationToken());
			await InvoiceTable.PurgeAsync("allInvoices", "", true, new System.Threading.CancellationToken());
			await PaymentTypeTable.PurgeAsync("allPaymentTypes", "", true, new System.Threading.CancellationToken());

			InvoiceItems = new List<InvoiceItems>();
			Payments = new List<Payment>();
			Invoices = new List<Invoice>();
			PaymentTypes = new List<PaymentType>();

		}
		private async Task PullAsync()
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				Invoices = await InvoiceTable.ToListAsync();
				PaymentTypes = await PaymentTypeTable.ToListAsync();
				Payments = await PaymentTable.ToListAsync();
				InvoiceItems = await InvoiceItemsTable.ToListAsync();
				//TODO Should we be warning the user that they are not syncing with the server
			}
			else
			{
				try
				{
					//await _Parent.Client.SyncContext.PushAsync();
					await InvoiceTable.PullAsync("allInvoices", InvoiceTable.CreateQuery()); // query ID is used for incremental sync
					await PaymentTable.PullAsync("allPayments", PaymentTable.CreateQuery()); // query ID is used for incremental sync
					await PaymentTypeTable.PullAsync("allPaymentTypes", PaymentTypeTable.CreateQuery()); // query ID is used for incremental sync
					await InvoiceItemsTable.PullAsync("allInvoiceItems", InvoiceItemsTable.CreateQuery());
					//await UpdateLocalItemsForLocalDB();
				}

				catch (MobileServiceInvalidOperationException e)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"Sync Failed: {0}", e.InnerException);
				}
				catch (Exception ex)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"***********:D Exception :D************ \n {0}", ex.StackTrace);
				}
				finally
				{
					Invoices = await InvoiceTable.ToListAsync();
					Payments = await PaymentTable.ToListAsync();
					PaymentTypes = await PaymentTypeTable.ToListAsync();
					InvoiceItems = await InvoiceItemsTable.ToListAsync();
				}
			}
		}

		public async Task UpdateInvoicesFromAzure()
		{
			await PullAsync();
		}
		public async Task UpdateInvoicesFromLocalDB()
		{
			//RMR why check this? if it is null then it should be null here too, and if it isn't you are resetting?
			//if (InvoiceTable.ToListAsync() != null)
				Invoices = await InvoiceTable.ToListAsync();

			PaymentTypes = await PaymentTypeTable.ToListAsync();
			Payments = await PaymentTable.ToListAsync();
			InvoiceItems = await InvoiceItemsTable.ToListAsync();
		}

		public async Task DeletePaymentItem(Payment item, bool isSeprate = false)
		{
			try
			{
				await PaymentTable.DeleteAsync(item);
				if (isSeprate)
					await PaymentTable.PullAsync("allPayments", PaymentTable.CreateQuery());
			}
			catch (Exception ex)
			{
				Console.WriteLine("DELETE PAYMENT***" + ex.StackTrace);
			}
		}

		public async Task InsertOrSaveInvoice(Invoice model)
		{
			try
			{
				if (Invoices != null && Invoices.Any(x => x.Id == model.Id))
				{
					foreach (var item in model.Payments)
					{
						if (item.Update)
						{
							item.Update = false;
							await PaymentTable.UpdateAsync(item);
						}
						else if (item.Add)
						{
							item.Add = false;
							item.InvoiceId = model.Id;
							item.PaymentTypeId = item.PaymentType.Id;
							await PaymentTable.InsertAsync(item);
						}
						else if (item.Remove)
						{
							item.Remove = false;
							await PaymentTable.DeleteAsync(item);
						}
					}
					foreach (var e in model.Items)
					{
						if (e.AddNewFromInvoice)
						{
							e.AddNewFromInvoice = false;
							e.RemoveFromInvoice = false;
							var item = new InvoiceItems()
							{
								Invoice_Id = model.Id,
								Item_Id = e.Id
							};
							await AzureService.InventoryService.MarkItemSoldAsync(e);
							await InvoiceItemsTable.InsertAsync(item);
						}else if(e.RemoveFromInvoice)
						{
							e.RemoveFromInvoice = false;
							e.AddNewFromInvoice = false;
							var remove = AzureService.InvoiceService.InvoiceItems.FirstOrDefault(x => x.Item_Id == e.Id && x.Invoice_Id == model.Id);
							await AzureService.InventoryService.MarkItemSoldAsync(e, false);
							if (remove != null)
							{
								await InvoiceItemsTable.DeleteAsync(remove);
							}

						}
							
					}
					await InvoiceTable.UpdateAsync(model);
					await AzureService.DefaultService.PushAsync();
					await PullAsync();
					//await Client.SyncContext.PushAsync();
					//RMR TODO Refactor - might not need the pull //await AzureService.InventoryService.PullAsync();// send changes to the mobile service

				}
				else {//new entry
					model.Id = Guid.NewGuid().ToString();
					await InvoiceTable.InsertAsync(model);
					foreach (var item in model.Items)
					{
						item.AddNewFromInvoice = false;
						item.RemoveFromInvoice = false;
						await InvoiceItemsTable.InsertAsync(new InvoiceItems()
						{
							Invoice_Id = model.Id,
							Item_Id = item.Id
						});
						await AzureService.InventoryService.MarkItemSoldAsync(item);
					}
					foreach (var e in model.Payments)
					{
						e.InvoiceId = model.Id;
						e.PaymentTypeId = e.PaymentType.Id;
						e.PaymentType = null;
						await PaymentTable.InsertAsync(e);
						e.Add = false;
						e.Remove = false;
						e.Update = false;
					}
					//await Client.SyncContext.PushAsync();
					await AzureService.DefaultService.PushAsync();
					//RMR TODO Refactor - might not need the pull //await PullAsync();
					//await AzureService.InventoryService.SyncAsync();// send changes to the mobile service
				}
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(@"ERROR {0}", ex.Message);
			}
			finally
			{
				await UpdateInvoicesFromLocalDB();
			}
		}

		private async Task DeleteById(string id)
		{
			try
			{
				var foundItem = Invoices.Where(x => x.Id == id).First();//.IndexOf(item);

				Invoices.Remove(foundItem);

				await InvoiceTable.DeleteAsync(foundItem);

				await PullAsync();

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		public async Task Delete(Invoice item)
		{
			await DeleteById(item.Id);
		}




		internal delegate void FilterChanged();

		internal FilterChanged FilterTextChanged;

		private string filterText = "";

		public string FilterText
		{
			get { return filterText; }
			set
			{
				filterText = value;
				OnFilterTextChanged();
				//RaisePropertyChanged("FilterText");
			}
		}

		private void OnFilterTextChanged()
		{
			if (FilterTextChanged != null)
				FilterTextChanged();
		}

		public bool FilterRecords(object loc)
		{
			//var item = loc as Invoice;
			//var searchString = String.Format("{0} {1} {2} {3}",
			//                                 item.Name != null ? item.Name : string.Empty,
			//                                 item.City != null ? item.City : string.Empty,
			//                                 item.Address1 != null ? item.Address1 : string.Empty,
			//                                 item.Address2 != null ? item.Address2 : string.Empty,
			//                                 item.Phone != null ? item.Phone : string.Empty
			//);
			//if (item != null && FilterText.Equals("") && !string.IsNullOrEmpty(FilterText))
			//	return true;
			//else if (item != null)
			//{
			//	var exactValue = searchString.ToLower();
			//	string text = FilterText.ToLower();
			//	return exactValue.Contains(text);
			//}
			return false;
		}

		public string GetNextInvoiceNumber()
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				return string.Empty;
			}
			if (!Reachability.IsHostReachable(_Parent.ApplicationURL))
			{
				return string.Empty;
			}
			else
			{
				using (var Client = new HttpClient())
				{
					Client.BaseAddress = new Uri(AzureService.DefaultService.ApplicationURL);

					Client.DefaultRequestHeaders.Accept.Clear();
					Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
					Client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
					//var httpContent = new StringContent(JsonConvert.SerializeObject(pp), Encoding.UTF8, "application/json");
					var response = Client.GetAsync("api/invoice/GetNextInvoiceNumber").Result;

					if (!response.IsSuccessStatusCode)
					{
						return string.Empty;
					}
					else
					{
						var json = response.Content.ReadAsStringAsync().Result;
						var pictureName = JsonConvert.DeserializeObject<string>(json);
						return pictureName;

					}
				}
			}
		}
	}
}

