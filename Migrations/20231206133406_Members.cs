using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class Members : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Evenements_EvenementId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Mand_AspNetUsers_SenderId",
                table: "Mand");

            migrationBuilder.DropForeignKey(
                name: "FK_Mand_Evenements_RecipientId",
                table: "Mand");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EvenementId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parameter",
                table: "Parameter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mand",
                table: "Mand");

            migrationBuilder.DropColumn(
                name: "EvenementId",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Parameter",
                newName: "Parameters");

            migrationBuilder.RenameTable(
                name: "Mand",
                newName: "Mands");

            migrationBuilder.RenameIndex(
                name: "IX_Mand_SenderId",
                table: "Mands",
                newName: "IX_Mands_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Mand_RecipientId",
                table: "Mands",
                newName: "IX_Mands_RecipientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parameters",
                table: "Parameters",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mands",
                table: "Mands",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EvenementMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvenementId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Added = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHost = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvenementMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvenementMembers_AspNetUsers_AddedById",
                        column: x => x.AddedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_EvenementMembers_AspNetUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_EvenementMembers_Evenements_EvenementId",
                        column: x => x.EvenementId,
                        principalTable: "Evenements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvenementMembers_AddedById",
                table: "EvenementMembers",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_EvenementMembers_EvenementId",
                table: "EvenementMembers",
                column: "EvenementId");

            migrationBuilder.CreateIndex(
                name: "IX_EvenementMembers_MemberId",
                table: "EvenementMembers",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mands_AspNetUsers_SenderId",
                table: "Mands",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Mands_Evenements_RecipientId",
                table: "Mands",
                column: "RecipientId",
                principalTable: "Evenements",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mands_AspNetUsers_SenderId",
                table: "Mands");

            migrationBuilder.DropForeignKey(
                name: "FK_Mands_Evenements_RecipientId",
                table: "Mands");

            migrationBuilder.DropTable(
                name: "EvenementMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parameters",
                table: "Parameters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mands",
                table: "Mands");

            migrationBuilder.RenameTable(
                name: "Parameters",
                newName: "Parameter");

            migrationBuilder.RenameTable(
                name: "Mands",
                newName: "Mand");

            migrationBuilder.RenameIndex(
                name: "IX_Mands_SenderId",
                table: "Mand",
                newName: "IX_Mand_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Mands_RecipientId",
                table: "Mand",
                newName: "IX_Mand_RecipientId");

            migrationBuilder.AddColumn<int>(
                name: "EvenementId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parameter",
                table: "Parameter",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mand",
                table: "Mand",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Mand_AspNetUsers_SenderId",
                table: "Mand",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Mand_Evenements_RecipientId",
                table: "Mand",
                column: "RecipientId",
                principalTable: "Evenements",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction) ;
        }
    }
}
