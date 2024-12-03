using Microsoft.EntityFrameworkCore;
using Mobi.Data.Domain;
using Mobi.Data.Mapping;

public class ApplicationContext : DbContext
{
    public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    // DbSet properties
    public DbSet<Companys> Companys { get; set; }
    public DbSet<SystemUsers> SystemUsers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        // Apply entity mappings
        new CompanysMap(modelBuilder.Entity<Companys>());
        new SystemUsersMap(modelBuilder.Entity<SystemUsers>());
        
    }
}
