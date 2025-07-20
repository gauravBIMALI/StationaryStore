using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRoles.Migrations
{
    /// <inheritdoc />
    public partial class gaurav6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SellerAddProductViewModel",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SellerCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerAddProductViewModel", x => x.ProductId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellerAddCategoryViewModel_SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel",
                column: "SellerAddProductViewModelProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerAddCategoryViewModel_SellerAddProductViewModel_SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel",
                column: "SellerAddProductViewModelProductId",
                principalTable: "SellerAddProductViewModel",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerAddCategoryViewModel_SellerAddProductViewModel_SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel");

            migrationBuilder.DropTable(
                name: "SellerAddProductViewModel");

            migrationBuilder.DropIndex(
                name: "IX_SellerAddCategoryViewModel_SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel");

            migrationBuilder.DropColumn(
                name: "SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel");
        }
    }
}
