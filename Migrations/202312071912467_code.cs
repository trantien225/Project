namespace Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class code : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Code");
        }
    }
}
