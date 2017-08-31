using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using GreenleafiManager.Api.DataObjects;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;

namespace GreenleafiManager.Api.Models {
    public class MobileServiceContext : DbContext {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
        //
        // To enable Entity Framework migrations in the cloud, please ensure that the 
        // service name, set by the 'MS_MobileServiceName' AppSettings in the local 
        // Web.config, is the same as the service name when hosted in Azure.

        private const string ConnectionStringName = "Name=GreenleafEntities";

        public MobileServiceContext () : base( ConnectionStringName ) {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        public void Update ( EntityData entity ) {
            Entry( entity ).State = EntityState.Modified;
        }

        protected override void OnModelCreating ( DbModelBuilder modelBuilder ) {
            var schema = "dbo";// ServiceSettingsDictionary.GetSchemaName();
            if ( !string.IsNullOrEmpty( schema ) ) {
                modelBuilder.HasDefaultSchema( schema );
            }

            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", ( property, attributes ) => attributes.Single().ColumnType.ToString() ) );
        }
    }
}