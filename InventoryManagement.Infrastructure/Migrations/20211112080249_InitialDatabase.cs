using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryManagement.Infrastructure.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    PartNo = table.Column<string>(type: "text", nullable: false),
                    WarehouseLocationNo = table.Column<string>(type: "text", nullable: false),
                    POQty = table.Column<int>(type: "integer", nullable: false),
                    ReceivingQty = table.Column<int>(type: "integer", nullable: false),
                    BalanceQty = table.Column<int>(type: "integer", nullable: false),
                    RequestQty = table.Column<int>(type: "integer", nullable: false),
                    AvailableQty = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserCreated = table.Column<string>(type: "text", nullable: true),
                    UserUpdated = table.Column<string>(type: "text", nullable: true),
                    UserIdCreated = table.Column<string>(type: "text", nullable: true),
                    UserIdUpdated = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_PartNoAndWarehouseLocationNo", x => new { x.PartNo, x.WarehouseLocationNo });
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvent",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    aggregatetype = table.Column<string>(type: "character varying(255)", nullable: false),
                    aggregateid = table.Column<string>(type: "character varying(255)", nullable: false),
                    type = table.Column<string>(type: "character varying(255)", nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvent", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "OutboxEvent");
        }
    }
}
