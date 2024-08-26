using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class AddStartDateToEvenement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Evenements",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Evenements");
        }
    }
}
