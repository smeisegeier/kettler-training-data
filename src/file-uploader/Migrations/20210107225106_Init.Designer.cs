﻿// <auto-generated />
using System;
using FileUploader.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FileUploader.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20210107225106_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("kettler")
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("FileUploader.Models.Record", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Power")
                        .HasColumnType("int");

                    b.Property<int>("Pulse")
                        .HasColumnType("int");

                    b.Property<int>("RPM")
                        .HasColumnType("int");

                    b.Property<double>("Score_10sec")
                        .HasColumnType("float");

                    b.Property<double>("TimePassed_minutes")
                        .HasColumnType("float");

                    b.Property<double>("TimePassed_percent")
                        .HasColumnType("float");

                    b.Property<int?>("TrainingId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TrainingId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("FileUploader.Models.Training", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Calibration")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Date")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Device")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("Duration_minutes")
                        .HasColumnType("float");

                    b.Property<string>("Energy")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Power_Avg")
                        .HasColumnType("float");

                    b.Property<double>("RPM_Avg")
                        .HasColumnType("float");

                    b.Property<string>("RecordIntervall")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Software")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Streak_days")
                        .HasColumnType("int");

                    b.Property<string>("Time")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TrainingDateTime")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Transmission")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("FileName")
                        .IsUnique()
                        .HasFilter("[FileName] IS NOT NULL")
                        .IsClustered(false);

                    b.ToTable("Trainings");
                });

            modelBuilder.Entity("FileUploader.Models.Record", b =>
                {
                    b.HasOne("FileUploader.Models.Training", null)
                        .WithMany("Records")
                        .HasForeignKey("TrainingId");
                });

            modelBuilder.Entity("FileUploader.Models.Training", b =>
                {
                    b.Navigation("Records");
                });
#pragma warning restore 612, 618
        }
    }
}