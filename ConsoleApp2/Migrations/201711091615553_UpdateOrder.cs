namespace ConsoleApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOrder : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "CustomerID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "CustomerID", c => c.Int(nullable: false));
        }
    }
}
