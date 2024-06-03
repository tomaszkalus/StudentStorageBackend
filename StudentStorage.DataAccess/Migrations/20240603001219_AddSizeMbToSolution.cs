using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentStorage.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeMbToSolution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SizeMb",
                table: "Solutions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeMb",
                table: "Solutions");
        }
    }
}
