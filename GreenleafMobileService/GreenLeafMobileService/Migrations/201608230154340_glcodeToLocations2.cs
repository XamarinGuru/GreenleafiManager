namespace GreenLeafMobileService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class glcodeToLocations2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Locations", "GlCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Locations", "GlCost", c => c.String());
        }
    }
}
