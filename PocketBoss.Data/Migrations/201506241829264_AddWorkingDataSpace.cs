namespace PocketBoss.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWorkingDataSpace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.workflow_WorkflowInstance", "WorkingDataStorage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.workflow_WorkflowInstance", "WorkingDataStorage");
        }
    }
}
