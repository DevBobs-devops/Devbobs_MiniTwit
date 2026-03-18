using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNamingInFollow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FollowsAuthorName",
                table: "Follows",
                newName: "Followed"
            );

            migrationBuilder.RenameColumn(
                name: "AuthorName",
                table: "Follows",
                newName: "Follower"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Followed",
                table: "Follows",
                newName: "FollowsAuthorName"
            );

            migrationBuilder.RenameColumn(
                name: "Follower",
                table: "Follows",
                newName: "AuthorName"
            );
        }
    }
}
