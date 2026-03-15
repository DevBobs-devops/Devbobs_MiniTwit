using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFollowSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_Follows", table: "Follows");

            migrationBuilder.DropColumn(name: "AuthorId", table: "Follows");

            migrationBuilder.DropColumn(name: "FollowsId", table: "Follows");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                columns: new[] { "AuthorName", "FollowsAuthorName" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_Follows", table: "Follows");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Follows",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "FollowsId",
                table: "Follows",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                columns: new[] { "AuthorId", "FollowsId", "AuthorName" }
            );
        }
    }
}
