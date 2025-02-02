using CharginAssignment.WithTests.Domain;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Auto Register Entities (using IEntity marker)
        modelBuilder.AutoRegisterDbSets(typeof(DomainAssemblyEntryPoint).Assembly);

        base.OnModelCreating(modelBuilder);

        // Auto Register Entity Configurations (Fluent API)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InfrastructureAssemblyEntryPoint).Assembly);
    }
}