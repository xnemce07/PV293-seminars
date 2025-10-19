using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Library.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Biography = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TotalBooksPublished = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPublishedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MostPopularGenre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ISBN = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    Pages = table.Column<int>(type: "INTEGER", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "BirthDate", "Country", "LastPublishedDate", "MostPopularGenre", "Name", "TotalBooksPublished" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Software engineer and author, known for Clean Code", new DateTime(1952, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "USA", null, "Programming", "Robert C. Martin", 1 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides", new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Various", null, "Programming", "Gang of Four", 1 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Authors of The Pragmatic Programmer", new DateTime(1965, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USA", null, "Programming", "David Thomas & Andrew Hunt", 1 }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Genre", "ISBN", "Pages", "Title", "Year" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("11111111-1111-1111-1111-111111111111"), "Programming", "978-0132350884", 464, "Clean Code", 2008 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("33333333-3333-3333-3333-333333333333"), "Programming", "978-0135957059", 352, "The Pragmatic Programmer", 2019 },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("22222222-2222-2222-2222-222222222222"), "Programming", "978-0201633610", 395, "Design Patterns", 1994 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}