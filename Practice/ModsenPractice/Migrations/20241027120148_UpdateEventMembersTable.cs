using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModsenPractice.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventMembersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsAndMembers_Events_MemberEventsId",
                table: "EventsAndMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsAndMembers_Members_EventMembersId",
                table: "EventsAndMembers");

            migrationBuilder.RenameColumn(
                name: "MemberEventsId",
                table: "EventsAndMembers",
                newName: "MemberID");

            migrationBuilder.RenameColumn(
                name: "EventMembersId",
                table: "EventsAndMembers",
                newName: "EventID");

            migrationBuilder.RenameIndex(
                name: "IX_EventsAndMembers_MemberEventsId",
                table: "EventsAndMembers",
                newName: "IX_EventsAndMembers_MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_EventsAndMembers_Events_EventID",
                table: "EventsAndMembers",
                column: "EventID",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventsAndMembers_Members_MemberID",
                table: "EventsAndMembers",
                column: "MemberID",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsAndMembers_Events_EventID",
                table: "EventsAndMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsAndMembers_Members_MemberID",
                table: "EventsAndMembers");

            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "EventsAndMembers",
                newName: "MemberEventsId");

            migrationBuilder.RenameColumn(
                name: "EventID",
                table: "EventsAndMembers",
                newName: "EventMembersId");

            migrationBuilder.RenameIndex(
                name: "IX_EventsAndMembers_MemberID",
                table: "EventsAndMembers",
                newName: "IX_EventsAndMembers_MemberEventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventsAndMembers_Events_MemberEventsId",
                table: "EventsAndMembers",
                column: "MemberEventsId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventsAndMembers_Members_EventMembersId",
                table: "EventsAndMembers",
                column: "EventMembersId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
