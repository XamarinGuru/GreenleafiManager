using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using SQLite;
using System.Linq;
using ModernHttpClient;

namespace GreenleafiManager
{
	public class LocationService 
	{
		AzureService _Parent;

		public IMobileServiceSyncTable<Location> LocationTable;

		public List<Location> Locations { get; private set; }

		//static LocationService instance;

		public LocationService(AzureService parent)
		{
			_Parent = parent;
			SetupTables();
		}

		public void SetupTables()
		{
			LocationTable = _Parent.Client.GetSyncTable<Location>();
		}

		private async Task PullAsync()
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				Locations = await LocationTable.ToListAsync();
			}
			else
			{
				try
				{
  					//await _Parent.Client.SyncContext.PushAsync();
					await LocationTable.PullAsync("allLocations", LocationTable.CreateQuery()); // query ID is used for incremental sync
					Locations = await LocationTable.ToListAsync();
				}
				catch (MobileServiceInvalidOperationException e)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
					Locations = await LocationTable.ToListAsync();
				}
				catch (Exception ex)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"Exception: {0}", ex.Message);
					Locations = await LocationTable.ToListAsync();
				}
			}
		}
		public async Task ClearAllData()
		{
			await LocationTable.PurgeAsync("allLocations", "", true, new System.Threading.CancellationToken());
			Locations = new List<Location>();
		}
		public async Task UpdateLocationsFromAzure()
		{
			await PullAsync();
		}
		public async Task UpdateLocationsFromLocalDB()
		{
			Locations = await LocationTable.ToListAsync();
		}
		public async Task InsertOrSave(Location model)
		{
			try
			{
				if (Locations.Any(x => x.Id == model.Id))
				{
					await LocationTable.UpdateAsync(model); // update Location item in the local database
					var foundItem = Locations.Where(x => x.Id == model.Id).First();
					foundItem.Address1 = model.Address1;
					foundItem.Address2 = model.Address2;
					foundItem.City = model.City;
					foundItem.Country = model.Country;
					foundItem.Name = model.Name;
					foundItem.Phone = model.Phone;
					foundItem.Province = model.Province;
					foundItem.Zip = model.Zip;
					await PullAsync(); // send changes to the mobile service
				}
				else {//new
					await LocationTable.InsertAsync(model); // Insert a new Location into the local database. 
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
				var foundItem = Locations.Where(x => x.Id == id).First();//.IndexOf(item);

				//Locations.Remove(foundItem);

				await LocationTable.DeleteAsync(foundItem);

				await PullAsync();

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		public async Task Delete(Location item)
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

		public bool FilerRecords(object loc)
		{
			var item = loc as Location;
			var searchString = String.Format("{0} {1} {2} {3}",
											 item.Name != null ? item.Name : string.Empty,
											 item.City != null ? item.City : string.Empty,
											 item.Address1 != null ? item.Address1 : string.Empty,
											 item.Address2 != null ? item.Address2 : string.Empty,
											 item.Phone != null ? item.Phone : string.Empty
			);
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

