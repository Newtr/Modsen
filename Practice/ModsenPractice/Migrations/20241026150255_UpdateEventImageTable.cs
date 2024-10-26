using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModsenPractice.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventImages_Events_MyEventId",
                table: "EventImages");

            migrationBuilder.DropIndex(
                name: "IX_EventImages_MyEventId",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "MyEventId",
                table: "EventImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyEventId",
                table: "EventImages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventImages_MyEventId",
                table: "EventImages",
                column: "MyEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventImages_Events_MyEventId",
                table: "EventImages",
                column: "MyEventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
