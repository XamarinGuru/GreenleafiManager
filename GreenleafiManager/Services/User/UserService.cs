using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Linq;
using ModernHttpClient;


namespace GreenleafiManager
{
	public class UserService 
	{
		AzureService _Parent;

		private IMobileServiceSyncTable<User> UserTable;
		private IMobileServiceSyncTable<Role> RoleTable;

		public List<User> Users { get; private set; }
		public List<Role> Roles { get; private set; }

		public UserService(AzureService parent)
		{
			_Parent = parent;
			SetupTables();
		}
		public void SetupTables()
		{
			UserTable = _Parent.Client.GetSyncTable<User>();
			RoleTable = _Parent.Client.GetSyncTable<Role>();
		}
		public async Task ClearAllData()
		{
			await UserTable.PurgeAsync("allUsers", "", true, new System.Threading.CancellationToken());
			await RoleTable.PurgeAsync("allRoles", "", true, new System.Threading.CancellationToken());

			Users = new List<User>();
			Roles = new List<Role>();
		}
		private async Task PullAsync()
		{
			if (!Reachability.IsHostReachable("http://google.com"))
			{
				Users = await UserTable.ToListAsync();
				Roles = await RoleTable.ToListAsync();
			}
			else
			{
				try
				{
					//await _Parent.Client.SyncContext.PushAsync();

					await UserTable.PullAsync("allUsers", UserTable.CreateQuery()); // query ID is used for incremental sync
					await RoleTable.PullAsync("allRoles", RoleTable.CreateQuery()); // query ID is used for incremental sync

					Users = await UserTable.ToListAsync();
					Roles = await RoleTable.ToListAsync();
				}

				catch (MobileServiceInvalidOperationException e)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
					Users = await UserTable.ToListAsync();
					Roles = await RoleTable.ToListAsync();

				}
				catch (Exception ex)
				{
					//RMR catching errors incase reachablility fails
					Console.Error.WriteLine(@"Exception: {0}", ex.Message);
					Users = await UserTable.ToListAsync();
					Roles = await RoleTable.ToListAsync();

				}
			}
		}

		public async Task UpdateUsersFromAzure()
		{
			await PullAsync();
		}
		public async Task UpdateUsersFromLocalDB()
		{
			Users = await UserTable.ToListAsync();
			Roles = await RoleTable.ToListAsync();
		}
		public async Task InsertOrSave(User model)
		{
			try
			{
				if (Users.Any(x => x.Id == model.Id))
				{
					await UserTable.UpdateAsync(model); // update User item in the local database
					var foundItem = Users.Where(x => x.Id == model.Id).First();
					foundItem.Address = model.Address;
					foundItem.Email = model.Email;
					foundItem.FirstName = model.FirstName;
					foundItem.LastName = model.LastName;
					foundItem.Phone = model.Phone;
					foundItem.UserName = model.UserName;
					foundItem.RoleId = Roles.Where(x => x.Name == model.RoleName).First().Id;
					await PullAsync(); // send changes to the mobile service
				}
				else {//new
					await UserTable.InsertAsync(model); // Insert a new User into the local database. 
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


		private async Task DeleteUserById(string id)
		{
			try
			{
				var foundItem = Users.Where(x => x.Id == id).First();//.IndexOf(item);

				Users.Remove(foundItem);

				await UserTable.DeleteAsync(foundItem);

				await PullAsync();

			}
			catch (MobileServiceInvalidOperationException e)
			{
				Console.Error.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		public async Task DeleteUser(User item)
		{
			await DeleteUserById(item.Id);
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
			var item = cust as User;
			var searchString = String.Format("{0} {1} {2} {3}",
				item.FirstName != null ? item.FirstName : string.Empty,
				item.LastName != null ? item.LastName : string.Empty,
				item.Email != null ? item.Email : string.Empty,
			                                 item.UserName != null ? item.UserName : string.Empty,
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


