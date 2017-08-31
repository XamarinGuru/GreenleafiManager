namespace GreenLeafMobileService.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class ItemsAndInvoiceTable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.InvoiceItems");
            AlterColumn("dbo.InvoiceItems", "Id", c => c.String(nullable: false, maxLength: 128,
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "ServiceTableColumn",
                        new AnnotationValues(oldValue: null, newValue: "Id")
                    },
                }));
            AddPrimaryKey("dbo.InvoiceItems", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.InvoiceItems");
            AlterColumn("dbo.InvoiceItems", "Id", c => c.Guid(nullable: false,
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "ServiceTableColumn",
                        new AnnotationValues(oldValue: "Id", newValue: null)
                    },
                }));
            AddPrimaryKey("dbo.InvoiceItems", "Id");
        }
    }
}
