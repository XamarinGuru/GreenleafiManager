using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace GreenleafiManager
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
		const string _ApplicationURL = @"http://greenleafmobileservice.azurewebsites.net/";
		//const string _ApplicationURL = @"http://192.168.112.102/greenleafmobileservice/";//RMR
		//const string _ApplicationURL = @"http://192.168.200.104/greenleafmobileservice/";//DJ
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
			await InvoiceService.ClearAllData();
			await InventoryService.ClearAllData();
			await LocationService.ClearAllData();
			await UserService.ClearAllData();
			await CustomerService.ClearAllData();
		}
		static CustomerService _CustomerService;
		public static CustomerService CustomerService
		{
			get
			{
				if (_CustomerService == null)
				{
					_CustomerService = new CustomerService(DefaultService);
				}
				return _CustomerService;
			}
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
		static InvoiceService _InvoiceService;
		public static InvoiceService InvoiceService
		{
			get
			{
				if (_InvoiceService == null)
				{
					_InvoiceService = new InvoiceService(DefaultService);
				}
				return _InvoiceService;
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
		static ReportsService _ReportsService;
		public static ReportsService ReportsService
		{
			get
			{
				if (_ReportsService == null)
				{
					_ReportsService = new ReportsService(DefaultService);
				}
				return _ReportsService;
			}
		}
		static UserService _UserService;
		public static UserService UserService
		{
			get
			{
				if (_UserService == null)
				{
					_UserService = new UserService(DefaultService);
				}
				return _UserService;
			}
		}

		public async Task InitializeStoreAsync()
		{
			var store = new MobileServiceSQLiteStore(LocalDbPath);

			store.DefineTable<Customer>();

			store.DefineTable<Location>();

			store.DefineTable<User>();
			store.DefineTable<Role>();

			store.DefineTable<Item>();
			store.DefineTable<GLCode>();
			store.DefineTable<StoneCode>();
			store.DefineTable<MetalCode>();
			store.DefineTable<ItemCode>();
			store.DefineTable<VendorId>();

			store.DefineTable<Invoice>();
			store.DefineTable<Payment>();
			store.DefineTable<PaymentType>();
			store.DefineTable<InvoiceItems>();

			// Uses the default conflict handler, which fails on conflict
			await Client.SyncContext.InitializeAsync(store);
			_InitialLoadComplete = true;
		}
		public async Task SetupConfirmationWithSyncForAllTables()
		{
			await CustomerSetupConfirmationWithSync();
			await LocationSetupConfirmationWithSync();
			await InventorySetupConfirmationWithSync();
			await InvoiceSetupConfirmationWithSync();
			await UserSetupConfirmationWithSync();
		}

		public async Task CustomerSetupConfirmationWithSync()
		{
			await CustomerService.UpdateCustomersFromLocalDB();

			if (CustomerService.Customers == null || CustomerService.Customers.Count == 0)
			{
				await CustomerService.UpdateCustomersFromAzure();
			}
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
			await InventoryService.UpdateItemsFromLocalDB();

			if (InventoryService.Items == null || InventoryService.Items.Count == 0)
			{
				await InventoryService.UpdateItemsFromAzure();
			}
		}
		public async Task InvoiceSetupConfirmationWithSync()
		{
			await InvoiceService.UpdateInvoicesFromLocalDB();

			if (InvoiceService.Invoices == null || InvoiceService.Invoices.Count == 0)
			{
				await InvoiceService.UpdateInvoicesFromAzure();
			}
		}
		public async Task UserSetupConfirmationWithSync()
		{
			await UserService.UpdateUsersFromLocalDB();

			if (UserService.Users == null || UserService.Users.Count == 0)
			{
				await UserService.UpdateUsersFromAzure();
			}
		}


	}
}
