using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace GreenleafiManagerPhone
{
	public class AzureService
	{
		static AzureService instance;
		private AzureService()
		{
			CurrentPlatform.Init();
			SQLitePCL.CurrentPlatform.Init();
			_Client = new MobileServiceClient(ApplicationURL);
		}
		private bool _InitialLoadComplete = false;
		public bool InitialLoadComplete
		{
			get{return _InitialLoadComplete;}
		}
		public static AzureService DefaultService
		{
			get
			{
				if (instance == null)
				{
					instance = new AzureService();
				}
				return instance;
			}
		}
		const string _ApplicationURL = @"http://greenleafmobileservice.azurewebsites.net";
		//const string _ApplicationURL = @"http://192.168.112.102/greenleafmobileservice";//RMR
		//const string _ApplicationURL = @"http://192.168.200.105/greenleafmobileservice";//DJ
		public string ApplicationURL { get { return _ApplicationURL; } }

		const string _LocalDbPath = "localstoreinv.db";
		public string LocalDbPath { get { return _LocalDbPath; } }

		MobileServiceClient _Client;
		public MobileServiceClient Client { get { return _Client; } }

		public async Task PushAsync()
		{
			await Client.SyncContext.PushAsync();
		}
		public async Task PurgeAllTables()
		{
			await InventoryService.ClearAllData();
			await LocationService.ClearAllData();
		}
	
		static InventoryService _InventoryService;
		public static InventoryService InventoryService
		{
			get
			{
				if (_InventoryService == null)
				{
					_InventoryService = new InventoryService(DefaultService);
				}
				return _InventoryService;
			}
		}

		static LocationService _LocationService;
		public static LocationService LocationService
		{
			get
			{
				if (_LocationService == null)
				{
					_LocationService = new LocationService(DefaultService);
				}
				return _LocationService;
			}
		}


		public async Task InitializeStoreAsync()
		{
			var store = new MobileServiceSQLiteStore(LocalDbPath);



			store.DefineTable<Location>();


			store.DefineTable<Item>();
			store.DefineTable<GLCode>();
			store.DefineTable<StoneCode>();
			store.DefineTable<MetalCode>();
			store.DefineTable<ItemCode>();
			store.DefineTable<VendorId>();

			// Uses the default conflict handler, which fails on conflict
			await Client.SyncContext.InitializeAsync(store);
			_InitialLoadComplete = true;
		}
		public async Task SetupConfirmationWithSyncForAllTables()
		{
			await LocationSetupConfirmationWithSync();
			await InventorySetupConfirmationWithSync();
		}


		public async Task LocationSetupConfirmationWithSync()
		{
			await LocationService.UpdateLocationsFromLocalDB();

			if (LocationService.Locations == null || LocationService.Locations.Count == 0)
			{
				await LocationService.UpdateLocationsFromAzure();
			}
		}
		public async Task InventorySetupConfirmationWithSync()
		{
			await InventoryService.AllCodesAndIdsSetupConfirmationWithSync();
		
		}



	}
}
