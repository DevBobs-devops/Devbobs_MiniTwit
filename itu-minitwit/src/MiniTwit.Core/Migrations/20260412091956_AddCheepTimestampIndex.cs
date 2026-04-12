using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCheepTimestampIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cheeps_Timestamp",
                table: "Cheeps",
                column: "Timestamp"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Cheeps_Timestamp", table: "Cheeps");
        }
    }
}
