﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShipmondoBackendAssignment.DB;

#nullable disable

namespace ShipmondoBackendAssignment.Migrations
{
    [DbContext(typeof(ShipmondoDbContext))]
    partial class ShipmondoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("ShipmondoBackendAssignment.DB.Models.AccountBalance", b =>
                {
                    b.Property<decimal>("amount")
                        .HasColumnType("TEXT");

                    b.Property<string>("currencyCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("updateInstant")
                        .HasColumnType("TEXT");

                    b.HasIndex("updateInstant");

                    b.ToTable("AccountBalances");
                });

            modelBuilder.Entity("ShipmondoBackendAssignment.DB.Models.Shipment", b =>
                {
                    b.Property<int>("id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("packageNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("Shipments");
                });
#pragma warning restore 612, 618
        }
    }
}
