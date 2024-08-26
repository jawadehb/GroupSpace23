using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace23.Migrations
{
    /// <inheritdoc />
    public partial class Quantities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alter the 'Quantities' column if it exists, or add it if it doesn't
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Mands' 
                      AND COLUMN_NAME = 'Quantities'
                )
                BEGIN
                    -- Column does not exist, so add it
                    ALTER TABLE [Mands] ADD [Quantities] INT NOT NULL DEFAULT 0;
                END
                ELSE
                BEGIN
                    -- Column exists, ensure it's of type INT and not NULL
                    -- If the column is not an INT or has a different specification, handle accordingly
                    DECLARE @sql NVARCHAR(MAX) = '';
                    IF EXISTS (
                        SELECT * 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Mands' 
                          AND COLUMN_NAME = 'Quantities' 
                          AND DATA_TYPE <> 'int'
                    )
                    BEGIN
                        SET @sql = 'ALTER TABLE [Mands] ALTER COLUMN [Quantities] INT NOT NULL;';
                    END

                    IF @sql <> ''
                    BEGIN
                        EXEC sp_executesql @sql;
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the 'Quantities' column if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Mands' 
                      AND COLUMN_NAME = 'Quantities'
                )
                BEGIN
                    ALTER TABLE [Mands] DROP COLUMN [Quantities];
                END
            ");
        }
    }
}
