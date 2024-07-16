﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Data;

#nullable disable

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240709122514_UpdateUsersRoleRelationship")]
    partial class UpdateUsersRoleRelationship
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.CustomersModel", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("CustomerEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerPhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.EventRegistrationModel", b =>
                {
                    b.Property<int>("RegistrationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RegistrationId"));

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.HasKey("RegistrationId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("EventId");

                    b.ToTable("EventRegistrations");
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.EventsModel", b =>
                {
                    b.Property<int>("EventID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EventID"));

                    b.Property<string>("EventDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EventEndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EventStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImgUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberRegistrations")
                        .HasColumnType("int");

                    b.HasKey("EventID");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.RolesModel", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("Permissions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = 1,
                            Permissions = "[\"C\\u00F4ng vi\\u1EC7c A\",\"C\\u00F4ng vi\\u1EC7c C\"]",
                            RoleName = "Quản lý"
                        },
                        new
                        {
                            RoleId = 2,
                            Permissions = "[\"C\\u00F4ng vi\\u1EC7c B\",\"C\\u00F4ng vi\\u1EC7c D\"]",
                            RoleName = "Trưởng phòng"
                        });
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.UsersModel", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Address = "Bình Dương",
                            Password = "IP/yw120FvWx13bNsVOPMGYHxop2ubZh2Fpzz6gVgm4=",
                            PhoneNumber = "0332613703",
                            ProfilePicture = "",
                            RoleId = 1,
                            UserEmail = "AnhTu080302@gmail.com",
                            UserName = "Nguyễn Anh Tú"
                        },
                        new
                        {
                            UserId = 2,
                            Address = "Bình Dương",
                            Password = "GjPebaQmwDWPA0/xh4gBON4Y6bndNyrJ1b5l0iycUzo=",
                            PhoneNumber = "0123456789",
                            ProfilePicture = "",
                            RoleId = 2,
                            UserEmail = "XT123@gmail.com",
                            UserName = "Mai Xuân Tiền"
                        });
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.EventRegistrationModel", b =>
                {
                    b.HasOne("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.CustomersModel", "Customers")
                        .WithMany("Registrations")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.EventsModel", "Event")
                        .WithMany("Registrations")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customers");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.UsersModel", b =>
                {
                    b.HasOne("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.RolesModel", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.CustomersModel", b =>
                {
                    b.Navigation("Registrations");
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.EventsModel", b =>
                {
                    b.Navigation("Registrations");
                });

            modelBuilder.Entity("QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models.RolesModel", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
