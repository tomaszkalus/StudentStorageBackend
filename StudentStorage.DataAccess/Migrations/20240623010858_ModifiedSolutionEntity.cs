using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentStorage.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedSolutionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SizeMb",
                table: "Solutions",
                newName: "SizeBytes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                table: "Solutions",
                newName: "SizeMb");
        }
    }
}
