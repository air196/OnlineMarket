﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OnlineMarket.DAL;

#nullable disable

namespace OnlineMarket.DAL.Migrations.Initial
{
    [DbContext(typeof(MarketDbContext))]
    [Migration("20230314153326_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("OnlineMarket.Domain.Models.Audit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<long>("Id"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("action");

                    b.Property<string>("ColumnName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("column_name");

                    b.Property<long>("EntityId")
                        .HasColumnType("bigint")
                        .HasColumnName("entity_id");

                    b.Property<string>("NewValue")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("new_value");

                    b.Property<string>("OldValue")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("old_value");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("table_name");

                    b.Property<DateTime>("Tmstamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("tmstamp");

                    b.Property<long>("TransactionId")
                        .HasColumnType("bigint")
                        .HasColumnName("transaction_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("audit_pk");

                    b.HasIndex("UserId");

                    b.ToTable("audit", (string)null);
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<long>("Id"));

                    b.Property<string>("AddressToDeliver")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("address_to_deliver");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("comment");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<DateTime>("DeliverBy")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deliver_by");

                    b.Property<long?>("PromocodeId")
                        .HasColumnType("bigint")
                        .HasColumnName("promocode_id");

                    b.Property<long>("Status")
                        .HasColumnType("bigint")
                        .HasColumnName("status");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("order_pk");

                    b.HasIndex("PromocodeId");

                    b.HasIndex("UserId");

                    b.ToTable("order", null, t =>
                        {
                            t.HasCheckConstraint("order_deliver_by_chk", "deliver_by > current_timestamp");
                        });
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Product", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<long>("Id"));

                    b.Property<double>("CurrentPrice")
                        .HasColumnType("double precision")
                        .HasColumnName("current_price");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("product_pk");

                    b.HasAlternateKey("Name")
                        .HasName("product_name_uk");

                    b.ToTable("product", null, t =>
                        {
                            t.HasCheckConstraint("product_price_chk", "current_price > 0");
                        });
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.ProductOrder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<long>("Id"));

                    b.Property<double>("Count")
                        .HasColumnType("double precision")
                        .HasColumnName("count");

                    b.Property<long>("OrderId")
                        .HasColumnType("bigint")
                        .HasColumnName("order_id");

                    b.Property<double>("Price")
                        .HasColumnType("double precision")
                        .HasColumnName("price");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    b.HasKey("Id")
                        .HasName("product_order_pk");

                    b.HasAlternateKey("ProductId", "OrderId")
                        .HasName("product_order_uk");

                    b.HasIndex("OrderId");

                    b.ToTable("product_order", null, t =>
                        {
                            t.HasCheckConstraint("product_order_count_chk", "count > 0");

                            t.HasCheckConstraint("product_order_price_chk", "price > 0");
                        });
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Promocode", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("ActiveFrom")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("active_from");

                    b.Property<DateTime>("ActiveTo")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("active_to");

                    b.Property<long?>("DiscountPercent")
                        .HasColumnType("bigint")
                        .HasColumnName("discount_percent");

                    b.Property<double?>("DiscountSum")
                        .HasColumnType("double precision")
                        .HasColumnName("discount_sum");

                    b.Property<double?>("MinimumOrderCost")
                        .HasColumnType("double precision")
                        .HasColumnName("min_order_cost");

                    b.Property<string>("PromoCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("promocode");

                    b.HasKey("Id")
                        .HasName("promocode_pk");

                    b.HasAlternateKey("PromoCode")
                        .HasName("promocode_promo_uk");

                    b.ToTable("promocode", null, t =>
                        {
                            t.HasCheckConstraint("promocode_act_dates_chk", "active_to > active_from");

                            t.HasCheckConstraint("promocode_disc_perc_chk", "discount_percent > 0");

                            t.HasCheckConstraint("promocode_disc_sum_chk", "discount_sum > 0");

                            t.HasCheckConstraint("promocode_min_ord_cost_chk", "min_order_cost > 0");
                        });
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<long>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("login");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("SecondName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("second_name");

                    b.HasKey("Id")
                        .HasName("user_pk");

                    b.HasAlternateKey("Email")
                        .HasName("user_email_uk");

                    b.HasAlternateKey("Login")
                        .HasName("user_login_uk");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Audit", b =>
                {
                    b.HasOne("OnlineMarket.Domain.Models.User", "User")
                        .WithMany("Logs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("audit_user_fk");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Order", b =>
                {
                    b.HasOne("OnlineMarket.Domain.Models.Promocode", "Promocode")
                        .WithMany("Orders")
                        .HasForeignKey("PromocodeId")
                        .HasConstraintName("order_promocode_fk");

                    b.HasOne("OnlineMarket.Domain.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("order_user_fk");

                    b.Navigation("Promocode");

                    b.Navigation("User");
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.ProductOrder", b =>
                {
                    b.HasOne("OnlineMarket.Domain.Models.Order", "Order")
                        .WithMany("ProductsOrders")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("po_order_fk");

                    b.HasOne("OnlineMarket.Domain.Models.Product", "Product")
                        .WithMany("ProductsOrders")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("po_product_fk");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Order", b =>
                {
                    b.Navigation("ProductsOrders");
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Product", b =>
                {
                    b.Navigation("ProductsOrders");
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.Promocode", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("OnlineMarket.Domain.Models.User", b =>
                {
                    b.Navigation("Logs");

                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
