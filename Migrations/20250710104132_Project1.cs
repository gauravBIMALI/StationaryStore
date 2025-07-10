using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRoles.Migrations
{
    /// <inheritdoc />
    public partial class Project1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SellerAddCategoryViewModel",
                columns: table => new
                {
                    SellerCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SellerCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellerCategoryType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerAddCategoryViewModel", x => x.SellerCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "SellerCategories",
                columns: table => new
                {
                    SellerCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SellerCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellerCategoryType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerCategories", x => x.SellerCategoryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellerAddCategoryViewModel");

            migrationBuilder.DropTable(
                name: "SellerCategories");
        }
    }
}
