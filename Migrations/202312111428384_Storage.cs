namespace Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Storage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Storages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.String(maxLength: 40, unicode: false),
                        Quantity = c.String(),
                        Price = c.Double(),
                        Unit = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Storages", "ProductId", "dbo.Products");
            DropIndex("dbo.Storages", new[] { "ProductId" });
            DropTable("dbo.Storages");
        }
    }
}
