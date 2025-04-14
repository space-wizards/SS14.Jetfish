﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SS14.Jetfish.Database;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250414233000_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SS14.Jetfish.Database.Model.Policy.AccessPolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.PrimitiveCollection<int[]>("AccessAreas")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.ToTable("AccessPolicy");
                });

            modelBuilder.Entity("SS14.Jetfish.Database.Model.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("SS14.Jetfish.Database.Model.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("SS14.Jetfish.Database.Model.Team", b =>
                {
                    b.OwnsMany("SS14.Jetfish.Database.Model.Policy.ResourcePolicy", "ResourcePolicies", b1 =>
                        {
                            b1.Property<Guid>("TeamId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<int>("AccessPolicyId")
                                .HasColumnType("integer");

                            b1.Property<Guid>("ResourceId")
                                .HasColumnType("uuid");

                            b1.HasKey("TeamId", "Id");

                            b1.HasIndex("AccessPolicyId");

                            b1.ToTable("Team_ResourcePolicies");

                            b1.HasOne("SS14.Jetfish.Database.Model.Policy.AccessPolicy", "AccessPolicy")
                                .WithMany()
                                .HasForeignKey("AccessPolicyId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.WithOwner()
                                .HasForeignKey("TeamId");

                            b1.Navigation("AccessPolicy");
                        });

                    b.Navigation("ResourcePolicies");
                });

            modelBuilder.Entity("SS14.Jetfish.Database.Model.User", b =>
                {
                    b.OwnsMany("SS14.Jetfish.Database.Model.Policy.ResourcePolicy", "ResourcePolicies", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<int>("AccessPolicyId")
                                .HasColumnType("integer");

                            b1.Property<Guid>("ResourceId")
                                .HasColumnType("uuid");

                            b1.HasKey("UserId", "Id");

                            b1.HasIndex("AccessPolicyId");

                            b1.ToTable("User_ResourcePolicies");

                            b1.HasOne("SS14.Jetfish.Database.Model.Policy.AccessPolicy", "AccessPolicy")
                                .WithMany()
                                .HasForeignKey("AccessPolicyId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.WithOwner()
                                .HasForeignKey("UserId");

                            b1.Navigation("AccessPolicy");
                        });

                    b.Navigation("ResourcePolicies");
                });
#pragma warning restore 612, 618
        }
    }
}
