using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataManager.Migrations
{
    /// <inheritdoc />
    public partial class SortingModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sortings",
                columns: table => new
                {
                    sorting_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    file = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sortings", x => x.sorting_id);
                    table.ForeignKey(
                        name: "FK_Sortings_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "chat_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SortingParams",
                columns: table => new
                {
                    sorting_params_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortingId = table.Column<int>(type: "integer", nullable: false),
                    field = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortingParams", x => x.sorting_params_id);
                    table.ForeignKey(
                        name: "FK_SortingParams_Sortings_SortingId",
                        column: x => x.SortingId,
                        principalTable: "Sortings",
                        principalColumn: "sorting_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SortingParams_SortingId",
                table: "SortingParams",
                column: "SortingId");

            migrationBuilder.CreateIndex(
                name: "IX_Sortings_ChatId",
                table: "Sortings",
                column: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SortingParams");

            migrationBuilder.DropTable(
                name: "Sortings");
        }
    }
}
