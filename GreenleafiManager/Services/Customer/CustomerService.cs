using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Linq;
//using ModernHttpClient;


namespace GreenleafiManager
{
	public class CustomerService
	{
		AzureService _Parent;
		
		private IMobileServiceSyncTable<Customer> CustomerTable;

		public List<Customer> Customers { get; private set; }


		public CustomerService(AzureService parent)
		{
			_Parent = parent;
			SetupTables();
		}
		public void SetupTables()
		{
			CustomerTable = _Parent.Client.GetSyncTable<Customer>();
		}
		private async Task PullAsync()
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				Customers = await CustomerTable.ToListAsync();
			}
			else
			{
				try
				{
					//await _Parent.Client.SyncContext.PushAsync();
					//Client.SyncContext.();
					await CustomerTable.PullAsync("allCustomers", CustomerTable.CreateQuery()); // query ID is used for incremental sync
				}
				catch (MobileServiceInvalidOperationException e)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
					Customers = await CustomerTable.ToListAsync();

				}
				catch (Exception ex)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"Exception: {0}", ex.Message);
					Customers = await CustomerTable.ToListAsync();
				}
				finally
				{
					Customers = await CustomerTable.ToListAsync();
				}
			}
		}

		public async Task ClearAllData()
		{
			await CustomerTable.PurgeAsync("allCustomers", "", true, new System.Threading.CancellationToken());
			Customers = new List<Customer>();
		}
		public async Task UpdateCustomersFromAzure()
		{
			await PullAsync();
		}
		public async Task UpdateCustomersFromLocalDB()
		{
			Customers = await CustomerTable.ToListAsync();
		}
		public async Task InsertOrSave(Customer model)
		{
			try
			{
				if (Customers.Any(x => x.Id == model.Id))
				{
					await CustomerTable.UpdateAsync(model); // update Customer item in the local database
					var foundItem = Customers.Where(x => x.Id == model.Id).First();
					foundItem.Address = model.Address;
					foundItem.Email = model.Email;
					foundItem.FirstName = model.FirstName;
					foundItem.LastName = model.LastName;
					foundItem.Phone = model.Phone;
					await PullAsync(); // send changes to the mobile service
				}
				else {//new
					await CustomerTable.InsertAsync(model); // Insert a new Customer into the local database. 
					await PullAsync(); // send changes to the mobile service
				}
			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}


		private async Task DeleteById(string id)
		{
			try
			{
				var foundItem = Customers.Where(x => x.Id == id).First();//.IndexOf(item);

				//Customers.Remove(foundItem);

				await CustomerTable.DeleteAsync(foundItem);

				await PullAsync();

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		public async Task Delete(Customer item)
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

		public bool FilerRecords(object cust)
		{
			Customer item = cust as Customer;

			var searchString = item.SearchString;
			if (item != null && FilterText.Equals("") && !string.IsNullOrEmpty(FilterText))
				return true;
			else if (item != null)
			{
				var exactValue = searchString.ToLower();
				string text = FilterText.ToLower();
				return exactValue.Contains(text);
			}
			return false;
		}

	}
}

