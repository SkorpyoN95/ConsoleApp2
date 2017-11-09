namespace ConsoleApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Customer_CompanyName", c => c.String(maxLength: 128));
            CreateIndex("dbo.Orders", "ProductID");
            CreateIndex("dbo.Orders", "Customer_CompanyName");
            AddForeignKey("dbo.Orders", "ProductID", "dbo.Products", "ProductId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "Customer_CompanyName", "dbo.Customers", "CompanyName");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "Customer_CompanyName", "dbo.Customers");
            DropForeignKey("dbo.Orders", "ProductID", "dbo.Products");
            DropIndex("dbo.Orders", new[] { "Customer_CompanyName" });
            DropIndex("dbo.Orders", new[] { "ProductID" });
            DropColumn("dbo.Orders", "Customer_CompanyName");
        }
    }
}
