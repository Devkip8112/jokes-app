using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAspNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserJokesNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Jokes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "FavoriteJokes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jokes_ApplicationUserId",
                table: "Jokes",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteJokes_ApplicationUserId",
                table: "FavoriteJokes",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteJokes_AspNetUsers_ApplicationUserId",
                table: "FavoriteJokes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jokes_AspNetUsers_ApplicationUserId",
                table: "Jokes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteJokes_AspNetUsers_ApplicationUserId",
                table: "FavoriteJokes");

            migrationBuilder.DropForeignKey(
                name: "FK_Jokes_AspNetUsers_ApplicationUserId",
                table: "Jokes");

            migrationBuilder.DropIndex(
                name: "IX_Jokes_ApplicationUserId",
                table: "Jokes");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteJokes_ApplicationUserId",
                table: "FavoriteJokes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Jokes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "FavoriteJokes");
        }
    }
}
