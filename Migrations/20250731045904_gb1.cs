using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRoles.Migrations
{
    /// <inheritdoc />
    public partial class gb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SellerCategories");

            migrationBuilder.AlterColumn<string>(
                name: "SellerId",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SellerId",
                table: "Product",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AspNetUsers_SellerId",
                table: "Product",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_SellerId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_SellerId",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "SellerId",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
    }
}
