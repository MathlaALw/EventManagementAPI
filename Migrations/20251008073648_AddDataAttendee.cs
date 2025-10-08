using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDataAttendee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Attendees",
                columns: new[] { "AttendeeId", "Email", "EventId", "FullName", "Phone", "RegisteredAt" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "sara.alharthy@example.com", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Sara Al-Harthy", "+96890123456", new DateTime(2025, 10, 8, 7, 36, 47, 403, DateTimeKind.Utc).AddTicks(879) },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "mohammed.busaidi@example.com", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Mohammed Al-Busaidi", "+96891234567", new DateTime(2025, 10, 8, 7, 36, 47, 403, DateTimeKind.Utc).AddTicks(1158) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Attendees",
                keyColumn: "AttendeeId",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));
        }
    }
}
