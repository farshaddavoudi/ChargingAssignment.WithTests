using CharginAssignment.WithTests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Configuration;

public class ChargeStationEntityConfig : IEntityTypeConfiguration<ChargeStationEntity>
{
    public void Configure(EntityTypeBuilder<ChargeStationEntity> builder)
    {
        builder.ToTable(TableNameConst.ChargeStations);

        builder.Property(cs => cs.Name).IsRequired().HasMaxLength(500);

        builder.HasOne(cs => cs.Group)
            .WithMany(g => g.ChargeStations)
            .HasForeignKey(cs => cs.GroupId);

        builder.OwnsMany(cs => cs.Connectors, a =>
        {
            a.ToTable(TableNameConst.Connectors);

            a.Property(c => c.Id).ValueGeneratedNever();

            a.WithOwner().HasForeignKey("ChargeStationId");
        });
    }
}