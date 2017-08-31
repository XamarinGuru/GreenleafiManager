namespace GreenLeafMobileService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class glcodeToLocations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "GlCost", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "GlCost");
        }
    }
}
