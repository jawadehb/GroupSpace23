using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class memberlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EvenementId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EvenementId",
                table: "AspNetUsers",
                column: "EvenementId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Evenements_EvenementId",
                table: "AspNetUsers",
                column: "EvenementId",
                principalTable: "Evenements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Evenements_EvenementId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EvenementId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EvenementId",
                table: "AspNetUsers");
        }
    }
}
