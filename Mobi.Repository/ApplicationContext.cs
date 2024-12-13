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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        // Apply entity mappings
        new CompanysMap(modelBuilder.Entity<Company>());
        new SystemUsersMap(modelBuilder.Entity<SystemUsers>());
        new EmployeeMap(modelBuilder.Entity<Employee>());
        new EmployeeAttendanceLogsMap(modelBuilder.Entity<EmployeeAttendanceLogs>());
        new SystemUserAuthorityMappingMap(modelBuilder.Entity<SystemUserAuthorityMapping>());
        new EmployeeLocationMap(modelBuilder.Entity<EmployeeLocation>());
        new LocationMap(modelBuilder.Entity<Location>());
        new LocationBeaconMappingMap(modelBuilder.Entity<LocationBeaconMapping>());
        new LanguageMap(modelBuilder.Entity<Language>());
        new LocaleStringResourceMap(modelBuilder.Entity<LocaleStringResource>());

    }
}
