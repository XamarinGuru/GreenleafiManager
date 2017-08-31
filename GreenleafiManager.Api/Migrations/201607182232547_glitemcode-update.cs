namespace GreenleafiManager.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class glitemcodeupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "GlItemCode", c => c.String());
            DropColumn("dbo.Items", "Code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "Code", c => c.String());
            DropColumn("dbo.Items", "GlItemCode");
        }
    }
}
