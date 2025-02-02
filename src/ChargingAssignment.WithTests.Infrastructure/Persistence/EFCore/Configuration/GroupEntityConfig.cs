using CharginAssignment.WithTests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Configuration;

public class GroupEntityConfig : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.ToTable(TableNameConst.Groups);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(500);
    }
}