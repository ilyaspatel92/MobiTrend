﻿using Microsoft.EntityFrameworkCore;
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
               CompanyId= "3232cb15-cd04-4a57-b8ea-46dfda925f98",
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

    }
}
