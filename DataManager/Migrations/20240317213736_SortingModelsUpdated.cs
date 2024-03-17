using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataManager.Migrations
{
    /// <inheritdoc />
    public partial class SortingModelsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "value",
                table: "SortingParams");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "SortingParams",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is_descending",
                table: "SortingParams",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "SortingParams");

            migrationBuilder.DropColumn(
                name: "is_descending",
                table: "SortingParams");

            migrationBuilder.AddColumn<string>(
                name: "value",
                table: "SortingParams",
                type: "text",
                nullable: true);
        }
    }
}
