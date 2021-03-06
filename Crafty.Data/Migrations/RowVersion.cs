namespace Crafty.Data.Migrations
{
  using System;
  using System.Data.Entity.Migrations;

  public partial class RowVersion : DbMigration
  {
    public override void Up()
    {
      AddColumn("dbo.Items", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
    }

    public override void Down()
    {
      DropColumn("dbo.Items", "RowVersion");
    }
  }
}
