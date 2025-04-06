﻿// <auto-generated />
using EshopApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EshopApi.Data.Migrations
{
    [DbContext(typeof(EshopContext))]
    partial class EshopContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("EshopApi.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<int>("Stock")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Products", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Apple smartphone",
                            ImageUrl = "someUrl1",
                            Name = "iPhone 15",
                            Price = 999.99m,
                            Stock = 0
                        },
                        new
                        {
                            Id = 2,
                            Description = "Samsung smartphone",
                            ImageUrl = "someUrl3",
                            Name = "Samsung Galaxy S24",
                            Price = 899.99m,
                            Stock = 0
                        },
                        new
                        {
                            Id = 3,
                            Description = "Google smartphone",
                            ImageUrl = "someUrl3",
                            Name = "Google Pixel 8",
                            Price = 799.99m,
                            Stock = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
