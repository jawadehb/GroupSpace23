﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class Langues_correction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Language",
                table: "Language");

            migrationBuilder.RenameTable(
                name: "Language",
                newName: "Languages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Languages",
                table: "Languages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Languages",
                table: "Languages");

            migrationBuilder.RenameTable(
                name: "Languages",
                newName: "Language");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Language",
                table: "Language",
                column: "Id");
        }
    }
}
