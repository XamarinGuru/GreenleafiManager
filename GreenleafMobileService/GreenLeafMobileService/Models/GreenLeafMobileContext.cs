using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using GreenLeafMobileService.DataObjects;

namespace GreenLeafMobileService.Models
{
    public class GreenLeafMobileContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private const string connectionStringName = "Name=MS_TableConnectionString";

        public GreenLeafMobileContext() : base(connectionStringName)
        {
            Database.SetInitializer<GreenLeafMobileContext>(null);
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GLCode> GLCodes { get; set; }
        public DbSet<MetalCode> MetalCodes { get; set; }
        public DbSet<StoneCode> StoneCodes { get; set; }
        public DbSet<VendorId> VendorIds { get; set; }
        public DbSet<ItemCode> ItemCodes { get; set; }
        public DbSet<InvoiceItems> InvoiceItems { get; set; }
        public DbSet<ItemsPictures> ItemPictures { get; set; }

        public void Update(EntityData entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var schema = "dbo";// ServiceSettingsDictionary.GetSchemaName();
            if (!string.IsNullOrEmpty(schema))
            {
                modelBuilder.HasDefaultSchema(schema);
            }
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));

           // modelBuilder.Entity<Invoice>().HasMany(b => b.Items);//.WithMany(x => x.Invoices);
        }
    }

}
