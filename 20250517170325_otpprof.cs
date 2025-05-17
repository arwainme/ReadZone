using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadZone.Migrations
{
    /// <inheritdoc />
    public partial class otpprof : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBookmarkBooks");

            migrationBuilder.DropTable(
                name: "UserDownloadedBooks");

            migrationBuilder.DropTable(
                name: "UserFavoriteBooks");

            migrationBuilder.CreateTable(
                name: "UserLibraries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLibraries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLibraryBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    UserLibraryId = table.Column<int>(type: "int", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    IsBookmarked = table.Column<bool>(type: "bit", nullable: false),
                    IsDownloaded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibraryBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLibraryBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLibraryBooks_UserLibraries_UserLibraryId",
                        column: x => x.UserLibraryId,
                        principalTable: "UserLibraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraries_UserId",
                table: "UserLibraries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryBooks_BookId",
                table: "UserLibraryBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryBooks_UserLibraryId",
                table: "UserLibraryBooks",
                column: "UserLibraryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLibraryBooks");

            migrationBuilder.DropTable(
                name: "UserLibraries");

            migrationBuilder.CreateTable(
                name: "UserBookmarkBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBookmarkBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBookmarkBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBookmarkBooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDownloadedBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDownloadedBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDownloadedBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDownloadedBooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavoriteBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavoriteBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavoriteBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavoriteBooks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarkBooks_BookId",
                table: "UserBookmarkBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBookmarkBooks_UserId",
                table: "UserBookmarkBooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDownloadedBooks_BookId",
                table: "UserDownloadedBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDownloadedBooks_UserId",
                table: "UserDownloadedBooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteBooks_BookId",
                table: "UserFavoriteBooks",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavoriteBooks_UserId",
                table: "UserFavoriteBooks",
                column: "UserId");
        }
    }
}
