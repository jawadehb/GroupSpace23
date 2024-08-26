using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class MandSender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Mand",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Mand_SenderId",
                table: "Mand",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mand_AspNetUsers_SenderId",
                table: "Mand",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mand_AspNetUsers_SenderId",
                table: "Mand");

            migrationBuilder.DropIndex(
                name: "IX_Mand_SenderId",
                table: "Mand");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Mand");
        }
    }
}
