using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class Title : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the 'Info' column exists before renaming
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Mands' 
                      AND COLUMN_NAME = 'Info'
                )
                BEGIN
                    EXEC sp_rename 'Mands.Info', 'Title', 'COLUMN';
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Check if the 'Title' column exists before renaming back
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Mands' 
                      AND COLUMN_NAME = 'Title'
                )
                BEGIN
                    EXEC sp_rename 'Mands.Title', 'Info', 'COLUMN';
                END
            ");
        }
    }
}
