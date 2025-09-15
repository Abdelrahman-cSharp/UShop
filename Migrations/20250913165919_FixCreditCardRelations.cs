using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UShop.Migrations
{
    /// <inheritdoc />
    public partial class FixCreditCardRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Customers_CustomerId",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Sellers_SellerId",
                table: "CreditCards");

            migrationBuilder.DropTable(
                name: "AddressViewModel");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_CustomerId",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_SellerId",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "CreditCards");

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
                name: "IX_CreditCards_CustomerId",
                table: "CreditCards",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_CustomerId1",
                table: "CreditCards",
                column: "CustomerId1",
                unique: true,
                filter: "[CustomerId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_SellerId",
                table: "CreditCards",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_SellerId1",
                table: "CreditCards",
                column: "SellerId1",
                unique: true,
                filter: "[SellerId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Customers_CustomerId",
                table: "CreditCards",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Customers_CustomerId1",
                table: "CreditCards",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Sellers_SellerId",
                table: "CreditCards",
                column: "SellerId",
                principalTable: "Sellers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Sellers_SellerId1",
                table: "CreditCards",
                column: "SellerId1",
                principalTable: "Sellers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Customers_CustomerId",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Customers_CustomerId1",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Sellers_SellerId",
                table: "CreditCards");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Sellers_SellerId1",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_CustomerId",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_CustomerId1",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_SellerId",
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

            migrationBuilder.AddColumn<string>(
                name: "ExpiryDate",
                table: "CreditCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AddressViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressViewModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_CustomerId",
                table: "CreditCards",
                column: "CustomerId",
                unique: true,
                filter: "[CustomerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_SellerId",
                table: "CreditCards",
                column: "SellerId",
                unique: true,
                filter: "[SellerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Customers_CustomerId",
                table: "CreditCards",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Sellers_SellerId",
                table: "CreditCards",
                column: "SellerId",
                principalTable: "Sellers",
                principalColumn: "Id");
        }
    }
}
