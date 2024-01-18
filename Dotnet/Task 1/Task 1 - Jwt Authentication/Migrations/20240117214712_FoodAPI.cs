using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodOrderingSystemAPI.Migrations
{
    /// <inheritdoc />
    public partial class FoodAPI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    totalAmount = table.Column<float>(type: "real", nullable: false),
                    foodItemId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CheckoutDetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pincode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutDetails", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FoodCategories",
                columns: table => new
                {
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategories", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "SignupLogin",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    confirmPassword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false),
                    rememberMe = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignupLogin", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "FoodItems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imageUrl = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isVegan = table.Column<bool>(type: "bit", nullable: false),
                    calories = table.Column<float>(type: "real", nullable: false),
                    IsInGallery = table.Column<bool>(type: "bit", nullable: false),
                    FoodCategoryname = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_FoodItems_FoodCategories_FoodCategoryname",
                        column: x => x.FoodCategoryname,
                        principalTable: "FoodCategories",
                        principalColumn: "name");
                });

            migrationBuilder.CreateTable(
                name: "invoiceModel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateOfInvoice = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Total_Bill = table.Column<float>(type: "real", nullable: false),
                    transactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    orderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoiceModel", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoiceModel_SignupLogin_userId",
                        column: x => x.userId,
                        principalTable: "SignupLogin",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "otpModels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    otpValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isVerified = table.Column<bool>(type: "bit", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_otpModels", x => x.id);
                    table.ForeignKey(
                        name: "FK_otpModels_SignupLogin_userId",
                        column: x => x.userId,
                        principalTable: "SignupLogin",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    profileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    profilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pincode = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.profileId);
                    table.ForeignKey(
                        name: "FK_UserProfile_SignupLogin_userId",
                        column: x => x.userId,
                        principalTable: "SignupLogin",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<int>(type: "int", nullable: false),
                    orderBill = table.Column<float>(type: "real", nullable: false),
                    orderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    orderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    foodItemId = table.Column<int>(type: "int", nullable: true),
                    invoiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_FoodItems_foodItemId",
                        column: x => x.foodItemId,
                        principalTable: "FoodItems",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_invoiceModel_invoiceId",
                        column: x => x.invoiceId,
                        principalTable: "invoiceModel",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodItems_FoodCategoryname",
                table: "FoodItems",
                column: "FoodCategoryname");

            migrationBuilder.CreateIndex(
                name: "IX_invoiceModel_userId",
                table: "invoiceModel",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_foodItemId",
                table: "orders",
                column: "foodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_invoiceId",
                table: "orders",
                column: "invoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_otpModels_userId",
                table: "otpModels",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_userId",
                table: "UserProfile",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "CheckoutDetails");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "otpModels");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "FoodItems");

            migrationBuilder.DropTable(
                name: "invoiceModel");

            migrationBuilder.DropTable(
                name: "FoodCategories");

            migrationBuilder.DropTable(
                name: "SignupLogin");
        }
    }
}
