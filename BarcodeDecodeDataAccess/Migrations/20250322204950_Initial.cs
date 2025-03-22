using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarcodeDecodeDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transport_orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    barcode = table.Column<string>(type: "text", nullable: false),
                    destinations = table.Column<List<int>>(type: "integer[]", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    closed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transport_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transport_storage_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    barcode = table.Column<string>(type: "text", nullable: false),
                    transport_order_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transport_storage_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_transport_storage_units_transport_orders_transport_order_id",
                        column: x => x.transport_order_id,
                        principalTable: "transport_orders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "location_tickets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    departure_location = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    planned_locations = table.Column<List<int>>(type: "integer[]", nullable: false),
                    arrived_at_location = table.Column<int>(type: "integer", nullable: true),
                    arrived_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sorting_error_code = table.Column<int>(type: "integer", nullable: false),
                    error_message = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    transport_storage_unit_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location_tickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_location_tickets_transport_storage_units_transport_storage_",
                        column: x => x.transport_storage_unit_id,
                        principalTable: "transport_storage_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "transport_orders",
                columns: new[] { "id", "barcode", "closed_on", "created_on", "destinations", "external_id", "status" },
                values: new object[] { 1, "SomeBarcode1", null, new DateTimeOffset(new DateTime(2025, 3, 22, 23, 48, 50, 185, DateTimeKind.Unspecified).AddTicks(3174), new TimeSpan(0, 3, 0, 0, 0)), new List<int> { 13 }, "1aae532d-ccc5-4ea7-aae2-c4db55b6ecbb", 2 });

            migrationBuilder.InsertData(
                table: "transport_storage_units",
                columns: new[] { "id", "barcode", "created_on", "status", "transport_order_id", "updated_on" },
                values: new object[] { 1, "SomeBarcode1", new DateTimeOffset(new DateTime(2025, 3, 22, 23, 48, 50, 185, DateTimeKind.Unspecified).AddTicks(3236), new TimeSpan(0, 3, 0, 0, 0)), 1, 1, new DateTimeOffset(new DateTime(2025, 3, 22, 23, 48, 50, 185, DateTimeKind.Unspecified).AddTicks(3238), new TimeSpan(0, 3, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "location_tickets",
                columns: new[] { "id", "arrived_at_location", "arrived_on", "created_on", "departure_location", "error_message", "planned_locations", "sorting_error_code", "status", "transport_storage_unit_id" },
                values: new object[,]
                {
                    { 1, 11, new DateTimeOffset(new DateTime(2025, 3, 22, 23, 49, 20, 185, DateTimeKind.Unspecified).AddTicks(3241), new TimeSpan(0, 3, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 3, 22, 23, 49, 10, 185, DateTimeKind.Unspecified).AddTicks(3242), new TimeSpan(0, 3, 0, 0, 0)), 10, null, new List<int> { 11, 12 }, 0, 3, 1 },
                    { 2, null, new DateTimeOffset(new DateTime(2025, 3, 22, 23, 49, 40, 185, DateTimeKind.Unspecified).AddTicks(3246), new TimeSpan(0, 3, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 3, 22, 23, 49, 30, 185, DateTimeKind.Unspecified).AddTicks(3248), new TimeSpan(0, 3, 0, 0, 0)), 11, null, new List<int> { 13 }, 0, 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_location_tickets_arrived_at_location",
                table: "location_tickets",
                column: "arrived_at_location");

            migrationBuilder.CreateIndex(
                name: "ix_location_tickets_created_on",
                table: "location_tickets",
                column: "created_on");

            migrationBuilder.CreateIndex(
                name: "ix_location_tickets_departure_location",
                table: "location_tickets",
                column: "departure_location");

            migrationBuilder.CreateIndex(
                name: "ix_location_tickets_transport_storage_unit_id",
                table: "location_tickets",
                column: "transport_storage_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_transport_orders_barcode",
                table: "transport_orders",
                column: "barcode");

            migrationBuilder.CreateIndex(
                name: "ix_transport_orders_closed_on",
                table: "transport_orders",
                column: "closed_on");

            migrationBuilder.CreateIndex(
                name: "ix_transport_orders_created_on",
                table: "transport_orders",
                column: "created_on");

            migrationBuilder.CreateIndex(
                name: "ix_transport_orders_status",
                table: "transport_orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_transport_storage_units_barcode_status",
                table: "transport_storage_units",
                columns: new[] { "barcode", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_transport_storage_units_status_updated_on",
                table: "transport_storage_units",
                columns: new[] { "status", "updated_on" });

            migrationBuilder.CreateIndex(
                name: "ix_transport_storage_units_transport_order_id",
                table: "transport_storage_units",
                column: "transport_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_transport_storage_units_updated_on",
                table: "transport_storage_units",
                column: "updated_on");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "location_tickets");

            migrationBuilder.DropTable(
                name: "transport_storage_units");

            migrationBuilder.DropTable(
                name: "transport_orders");
        }
    }
}
