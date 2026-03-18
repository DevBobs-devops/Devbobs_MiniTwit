using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangesLikesRemovedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Likes",
                table: "Cheeps",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Likes", table: "Cheeps");
        }
    }
}
