namespace AccessBD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
        
            
            CreateTable(
                "dbo.Covariances",
                c => new
                    {
                        date = c.DateTime(nullable: false),
                        indexX = c.Int(nullable: false),
                        indexY = c.Int(nullable: false),
                        value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.date, t.indexX, t.indexY });
            
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Options", "underlying_AssetDBId", "dbo.Equities");
            DropForeignKey("dbo.Options", "AssetDBId", "dbo.AssetDBs");
            DropForeignKey("dbo.Forex", "RateDBId", "dbo.InterestRatesTypes");
            DropForeignKey("dbo.Forex", "AssetDBId", "dbo.AssetDBs");
            DropForeignKey("dbo.Equities", "AssetDBId", "dbo.AssetDBs");
            DropForeignKey("dbo.Everglades", "AssetDBId", "dbo.AssetDBs");
            DropForeignKey("dbo.InterestRatesValues", "RateDBId", "dbo.InterestRatesTypes");
            DropForeignKey("dbo.Prices", "AssetDBId", "dbo.AssetDBs");
            DropForeignKey("dbo.Portfolio Composition", "AssetDBId", "dbo.AssetDBs");
            DropIndex("dbo.Options", new[] { "underlying_AssetDBId" });
            DropIndex("dbo.Options", new[] { "AssetDBId" });
            DropIndex("dbo.Forex", new[] { "RateDBId" });
            DropIndex("dbo.Forex", new[] { "AssetDBId" });
            DropIndex("dbo.Equities", new[] { "AssetDBId" });
            DropIndex("dbo.Everglades", new[] { "AssetDBId" });
            DropIndex("dbo.InterestRatesValues", new[] { "RateDBId" });
            DropIndex("dbo.Prices", new[] { "AssetDBId" });
            DropIndex("dbo.Portfolio Composition", new[] { "AssetDBId" });
            DropTable("dbo.Options");
            DropTable("dbo.Forex");
            DropTable("dbo.Equities");
            DropTable("dbo.Everglades");
            DropTable("dbo.InterestRatesValues");
            DropTable("dbo.Prices");
            DropTable("dbo.HedgingPortfolio");
            DropTable("dbo.Portfolio Composition");
            DropTable("dbo.LastConnectionDBs");
            DropTable("dbo.Covariances");
            DropTable("dbo.Cash");
            DropTable("dbo.InterestRatesTypes");
            DropTable("dbo.AssetDBs");
        }
    }
}
