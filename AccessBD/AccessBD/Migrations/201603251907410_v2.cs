namespace AccessBD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CorrelMat", "indexX", c => c.Int(nullable: false));
            AddColumn("dbo.CorrelMat", "indexY", c => c.Int(nullable: false));
            AddColumn("dbo.CorrelMat", "value", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CorrelMat", "value");
            DropColumn("dbo.CorrelMat", "indexY");
            DropColumn("dbo.CorrelMat", "indexX");
        }
    }
}
