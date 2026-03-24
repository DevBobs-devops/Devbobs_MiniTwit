using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Core.Migrations
{
    /// <inheritdoc />
    public partial class addNrLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NrLikes",
                table: "Cheeps",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NrLikes",
                table: "Cheeps");
        }
    }
}
