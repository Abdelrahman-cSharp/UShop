using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UShop.Migrations
{
    /// <inheritdoc />
    public partial class remove_creditcard_from_seller_customer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Customers_CustomerId1",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Sellers_SellerId1",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_CustomerId1",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_SellerId1",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "SellerId1",
                table: "CreditCards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "CreditCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SellerId1",
                table: "CreditCards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_CustomerId1",
                table: "CreditCards",
                column: "CustomerId1",
                unique: true,
                filter: "[CustomerId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_SellerId1",
                table: "CreditCards",
                column: "SellerId1",
                unique: true,
                filter: "[SellerId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Customers_CustomerId1",
                table: "CreditCards",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Sellers_SellerId1",
                table: "CreditCards",
                column: "SellerId1",
                principalTable: "Sellers",
                principalColumn: "Id");
        }
    }
}
