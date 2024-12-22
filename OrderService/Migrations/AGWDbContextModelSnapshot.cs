﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderService.Data;

#nullable disable

namespace OrderService.Migrations
{
    [DbContext(typeof(OrderDbContext))]
    partial class AGWDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OrderService.Model.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<bool>("VisableFlag")
                        .HasColumnType("bit");

                    b.HasKey("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("ordersTable");

                    b.HasData(
                        new
                        {
                            OrderId = 1,
                            ProductId = 1,
                            VisableFlag = true
                        },
                        new
                        {
                            OrderId = 2,
                            ProductId = 2,
                            VisableFlag = true
                        },
                        new
                        {
                            OrderId = 3,
                            ProductId = 3,
                            VisableFlag = true
                        });
                });

            modelBuilder.Entity("OrderService.Model.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("ProductDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.ToTable("ProductsTables");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            ProductDescription = "Product 1 Description",
                            ProductName = "Product 1"
                        },
                        new
                        {
                            ProductId = 2,
                            ProductDescription = "Product 2 Description",
                            ProductName = "Product 2"
                        },
                        new
                        {
                            ProductId = 3,
                            ProductDescription = "Product 3 Description",
                            ProductName = "Product 3"
                        });
                });

            modelBuilder.Entity("OrderService.Model.Order", b =>
                {
                    b.HasOne("OrderService.Model.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });
#pragma warning restore 612, 618
        }
    }
}
