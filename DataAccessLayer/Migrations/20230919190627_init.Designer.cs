﻿// <auto-generated />
using System;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(ClinicaDbContext))]
    [Migration("20230919190627_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DataAccessLayer.Model.AppointmentModel", b =>
                {
                    b.Property<int>("AppointId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointId"));

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DoctorMsg")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DoctorUserId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<string>("PDFFile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PatientMsg")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PatientUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("info")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AppointId");

                    b.HasIndex("DoctorUserId");

                    b.HasIndex("PatientUserId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("DataAccessLayer.Model.ImageUser", b =>
                {
                    b.Property<int>("ImgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImgId"));

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ImgId");

                    b.HasIndex("UserId");

                    b.ToTable("ImgUser");
                });

            modelBuilder.Entity("DataAccessLayer.Model.Message", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MessageId"));

                    b.Property<int?>("AppointId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeSend")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("MessageId");

                    b.HasIndex("AppointId");

                    b.HasIndex("UserId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("DataAccessLayer.Model.UserModel", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateATT")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasDiscriminator<string>("Discriminator").HasValue("UserModel");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("DataAccessLayer.Model.DoctorModel", b =>
                {
                    b.HasBaseType("DataAccessLayer.Model.UserModel");

                    b.Property<string>("AdInfo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Especialization")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("Fees")
                        .HasColumnType("real");

                    b.Property<string>("Region")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("DoctorModel");
                });

            modelBuilder.Entity("DataAccessLayer.Model.PatientModel", b =>
                {
                    b.HasBaseType("DataAccessLayer.Model.UserModel");

                    b.HasDiscriminator().HasValue("PatientModel");
                });

            modelBuilder.Entity("DataAccessLayer.Model.AppointmentModel", b =>
                {
                    b.HasOne("DataAccessLayer.Model.DoctorModel", "Doctor")
                        .WithMany()
                        .HasForeignKey("DoctorUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Model.PatientModel", "Patient")
                        .WithMany()
                        .HasForeignKey("PatientUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("DataAccessLayer.Model.ImageUser", b =>
                {
                    b.HasOne("DataAccessLayer.Model.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataAccessLayer.Model.Message", b =>
                {
                    b.HasOne("DataAccessLayer.Model.AppointmentModel", "AppointID")
                        .WithMany("Messages")
                        .HasForeignKey("AppointId");

                    b.HasOne("DataAccessLayer.Model.UserModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppointID");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataAccessLayer.Model.AppointmentModel", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
