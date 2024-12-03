using FluentMigrator;

namespace Mobi.Repository.Migrations
{
    [Migration(20241126001)] // A unique timestamp-based version number
    public class SchemaMigration : Migration
    {
        public override void Up()
        {         
            if (!Schema.Table("Companys").Exists())
            {
                // Create the Companys table
                Create.Table("Companys")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("CompanyName").AsString(255).NotNullable()
                    .WithColumn("CompanyId").AsInt32().NotNullable()
                    .WithColumn("CreatedDate").AsDate().NotNullable();
            }

            if (!Schema.Table("SystemUsers").Exists())
            {
                // Create the SystemUsers table
                Create.Table("SystemUsers")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("EmployeeName").AsString(255).NotNullable()
                    .WithColumn("UserName").AsString(255).NotNullable()
                    .WithColumn("UserStatus").AsString(50).NotNullable()
                    .WithColumn("Password").AsString(255).NotNullable()
                    .WithColumn("CompanyID").AsInt32().NotNullable()
                    .WithColumn("CreatedDate").AsDate().NotNullable()
                    .WithColumn("Deleted").AsBoolean().NotNullable();

                // Add foreign key only if the table was created
                Create.ForeignKey("FK_SystemUsers_Companys")
                    .FromTable("SystemUsers").ForeignColumn("CompanyID")
                    .ToTable("Companys").PrimaryColumn("Id");
            }

        }

        public override void Down()
        {
            // Drop foreign keys first
            if (Schema.Table("SystemUsers").Exists())
            {
                Delete.ForeignKey("FK_SystemUsers_Companys").OnTable("SystemUsers");
                Delete.Table("SystemUsers");
            }

            if (Schema.Table("Companys").Exists())
            {
                Delete.Table("Companys");
            }
        }
    }
}
