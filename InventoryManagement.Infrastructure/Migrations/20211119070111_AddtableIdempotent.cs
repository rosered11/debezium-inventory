using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryManagement.Infrastructure.Migrations
{
    public partial class AddtableIdempotent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Idempotent",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "text", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_EventIdAndEventType", x => new { x.EventId, x.EventType });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Idempotent");
        }
    }
}
