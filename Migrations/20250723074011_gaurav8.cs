using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRoles.Migrations
{
    /// <inheritdoc />
    public partial class gaurav8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerAddProductViewModel_SellerAddCategoryViewModel_SellerCategoryId",
                table: "SellerAddProductViewModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SellerAddProductViewModel",
                table: "SellerAddProductViewModel");

            migrationBuilder.DropIndex(
                name: "IX_SellerAddProductViewModel_SellerCategoryId",
                table: "SellerAddProductViewModel");

            migrationBuilder.RenameTable(
                name: "SellerAddProductViewModel",
                newName: "Product");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "SellerAddProductViewModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SellerAddProductViewModel",
                table: "SellerAddProductViewModel",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerAddProductViewModel_SellerCategoryId",
                table: "SellerAddProductViewModel",
                column: "SellerCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerAddProductViewModel_SellerAddCategoryViewModel_SellerCategoryId",
                table: "SellerAddProductViewModel",
                column: "SellerCategoryId",
                principalTable: "SellerAddCategoryViewModel",
                principalColumn: "SellerCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
