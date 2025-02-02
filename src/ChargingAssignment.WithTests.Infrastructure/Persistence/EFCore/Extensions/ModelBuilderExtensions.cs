using CharginAssignment.WithTests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Extensions;

public static class ModelBuilderExtensions
{
    public static void AutoRegisterDbSets(this ModelBuilder modelBuilder, Assembly entitiesAssembly)
    {
        IEnumerable<Type> entityTypes = entitiesAssembly
            .GetExportedTypes()
            .Where(type => type.IsClass &&
                           !type.IsAbstract &&
                           typeof(IEntity).IsAssignableFrom(type))
            .ToList();

        foreach (var entity in entityTypes)
        {
            modelBuilder.Entity(entity);
        }
    }

    public static void SetRestrictAsDefaultDeleteBehavior(this ModelBuilder modelBuilder)
    {
        IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (IMutableForeignKey fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}