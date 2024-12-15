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
                    .WithColumn("CompanyId").AsString(255).NotNullable()
                    .WithColumn("CreatedDate").AsDateTime().NotNullable();
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
                    .WithColumn("CompanyID").AsString(255).NotNullable()
                    .WithColumn("CreatedDate").AsDateTime().NotNullable()
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
                    .WithColumn("CompanyId").AsString(255).NotNullable()
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
                    .WithColumn("CID").AsString(255).Nullable()
                    .WithColumn("CreatedDate").AsDateTime().NotNullable();

                Create.ForeignKey("FK_Employee_Companys")
                    .FromTable("Employee").ForeignColumn("CompanyId")
                    .ToTable("Companys").PrimaryColumn("Id");
            }

            if (!Schema.Table("EmployeeAttendanceLogs").Exists())
            {
                // Create the EmployeeAttendanceLogs table
                Create.Table("EmployeeAttendanceLogs")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("EmployeeId").AsInt32().NotNullable()
                    .WithColumn("DateandTime").AsDateTime().NotNullable()
                    .WithColumn("TransTypeId").AsInt32().NotNullable()
                    .WithColumn("CurrentLocation").AsString(255).NotNullable()
                    .WithColumn("ProofTypeId").AsInt32().NotNullable()
                    .WithColumn("Latitude").AsDecimal().NotNullable()
                    .WithColumn("Longtitude").AsDecimal().NotNullable()
                    .WithColumn("MobileSerialNumber").AsString().NotNullable()
                    .WithColumn("Photo").AsString().NotNullable()
                    .WithColumn("LocationId").AsInt32().NotNullable()
                    .WithColumn("Transferred").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("TransferTime").AsDateTime().NotNullable()
                    .WithColumn("LocationBeaconMappingId").AsInt32().NotNullable();

                // Add foreign key only if the table was created
                Create.ForeignKey("FK_EmployeeAttendanceLogs_LocationBeaconMappingId")
                    .FromTable("EmployeeAttendanceLogs").ForeignColumn("LocationBeaconMappingId")
                    .ToTable("LocationBeaconMapping").PrimaryColumn("Id");
            }

            if (!Schema.Table("SystemUserAuthorityMapping").Exists())
            {
                // Create the EmployeeAttendanceLogs table
                Create.Table("SystemUserAuthorityMapping")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("SystemUserID").AsInt32().NotNullable()
                    .WithColumn("ScreenAuthority").AsString().NotNullable()
                    .WithColumn("ScreenAuthoritySystemName").AsString().NotNullable();

                // Add foreign key only if the table was created
                Create.ForeignKey("FK_SystemUserAuthorityMapping_SystemUserID")
                    .FromTable("SystemUserAuthorityMapping").ForeignColumn("SystemUserID")
                    .ToTable("SystemUsers").PrimaryColumn("Id");
            }

            if (!Schema.Table("EmployeeLocation").Exists())
            {
                // Create the EmployeeLocation table
                Create.Table("EmployeeLocation")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("EmployeeId").AsInt32().NotNullable()
                    .WithColumn("LocationId").AsInt32().NotNullable();

                // Add foreign key only if the table was created
                Create.ForeignKey("FK_EmployeeLocation_EmployeeId")
                    .FromTable("EmployeeLocation").ForeignColumn("EmployeeId")
                    .ToTable("Employee").PrimaryColumn("Id");
            }

            if (!Schema.Table("Location").Exists())
            {
                // Create the Location table
                Create.Table("Location")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("LocationNameEnglish").AsString(2000).NotNullable()
                    .WithColumn("LocationNameArabic").AsString(2000).NotNullable()
                    .WithColumn("Status").AsBoolean().NotNullable().WithDefaultValue(false)
                    .WithColumn("BeaconProof").AsInt32().NotNullable()
                    .WithColumn("GPSProof").AsInt32().NotNullable()
                    .WithColumn("CreatedDate").AsDateTime().NotNullable()
                    .WithColumn("GPSLocationAddress").AsString(2000).Nullable()
                    .WithColumn("Latitude").AsDecimal().Nullable()
                    .WithColumn("Longtitude").AsDecimal().Nullable()
                    .WithColumn("SetRadius").AsDecimal().Nullable()
                    .WithColumn("CompanyId").AsDecimal().Nullable();

                // Add foreign key only if the table was created
                Create.ForeignKey("FK_Location_CompanyId")
                    .FromTable("Location").ForeignColumn("CompanyId")
                    .ToTable("Companys").PrimaryColumn("Id");
            }

            if (!Schema.Table("LocationBeaconMapping").Exists())
            {
                // Create the EmployeeAttendanceLogs table
                Create.Table("LocationBeaconMapping")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("LocationId").AsInt32().NotNullable()
                    .WithColumn("BeaconName").AsString(2000).NotNullable()
                    .WithColumn("UUID").AsString(2000).NotNullable()
                    .WithColumn("Status").AsBoolean().NotNullable().WithDefaultValue(false);

                // Add foreign key only if the table was created
                Create.ForeignKey("FK_LocationBeaconMapping_LocationId")
                    .FromTable("LocationBeaconMapping").ForeignColumn("LocationId")
                    .ToTable("Location").PrimaryColumn("Id");
            }

            if (!Schema.Table("Language").Exists())
            {
                // Create the EmployeeAttendanceLogs table
                Create.Table("Language")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("LanguageName").AsString(2000).NotNullable()
                    .WithColumn("DisplayOrder").AsInt32().NotNullable()
                    .WithColumn("UniqueSeoCode").AsString(2000).NotNullable()
                    .WithColumn("Published").AsBoolean().NotNullable().WithDefaultValue(false);

                // Insert the language records
                Execute.Sql("INSERT INTO Language (LanguageName, UniqueSeoCode, Published, DisplayOrder) VALUES ('English', 'en', 1, 1)");
                Execute.Sql("INSERT INTO Language (LanguageName, UniqueSeoCode, Published, DisplayOrder) VALUES ('Arabic', 'ar', 1, 2)");


            }

            if (!Schema.Table("LocaleStringResource").Exists())
            {
                // Create the EmployeeAttendanceLogs table
                Create.Table("LocaleStringResource")
                    .WithColumn("Id").AsInt32().PrimaryKey().Identity() // Auto-increment primary key
                    .WithColumn("ResourceName").AsString(2000).NotNullable()
                    .WithColumn("ResourceValue").AsString(2000).NotNullable()
                    .WithColumn("LanguageId").AsInt32().NotNullable();

                // Add foreign key only if the table was created
                Create.ForeignKey("FK_LocaleStringResource_LanguageId")
                    .FromTable("LocaleStringResource").ForeignColumn("LanguageId")
                    .ToTable("Language").PrimaryColumn("Id");


                // Insert values into LocaleStringResource
                Execute.Sql("INSERT INTO LocaleStringResource (ResourceName, ResourceValue, LanguageId) VALUES ('Mobi.Test', 'This is English', 1)");
                Execute.Sql("INSERT INTO LocaleStringResource (ResourceName, ResourceValue, LanguageId) VALUES ('Mobi.Test', 'This is Arabic', 2)");
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

            if (Schema.Table("Employee").Exists())
            {
                Delete.Table("Employee");
            }

            if (Schema.Table("EmployeeAttendanceLogs").Exists())
            {
                Delete.Table("EmployeeAttendanceLogs");
            }
            if (Schema.Table("SystemUserAuthorityMapping").Exists())
            {
                Delete.Table("SystemUserAuthorityMapping");
            }
            if (Schema.Table("EmployeeLocation").Exists())
            {
                Delete.Table("EmployeeLocation");
            }
            if (Schema.Table("Location").Exists())
            {
                Delete.Table("Location");
            }
            if (Schema.Table("LocationBeaconMapping").Exists())
            {
                Delete.Table("LocationBeaconMapping");
            }
            if (Schema.Table("Language").Exists())
            {
                Delete.Table("Language");
            }
            if (Schema.Table("LocaleStringResource").Exists())
            {
                Delete.Table("LocaleStringResource");
            }
        }
    }
}
