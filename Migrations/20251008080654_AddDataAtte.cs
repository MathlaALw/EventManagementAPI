using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDataAtte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisteredAt",
                table: "Attendees",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "RegisteredAt",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "RegisteredAt",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RegisteredAt",
                table: "Attendees",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "RegisteredAt",
                value: new DateTime(2025, 10, 8, 7, 44, 54, 134, DateTimeKind.Utc).AddTicks(6901));

            migrationBuilder.UpdateData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                column: "RegisteredAt",
                value: new DateTime(2025, 10, 8, 7, 44, 54, 134, DateTimeKind.Utc).AddTicks(7201));
        }
    }
}
