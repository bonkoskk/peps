namespace AccessBD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetDBs",
                c => new
                    {
                        AssetDBId = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        PriceCurrency = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssetDBId);
            
            CreateTable(
                "dbo.InterestRatesTypes",
                c => new
                    {
                        RateDBId = c.Int(nullable: false, identity: true),
                        rate = c.Int(nullable: false),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.RateDBId);
            
            CreateTable(
                "dbo.Cash",
                c => new
                    {
                        date = c.DateTime(nullable: false),
                        value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.date);
            
            CreateTable(
                "dbo.CorrelMat",
                c => new
                    {
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.date);
            
            CreateTable(
                "dbo.LastConnectionDBs",
                c => new
                    {
                        LastConnectionDBId = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LastConnectionDBId);
            
            CreateTable(
                "dbo.Portfolio Composition",
                c => new
                    {
                        AssetDBId = c.Int(nullable: false),
                        date = c.DateTime(nullable: false),
                        quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.AssetDBId, t.date })
                .ForeignKey("dbo.AssetDBs", t => t.AssetDBId, cascadeDelete: true)
                .Index(t => t.AssetDBId);
            
            CreateTable(
                "dbo.HedgingPortfolio",
                c => new
                    {
                        date = c.DateTime(nullable: false),
                        value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.date);
            
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        AssetDBId = c.Int(nullable: false),
                        date = c.DateTime(nullable: false),
                        price = c.Double(nullable: false),
                        priceEur = c.Double(nullable: false),
                        open = c.Double(nullable: false),
                        openEur = c.Double(nullable: false),
                        high = c.Double(nullable: false),
                        highEur = c.Double(nullable: false),
                        low = c.Double(nullable: false),
                        lowEur = c.Double(nullable: false),
                        volume = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.AssetDBId, t.date })
                .ForeignKey("dbo.AssetDBs", t => t.AssetDBId, cascadeDelete: true)
                .Index(t => t.AssetDBId);
            
            CreateTable(
                "dbo.InterestRatesValues",
                c => new
                    {
                        RateDBId = c.Int(nullable: false),
                        date = c.DateTime(nullable: false),
                        value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.RateDBId, t.date })
                .ForeignKey("dbo.InterestRatesTypes", t => t.RateDBId, cascadeDelete: true)
                .Index(t => t.RateDBId);
            
            CreateTable(
                "dbo.Everglades",
                c => new
                    {
                        AssetDBId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssetDBId)
                .ForeignKey("dbo.AssetDBs", t => t.AssetDBId)
                .Index(t => t.AssetDBId);
            
            CreateTable(
                "dbo.Equities",
                c => new
                    {
                        AssetDBId = c.Int(nullable: false),
                        symbol = c.String(),
                    })
                .PrimaryKey(t => t.AssetDBId)
                .ForeignKey("dbo.AssetDBs", t => t.AssetDBId)
                .Index(t => t.AssetDBId);
            
            CreateTable(
                "dbo.Forex",
                c => new
                    {
                        AssetDBId = c.Int(nullable: false),
                        forex = c.Int(nullable: false),
                        RateDBId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssetDBId)
                .ForeignKey("dbo.AssetDBs", t => t.AssetDBId)
                .ForeignKey("dbo.InterestRatesTypes", t => t.RateDBId, cascadeDelete: true)
                .Index(t => t.AssetDBId)
                .Index(t => t.RateDBId);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        AssetDBId = c.Int(nullable: false),
                        underlying_AssetDBId = c.Int(),
                        underlyingId = c.Int(nullable: false),
                        maturity = c.DateTime(nullable: false),
                        strike = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.AssetDBId)
                .ForeignKey("dbo.AssetDBs", t => t.AssetDBId)
                .ForeignKey("dbo.Equities", t => t.underlying_AssetDBId)
                .Index(t => t.AssetDBId)
                .Index(t => t.underlying_AssetDBId);
            
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
            DropTable("dbo.CorrelMat");
            DropTable("dbo.Cash");
            DropTable("dbo.InterestRatesTypes");
            DropTable("dbo.AssetDBs");
        }
    }
}
