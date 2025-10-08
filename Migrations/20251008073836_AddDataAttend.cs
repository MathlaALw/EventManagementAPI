using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDataAttend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "RegisteredAt",
                value: new DateTime(2025, 10, 8, 7, 38, 36, 241, DateTimeKind.Utc).AddTicks(7334));

            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "RegisteredAt",
                value: new DateTime(2025, 10, 8, 7, 38, 36, 241, DateTimeKind.Utc).AddTicks(8694));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "RegisteredAt",
                value: new DateTime(2025, 10, 8, 7, 36, 47, 403, DateTimeKind.Utc).AddTicks(879));

            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "RegisteredAt",
                value: new DateTime(2025, 10, 8, 7, 36, 47, 403, DateTimeKind.Utc).AddTicks(1158));
        }
    }
}
