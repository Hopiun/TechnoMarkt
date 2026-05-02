using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechnoMarkt.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parent_category_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__D54EE9B47B2D90A1", x => x.category_id);
                    table.ForeignKey(
                        name: "FK_Category_Parent",
                        column: x => x.parent_category_id,
                        principalTable: "Category",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    сity_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__City__61D698BE5CCEFA82", x => x.сity_id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    country_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Country__7E8CD055985BE3F3", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "EventLog",
                columns: table => new
                {
                    log_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    entity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    entity_id = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLog", x => x.log_id);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Review__60883D9045B88A06", x => x.review_id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    wallet_balance = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValueSql: "((0.00))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Client__BF21A42480BE8261", x => x.client_id);
                    table.ForeignKey(
                        name: "FK_Client_AspNetUsers",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    brand_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    country_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Brand__5E5A8E271BA017D4", x => x.brand_id);
                    table.ForeignKey(
                        name: "FK_Brand_Country",
                        column: x => x.country_id,
                        principalTable: "Country",
                        principalColumn: "country_id");
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    supplier_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    country_id = table.Column<int>(type: "int", nullable: true),
                    contact = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supplier__6EE594E8D6393AB1", x => x.supplier_id);
                    table.ForeignKey(
                        name: "FK_Supplier_Country",
                        column: x => x.country_id,
                        principalTable: "Country",
                        principalColumn: "country_id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    payment_method_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    bank_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    card_number = table.Column<string>(type: "char(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: false),
                    card_type = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaymentM__8A3EA9EB81A265C1", x => x.payment_method_id);
                    table.ForeignKey(
                        name: "FK_PaymentMethod_Client",
                        column: x => x.client_id,
                        principalTable: "Client",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    brand_id = table.Column<int>(type: "int", nullable: false),
                    supplier_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    rating = table.Column<double>(type: "float", nullable: true),
                    weight = table.Column<double>(type: "float", nullable: true),
                    dimensions = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Item__52020FDDA4392901", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_Item_Brand",
                        column: x => x.brand_id,
                        principalTable: "Brand",
                        principalColumn: "brand_id");
                    table.ForeignKey(
                        name: "FK_Item_Category",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "FK_Item_Supplier",
                        column: x => x.supplier_id,
                        principalTable: "Supplier",
                        principalColumn: "supplier_id");
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    discount_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    item_id = table.Column<int>(type: "int", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    percent = table.Column<int>(type: "int", nullable: false),
                    date_from = table.Column<DateOnly>(type: "date", nullable: false),
                    date_to = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Discount__BDBE9EF95A6EC9E6", x => x.discount_id);
                    table.ForeignKey(
                        name: "FK_Discount_Category",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Discount_Item",
                        column: x => x.item_id,
                        principalTable: "Item",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Operator"),
                    status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Present"),
                    salary = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__C52E0BA8A5A707D5", x => x.employee_id);
                    table.ForeignKey(
                        name: "FK_Employee_AspNetUsers",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    store_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    city_id = table.Column<int>(type: "int", nullable: false),
                    admin_id = table.Column<int>(type: "int", nullable: true),
                    address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Store__A2F2A30C54797B75", x => x.store_id);
                    table.ForeignKey(
                        name: "FK_Store_Admin",
                        column: x => x.admin_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    operator_id = table.Column<int>(type: "int", nullable: true),
                    order_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    delivery_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "New"),
                    order_total = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValueSql: "((0.00))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order__465962290218EFAB", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Order_Client",
                        column: x => x.client_id,
                        principalTable: "Client",
                        principalColumn: "client_id");
                    table.ForeignKey(
                        name: "FK_Order_Operator",
                        column: x => x.operator_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK_Order_Store",
                        column: x => x.store_id,
                        principalTable: "Store",
                        principalColumn: "store_id");
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    warehouse_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    store_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    supply_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Warehous__734FE6BF63FC749B", x => x.warehouse_id);
                    table.ForeignKey(
                        name: "FK_Warehouse_Item",
                        column: x => x.item_id,
                        principalTable: "Item",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Warehouse_Store",
                        column: x => x.store_id,
                        principalTable: "Store",
                        principalColumn: "store_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderLine",
                columns: table => new
                {
                    order_line_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price_at_moment = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderLin__8F2B951FF85B7B4D", x => x.order_line_id);
                    table.ForeignKey(
                        name: "FK_OrderLine_Item",
                        column: x => x.item_id,
                        principalTable: "Item",
                        principalColumn: "item_id");
                    table.ForeignKey(
                        name: "FK_OrderLine_Order",
                        column: x => x.order_id,
                        principalTable: "Order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    payment_method_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__ED1FC9EAD3A18DE1", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_Payment_Order",
                        column: x => x.order_id,
                        principalTable: "Order",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_PaymentMethod",
                        column: x => x.payment_method_id,
                        principalTable: "PaymentMethod",
                        principalColumn: "payment_method_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_country_id",
                table: "Brand",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_Category_parent_category_id",
                table: "Category",
                column: "parent_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Client_user_id",
                table: "Client",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discount_category_id",
                table: "Discount",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_item_id",
                table: "Discount",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_store_id",
                table: "Employee",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_user_id",
                table: "Employee",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_action",
                table: "EventLog",
                column: "action");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_created_at",
                table: "EventLog",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_entity",
                table: "EventLog",
                column: "entity");

            migrationBuilder.CreateIndex(
                name: "IX_EventLog_role",
                table: "EventLog",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_Item_brand_id",
                table: "Item",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_Item_category_id",
                table: "Item",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Item_supplier_id",
                table: "Item",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_client_id",
                table: "Order",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_operator_id",
                table: "Order",
                column: "operator_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_store_id",
                table: "Order",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLine_item_id",
                table: "OrderLine",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLine_order_id",
                table: "OrderLine",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_order_id",
                table: "Payment",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_payment_method_id",
                table: "Payment",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_client_id",
                table: "PaymentMethod",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "UQ_PaymentMethod_Card",
                table: "PaymentMethod",
                column: "card_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Review_Client_Item",
                table: "Review",
                columns: new[] { "client_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Store_admin_id",
                table: "Store",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_country_id",
                table: "Supplier",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_item_id",
                table: "Warehouse",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Warehouse_Store_Item",
                table: "Warehouse",
                columns: new[] { "store_id", "item_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Store",
                table: "Employee",
                column: "store_id",
                principalTable: "Store",
                principalColumn: "store_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Store",
                table: "Employee");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "EventLog");

            migrationBuilder.DropTable(
                name: "OrderLine");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "Employee");
        }
    }
}
