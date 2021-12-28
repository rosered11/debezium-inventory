using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryManagement.Infrastructure.Migrations
{
    public partial class RenameTableFollowNamingConvention : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxEvent",
                table: "OutboxEvent");

            migrationBuilder.RenameTable(
                name: "OutboxEvent",
                newName: "outboxevent");

            migrationBuilder.RenameTable(
                name: "Idempotent",
                newName: "idempotent");

            migrationBuilder.RenameTable(
                name: "Inventories",
                newName: "inventory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outboxevent",
                table: "outboxevent",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_outboxevent",
                table: "outboxevent");

            migrationBuilder.RenameTable(
                name: "outboxevent",
                newName: "OutboxEvent");

            migrationBuilder.RenameTable(
                name: "idempotent",
                newName: "Idempotent");

            migrationBuilder.RenameTable(
                name: "inventory",
                newName: "Inventories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxEvent",
                table: "OutboxEvent",
                column: "id");
        }
    }
}
