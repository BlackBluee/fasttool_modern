﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace fasttool_modern.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241021171925_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("Action", b =>
                {
                    b.Property<string>("ActionID")
                        .HasMaxLength(16)
                        .HasColumnType("TEXT");

                    b.Property<string>("ButtonID")
                        .HasColumnType("TEXT");

                    b.Property<string>("DoAction")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileID")
                        .HasColumnType("TEXT");

                    b.Property<int>("Queue")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("ActionID");

                    b.HasIndex("ButtonID", "ProfileID");

                    b.ToTable("Actions");
                });

            modelBuilder.Entity("ButtonData", b =>
                {
                    b.Property<string>("ButtonID")
                        .HasMaxLength(16)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileID")
                        .HasColumnType("TEXT");

                    b.Property<string>("ActionID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeviceID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Image")
                        .HasColumnType("TEXT");

                    b.HasKey("ButtonID", "ProfileID");

                    b.HasIndex("DeviceID");

                    b.HasIndex("ProfileID");

                    b.ToTable("ButtonDatas");
                });

            modelBuilder.Entity("Device", b =>
                {
                    b.Property<string>("DeviceID")
                        .HasMaxLength(16)
                        .HasColumnType("TEXT");

                    b.Property<string>("Model")
                        .HasColumnType("TEXT");

                    b.Property<string>("Port")
                        .HasColumnType("TEXT");

                    b.Property<float>("Version")
                        .HasColumnType("REAL");

                    b.HasKey("DeviceID");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Profile", b =>
                {
                    b.Property<string>("ProfileID")
                        .HasMaxLength(16)
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfileName")
                        .HasColumnType("TEXT");

                    b.HasKey("ProfileID");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Action", b =>
                {
                    b.HasOne("ButtonData", "ButtonData")
                        .WithMany("Actions")
                        .HasForeignKey("ButtonID", "ProfileID");

                    b.Navigation("ButtonData");
                });

            modelBuilder.Entity("ButtonData", b =>
                {
                    b.HasOne("Device", "Device")
                        .WithMany("ButtonDatas")
                        .HasForeignKey("DeviceID");

                    b.HasOne("Profile", "Profile")
                        .WithMany("ButtonDatas")
                        .HasForeignKey("ProfileID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("ButtonData", b =>
                {
                    b.Navigation("Actions");
                });

            modelBuilder.Entity("Device", b =>
                {
                    b.Navigation("ButtonDatas");
                });

            modelBuilder.Entity("Profile", b =>
                {
                    b.Navigation("ButtonDatas");
                });
#pragma warning restore 612, 618
        }
    }
}