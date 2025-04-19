﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SS14.Jetfish.Database;

#nullable disable

namespace SS14.Jetfish.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ProjectTeam", b =>
                {
                    b.Property<Guid>("ProjectsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("ProjectsId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("ProjectTeam");
                });

            modelBuilder.Entity("SS14.Jetfish.FileHosting.Model.FileUsage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CardId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UploadedFileId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CardId")
                        .IsUnique();

                    b.HasIndex("ProjectId")
                        .IsUnique();

                    b.HasIndex("UploadedFileId");

                    b.ToTable("FileUsage");
                });

            modelBuilder.Entity("SS14.Jetfish.FileHosting.Model.UploadedFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Etag")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<DateTimeOffset>("LastModified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(180)
                        .HasColumnType("character varying(180)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<string>("RelativePath")
                        .IsRequired()
                        .HasMaxLength(260)
                        .HasColumnType("character varying(260)");

                    b.Property<Guid>("UploadedById")
                        .HasColumnType("uuid");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UploadedById");

                    b.ToTable("UploadedFile");
                });

            modelBuilder.Entity("SS14.Jetfish.Projects.Model.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("SS14.Jetfish.Projects.Model.Lane", b =>
                {
                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<int>("LaneId")
                        .HasColumnType("integer");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("ProjectId", "LaneId");

                    b.ToTable("Lane");
                });

            modelBuilder.Entity("SS14.Jetfish.Projects.Model.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.AccessPolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.PrimitiveCollection<short[]>("Permissions")
                        .IsRequired()
                        .HasColumnType("smallint[]");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AccessPolicies");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("IdpName")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uuid");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.TeamMember", b =>
                {
                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("integer");

                    b.HasKey("TeamId", "UserId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("TeamMember");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ProjectTeam", b =>
                {
                    b.HasOne("SS14.Jetfish.Projects.Model.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SS14.Jetfish.Security.Model.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SS14.Jetfish.FileHosting.Model.FileUsage", b =>
                {
                    b.HasOne("SS14.Jetfish.Projects.Model.Card", "Card")
                        .WithOne()
                        .HasForeignKey("SS14.Jetfish.FileHosting.Model.FileUsage", "CardId");

                    b.HasOne("SS14.Jetfish.Projects.Model.Project", "Project")
                        .WithOne()
                        .HasForeignKey("SS14.Jetfish.FileHosting.Model.FileUsage", "ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SS14.Jetfish.FileHosting.Model.UploadedFile", null)
                        .WithMany("Usages")
                        .HasForeignKey("UploadedFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("SS14.Jetfish.FileHosting.Model.UploadedFile", b =>
                {
                    b.HasOne("SS14.Jetfish.Security.Model.User", "UploadedBy")
                        .WithMany()
                        .HasForeignKey("UploadedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UploadedBy");
                });

            modelBuilder.Entity("SS14.Jetfish.Projects.Model.Card", b =>
                {
                    b.HasOne("SS14.Jetfish.Security.Model.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("SS14.Jetfish.Projects.Model.Lane", b =>
                {
                    b.HasOne("SS14.Jetfish.Projects.Model.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.Role", b =>
                {
                    b.HasOne("SS14.Jetfish.Security.Model.Team", null)
                        .WithMany("Roles")
                        .HasForeignKey("TeamId");

                    b.OwnsMany("SS14.Jetfish.Security.Model.ResourcePolicy", "Policies", b1 =>
                        {
                            b1.Property<Guid>("RoleId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<int>("AccessPolicyId")
                                .HasColumnType("integer");

                            b1.Property<bool>("Global")
                                .HasColumnType("boolean");

                            b1.Property<Guid?>("ResourceId")
                                .HasColumnType("uuid");

                            b1.HasKey("RoleId", "Id");

                            b1.HasIndex("AccessPolicyId");

                            b1.ToTable("Role_Policies");

                            b1.HasOne("SS14.Jetfish.Security.Model.AccessPolicy", "AccessPolicy")
                                .WithMany()
                                .HasForeignKey("AccessPolicyId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.WithOwner()
                                .HasForeignKey("RoleId");

                            b1.Navigation("AccessPolicy");
                        });

                    b.Navigation("Policies");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.TeamMember", b =>
                {
                    b.HasOne("SS14.Jetfish.Security.Model.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SS14.Jetfish.Security.Model.Team", "Team")
                        .WithMany("TeamMembers")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SS14.Jetfish.Security.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.User", b =>
                {
                    b.OwnsMany("SS14.Jetfish.Security.Model.ResourcePolicy", "ResourcePolicies", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<int>("AccessPolicyId")
                                .HasColumnType("integer");

                            b1.Property<bool>("Global")
                                .HasColumnType("boolean");

                            b1.Property<Guid?>("ResourceId")
                                .HasColumnType("uuid");

                            b1.HasKey("UserId", "Id");

                            b1.HasIndex("AccessPolicyId");

                            b1.ToTable("User_ResourcePolicies");

                            b1.HasOne("SS14.Jetfish.Security.Model.AccessPolicy", "AccessPolicy")
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

            modelBuilder.Entity("SS14.Jetfish.FileHosting.Model.UploadedFile", b =>
                {
                    b.Navigation("Usages");
                });

            modelBuilder.Entity("SS14.Jetfish.Security.Model.Team", b =>
                {
                    b.Navigation("Roles");

                    b.Navigation("TeamMembers");
                });
#pragma warning restore 612, 618
        }
    }
}
