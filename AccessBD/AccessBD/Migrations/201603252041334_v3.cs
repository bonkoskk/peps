namespace AccessBD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v3 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.CorrelMat");
            AddPrimaryKey("dbo.CorrelMat", new[] { "date", "indexX", "indexY" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.CorrelMat");
            AddPrimaryKey("dbo.CorrelMat", "date");
        }
    }
}
