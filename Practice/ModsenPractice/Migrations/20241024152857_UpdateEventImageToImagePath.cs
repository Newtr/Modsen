using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModsenPractice.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventImageToImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventImage",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Events",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Events");

            migrationBuilder.AddColumn<byte[]>(
                name: "EventImage",
                table: "Events",
                type: "BLOB",
                nullable: true);
        }
    }
}
