namespace GreenLeafMobileService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeVirtsFromInvoice : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Invoices", new[] { "CustomerId" });
            AlterColumn("dbo.Invoices", "CustomerId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoices", "CustomerId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Invoices", "CustomerId");
            AddForeignKey("dbo.Invoices", "CustomerId", "dbo.Customers", "Id");
        }
    }
}
