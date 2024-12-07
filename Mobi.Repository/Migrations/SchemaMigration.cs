using FluentMigrator;

namespace Mobi.Repository.Migrations
{
    [Migration(20241203001)] // A unique timestamp-based version number
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

            // Create Employee table
            if (!Schema.Table("Employee").Exists())
            {
                Create.Table("Employee")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                    .WithColumn("NameEng").AsString(255).NotNullable()
                    .WithColumn("NameArabic").AsString(255).Nullable()
                    .WithColumn("Status").AsBoolean().NotNullable().WithDefaultValue(true)
                    .WithColumn("CompanyId").AsInt32().NotNullable()
                    .WithColumn("FileNumber").AsString(100).Nullable()
                    .WithColumn("MobileNumber").AsString(20).Nullable()
                    .WithColumn("Email").AsString(255).Nullable()
                    .WithColumn("PhotoPath").AsString(500).Nullable()
                    .WithColumn("Password").AsString(255).NotNullable()
                    .WithColumn("UserName").AsString(255).NotNullable()
                    .WithColumn("MobileType").AsInt32().Nullable() // Enum for mobile type
                    .WithColumn("RegistrationVia").AsString(50).Nullable()
                    .WithColumn("DeviceId").AsString(255).Nullable()
                    .WithColumn("RegisterStatus").AsString(50).Nullable()
                    .WithColumn("NameEng").AsString(255).NotNullable()
                    .WithColumn("CID").AsString(255).Nullable();

                Create.ForeignKey("FK_Employee_Company")
                    .FromTable("Employee").ForeignColumn("CompanyId")
                    .ToTable("Company").PrimaryColumn("Id");
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

            if (Schema.Table("Company").Exists())
            {
                Delete.Table("Company");
            }
        }
    }
}
