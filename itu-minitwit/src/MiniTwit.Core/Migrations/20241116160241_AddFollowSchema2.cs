using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowSchema2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_AuthorId",
                table: "Follows"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowsId",
                table: "Follows"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_Follows", table: "Follows");

            migrationBuilder.DropIndex(name: "IX_Follows_FollowsId", table: "Follows");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Follows",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "FollowsAuthorName",
                table: "Follows",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                columns: new[] { "AuthorId", "FollowsId", "AuthorName" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_Follows", table: "Follows");

            migrationBuilder.DropColumn(name: "AuthorName", table: "Follows");

            migrationBuilder.DropColumn(name: "FollowsAuthorName", table: "Follows");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                columns: new[] { "AuthorId", "FollowsId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowsId",
                table: "Follows",
                column: "FollowsId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_AuthorId",
                table: "Follows",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowsId",
                table: "Follows",
                column: "FollowsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
