namespace Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spec : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Specifications", "Screen", c => c.String(maxLength: 4000));
            AddColumn("dbo.Specifications", "Camera", c => c.String(maxLength: 4000));
            AddColumn("dbo.Specifications", "CameraSelfie", c => c.String(maxLength: 4000));
            AddColumn("dbo.Specifications", "CPU", c => c.String(maxLength: 4000));
            AddColumn("dbo.Specifications", "Battery", c => c.String(maxLength: 4000));
            AddColumn("dbo.Specifications", "Origin", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Specifications", "Origin");
            DropColumn("dbo.Specifications", "Battery");
            DropColumn("dbo.Specifications", "CPU");
            DropColumn("dbo.Specifications", "CameraSelfie");
            DropColumn("dbo.Specifications", "Camera");
            DropColumn("dbo.Specifications", "Screen");
        }
    }
}
