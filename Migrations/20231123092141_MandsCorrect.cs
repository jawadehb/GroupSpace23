using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    public partial class MandsCorrect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the Mand table
            migrationBuilder.CreateTable(
                name: "Mand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecipientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mand_Evenements_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Evenements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create an index on the RecipientId column
            migrationBuilder.CreateIndex(
                name: "IX_Mand_RecipientId",
                table: "Mand",
                column: "RecipientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the Mand table if rolling back
            migrationBuilder.DropTable(
                name: "Mand");
        }
    }
}
