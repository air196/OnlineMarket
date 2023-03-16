using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OnlineMarket.DAL.Migrations.Initial
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    current_price = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_pk", x => x.id);
                    table.UniqueConstraint("product_name_uk", x => x.name);
                    table.CheckConstraint("product_price_chk", "current_price > 0");
                });

            migrationBuilder.CreateTable(
                name: "promocode",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    promocode = table.Column<string>(type: "text", nullable: false),
                    discount_percent = table.Column<long>(type: "bigint", nullable: true),
                    discount_sum = table.Column<double>(type: "double precision", nullable: true),
                    active_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    active_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    min_order_cost = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("promocode_pk", x => x.id);
                    table.UniqueConstraint("promocode_promo_uk", x => x.promocode);
                    table.CheckConstraint("promocode_act_dates_chk", "active_to > active_from");
                    table.CheckConstraint("promocode_disc_perc_chk", "discount_percent > 0");
                    table.CheckConstraint("promocode_disc_sum_chk", "discount_sum > 0");
                    table.CheckConstraint("promocode_min_ord_cost_chk", "min_order_cost > 0");
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    second_name = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pk", x => x.id);
                    table.UniqueConstraint("user_email_uk", x => x.email);
                    table.UniqueConstraint("user_login_uk", x => x.login);
                });

            migrationBuilder.CreateTable(
                name: "audit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    transaction_id = table.Column<long>(type: "bigint", nullable: false),
                    entity_id = table.Column<long>(type: "bigint", nullable: false),
                    table_name = table.Column<string>(type: "text", nullable: false),
                    column_name = table.Column<string>(type: "text", nullable: false),
                    old_value = table.Column<string>(type: "text", nullable: false),
                    new_value = table.Column<string>(type: "text", nullable: false),
                    action = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    tmstamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("audit_pk", x => x.Id);
                    table.ForeignKey(
                        name: "audit_user_fk",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    address_to_deliver = table.Column<string>(type: "text", nullable: false),
                    deliver_by = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<long>(type: "bigint", nullable: false),
                    promocode_id = table.Column<long>(type: "bigint", nullable: true),
                    comment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_pk", x => x.id);
                    table.CheckConstraint("order_deliver_by_chk", "deliver_by > current_timestamp");
                    table.ForeignKey(
                        name: "order_promocode_fk",
                        column: x => x.promocode_id,
                        principalTable: "promocode",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "order_user_fk",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    order_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    count = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_order_pk", x => x.id);
                    table.UniqueConstraint("product_order_uk", x => new { x.product_id, x.order_id });
                    table.CheckConstraint("product_order_count_chk", "count > 0");
                    table.CheckConstraint("product_order_price_chk", "price > 0");
                    table.ForeignKey(
                        name: "po_order_fk",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "po_product_fk",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_user_id",
                table: "audit",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_promocode_id",
                table: "order",
                column: "promocode_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_user_id",
                table: "order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_order_order_id",
                table: "product_order",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit");

            migrationBuilder.DropTable(
                name: "product_order");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "promocode");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
