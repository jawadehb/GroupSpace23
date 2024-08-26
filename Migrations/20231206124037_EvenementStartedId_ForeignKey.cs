using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class EvenementStartedId_ForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StartedById",
                table: "Evenements",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Evenements_StartedById",
                table: "Evenements",
                column: "StartedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Evenements_AspNetUsers_StartedById",
                table: "Evenements",
                column: "StartedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evenements_AspNetUsers_StartedById",
                table: "Evenements");

            migrationBuilder.DropIndex(
                name: "IX_Evenements_StartedById",
                table: "Evenements");

            migrationBuilder.AlterColumn<string>(
                name: "StartedById",
                table: "Evenements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
