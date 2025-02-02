﻿// <auto-generated />
using System;
using CharginAssignment.WithTests.Infrastructure.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CharginAssignment.WithTests.Infrastructure.Persistence.EFCore.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CharginAssignment.WithTests.Domain.Entities.ChargeStationEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("ChargeStations", (string)null);
                });

            modelBuilder.Entity("CharginAssignment.WithTests.Domain.Entities.GroupEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.ToTable("Groups", (string)null);
                });

            modelBuilder.Entity("CharginAssignment.WithTests.Domain.Entities.ChargeStationEntity", b =>
                {
                    b.HasOne("CharginAssignment.WithTests.Domain.Entities.GroupEntity", "Group")
                        .WithMany("ChargeStations")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("CharginAssignment.WithTests.Domain.Entities.ConnectorEntity", "Connectors", b1 =>
                        {
                            b1.Property<Guid>("ChargeStationId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<int>("MaxCurrent")
                                .HasColumnType("int");

                            b1.HasKey("ChargeStationId", "Id");

                            b1.ToTable("Connectors", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("ChargeStationId");
                        });

                    b.Navigation("Connectors");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("CharginAssignment.WithTests.Domain.Entities.GroupEntity", b =>
                {
                    b.Navigation("ChargeStations");
                });
#pragma warning restore 612, 618
        }
    }
}
