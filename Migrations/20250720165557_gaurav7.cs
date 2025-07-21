using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserRoles.Migrations
{
    /// <inheritdoc />
    public partial class gaurav7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerAddCategoryViewModel_SellerAddProductViewModel_SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel");

            migrationBuilder.DropIndex(
                name: "IX_SellerAddCategoryViewModel_SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel");

            migrationBuilder.DropColumn(
                name: "SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerAddProductViewModel_SellerAddCategoryViewModel_SellerCategoryId",
                table: "SellerAddProductViewModel");

            migrationBuilder.DropIndex(
                name: "IX_SellerAddProductViewModel_SellerCategoryId",
                table: "SellerAddProductViewModel");

            migrationBuilder.AddColumn<int>(
                name: "SellerAddProductViewModelProductId",
                table: "SellerAddCategoryViewModel",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
