using Microsoft.EntityFrameworkCore;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Data.Mapping;
public class ApplicationContext : DbContext
{
    //public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    // DbSet properties
    public DbSet<Company> Company { get; set; }
    public DbSet<SystemUsers> SystemUsers { get; set; }
    public DbSet<Employee> Employee { get; set; }
    public DbSet<EmployeeAttendanceLogs> EmployeeAttendanceLogs { get; set; }
    public DbSet<SystemUserAuthorityMapping> SystemUserAuthorityMapping { get; set; }
    public DbSet<EmployeeLocation> EmployeeLocation { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<LocationBeaconMapping> LocationBeaconMapping { get; set; }
    public DbSet<Language> Language { get; set; }
    public DbSet<LocaleStringResource> LocaleStringResource { get; set; }
    public DbSet<Picture> Picture { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        // Apply entity mappings
        new CompanysMap(modelBuilder.Entity<Company>());
        new SystemUsersMap(modelBuilder.Entity<SystemUsers>());
        new EmployeeMap(modelBuilder.Entity<Employee>());
        new LocationMap(modelBuilder.Entity<Location>());
        new LocationBeaconMappingMap(modelBuilder.Entity<LocationBeaconMapping>());
        new EmployeeAttendanceLogsMap(modelBuilder.Entity<EmployeeAttendanceLogs>());
        new SystemUserAuthorityMappingMap(modelBuilder.Entity<SystemUserAuthorityMapping>());
        new EmployeeLocationMap(modelBuilder.Entity<EmployeeLocation>());
        new LanguageMap(modelBuilder.Entity<Language>());
        new LocaleStringResourceMap(modelBuilder.Entity<LocaleStringResource>());
        new PictureMap(modelBuilder.Entity<Picture>());

        modelBuilder.Entity<Company>().HasData(
           new Company
           {
               Id=1,
               CompanyName="Mobi",
               CompanyId= "78951",
               CreatedDate= DateTime.Now
           }
        );
        // Seed data for Language
        modelBuilder.Entity<Language>().HasData(
             new Language
             {
                 Id = 1,
                 LanguageName = "English",
                 UniqueSeoCode = "en",
                 Published = true,
                 DisplayOrder = 1
             },
             new Language
             {
                 Id = 2,
                 LanguageName = "Arabic",
                 UniqueSeoCode = "ar",
                 Published = true,
                 DisplayOrder = 2
             }
         );

        // Seed data for LocaleStringResource
        modelBuilder.Entity<LocaleStringResource>().HasData(
            new LocaleStringResource
            {
                Id = 1,
                ResourceName = "Mobi.Test",
                ResourceValue = "This is English : Welcome to Mobi",
                LanguageId = 1
            },
            new LocaleStringResource
            {
                Id = 2,
                ResourceName = "Mobi.Test",
                ResourceValue = "This is Arabic : مرحبا بكم في موبي",
                LanguageId = 2
            }
        );

        // Seed data for SystemUsers
        modelBuilder.Entity<SystemUsers>().HasData(
            new SystemUsers
            {
                Id=1,
                EmployeeName = "Mobi",
                UserName = "Superadmin",
                Email = "superadmin@mobiitend.com",
                UserStatus = true,
                Password = BCrypt.Net.BCrypt.HashPassword("Mobitend@2025"),
                CompanyID = 1,
                CreatedDate = DateTime.Now,
                Deleted = false
            }
        );

        // Seed data for SystemUserAuthorityMapping
        modelBuilder.Entity<SystemUserAuthorityMapping>().HasData(
            new SystemUserAuthorityMapping { Id = 1, SystemUserID = 1, ScreenAuthority = "Locations", ScreenAuthoritySystemName = "locations" },
            new SystemUserAuthorityMapping { Id = 2, SystemUserID = 1, ScreenAuthority = "EmployeeAttendance", ScreenAuthoritySystemName = "employeeattendance" },
            new SystemUserAuthorityMapping { Id = 3, SystemUserID = 1, ScreenAuthority = "Employees", ScreenAuthoritySystemName = "employees" },
            new SystemUserAuthorityMapping { Id = 4, SystemUserID = 1, ScreenAuthority = "MobileManage", ScreenAuthoritySystemName = "mobilemanage" },
            new SystemUserAuthorityMapping { Id = 5, SystemUserID = 1, ScreenAuthority = "EmployeeLocation", ScreenAuthoritySystemName = "employeelocation" },
            new SystemUserAuthorityMapping { Id = 6, SystemUserID = 1, ScreenAuthority = "SystemUsers", ScreenAuthoritySystemName = "systemusers" },
            new SystemUserAuthorityMapping { Id = 7, SystemUserID = 1, ScreenAuthority = "Reports", ScreenAuthoritySystemName = "reports" },
            new SystemUserAuthorityMapping { Id = 8, SystemUserID = 1, ScreenAuthority = "ControlACL", ScreenAuthoritySystemName = "controlacl" }
        );


    }
}
