using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SampleAPI.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Biography = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    State = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UserType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    MembershipNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    JoinedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    City = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    State = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    HiredOn = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RentedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rentals_LibraryUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "LibraryUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rentals_LibraryUsers_StaffId",
                        column: x => x.StaffId,
                        principalTable: "LibraryUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PublicationYear = table.Column<int>(type: "int", nullable: false),
                    Edition = table.Column<int>(type: "int", nullable: false),
                    Pages = table.Column<int>(type: "int", nullable: false),
                    ReplacementCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookAuthors",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthors", x => new { x.BookId, x.AuthorId });
                    table.ForeignKey(
                        name: "FK_BookAuthors_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthors_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookCategories",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCategories", x => new { x.BookId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_BookCategories_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookCopies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AcquiredOn = table.Column<DateOnly>(type: "date", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ShelfLocation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    InventoryLabel = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true, computedColumnSql: "[Barcode] + ' @ ' + [ShelfLocation]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCopies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookCopies_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCopies_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_LibraryUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "LibraryUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RentalLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookCopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DailyLateFee = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    LateFeeCharged = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsReturned = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalLines_BookCopies_BookCopyId",
                        column: x => x.BookCopyId,
                        principalTable: "BookCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalLines_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookCopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_BookCopies_BookCopyId",
                        column: x => x.BookCopyId,
                        principalTable: "BookCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_LibraryUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "LibraryUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "BirthDate", "FirstName", "LastName", "NickName" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), "نویسنده برجسته داستان مدرن فارسی.", new DateOnly(1903, 2, 17), "صادق", "هدایت", null },
                    { new Guid("33333333-3333-3333-3333-333333333332"), "نویسنده و مترجم ایرانی و خالق رمان سووشون.", new DateOnly(1921, 4, 28), "سیمین", "دانشور", null },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "رمان‌نویس معاصر ایرانی و نویسنده کلیدر.", new DateOnly(1940, 8, 1), "محمود", "دولت‌آبادی", null },
                    { new Guid("33333333-3333-3333-3333-333333333334"), "نویسنده تاثیرگذار داستان‌نویسی معاصر فارسی.", new DateOnly(1938, 3, 16), "هوشنگ", "گلشیری", null },
                    { new Guid("33333333-3333-3333-3333-333333333335"), "شاعر و نویسنده بزرگ فارسی‌زبان.", null, "سعدی", "شیرازی", null },
                    { new Guid("33333333-3333-3333-3333-333333333336"), "غزل‌سرای نامدار ایرانی.", null, "حافظ", "شیرازی", null },
                    { new Guid("33333333-3333-3333-3333-333333333337"), "سراینده شاهنامه و احیاگر زبان فارسی.", null, "ابوالقاسم", "فردوسی", null },
                    { new Guid("33333333-3333-3333-3333-333333333338"), "شاعر نوگرای معاصر ایران.", new DateOnly(1935, 1, 5), "فروغ", "فرخزاد", null }
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Name", "Phone", "City", "Country", "PostalCode", "State", "Street" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "کتابخانه مرکزی تهران", "+98-21-88990011", "تهران", "ایران", "1417933161", "تهران", "خیابان انقلاب، نبش وصال" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "کتابخانه حافظ شیراز", "+98-71-32220011", "شیراز", "ایران", "7136453111", "فارس", "خیابان حافظیه، روبروی باغ جهان‌نما" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "کتابخانه نقش جهان اصفهان", "+98-31-36660011", "اصفهان", "ایران", "8146734611", "اصفهان", "میدان نقش جهان، گذر کتاب" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444441"), "رمان‌های فارسی و آثار داستانی بلند.", "رمان" },
                    { new Guid("44444444-4444-4444-4444-444444444442"), "آثار ماندگار ادبیات فارسی.", "ادبیات کلاسیک" },
                    { new Guid("44444444-4444-4444-4444-444444444443"), "مجموعه‌های شعر کلاسیک و معاصر.", "شعر" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "آثار حماسی و اسطوره‌ای.", "حماسه" },
                    { new Guid("44444444-4444-4444-4444-444444444445"), "داستان‌های کوتاه فارسی.", "داستان کوتاه" },
                    { new Guid("44444444-4444-4444-4444-444444444446"), "آثار نویسندگان معاصر ایران.", "ادبیات معاصر" }
                });

            migrationBuilder.InsertData(
                table: "LibraryUsers",
                columns: new[] { "Id", "CreatedAtUtc", "Email", "FirstName", "JoinedOn", "LastName", "MembershipNumber", "Phone", "Status", "UserType", "City", "Country", "PostalCode", "State", "Street" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777771"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "mina.ahmadi@example.ir", "مینا", new DateOnly(2025, 1, 12), "احمدی", "IR-MBR-1001", "+98-912-100-1001", "Active", "Customer", "تهران", "ایران", "1587654312", "تهران", "خیابان مطهری، پلاک ۱۲" },
                    { new Guid("77777777-7777-7777-7777-777777777772"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "omid.rahimi@example.ir", "امید", new DateOnly(2025, 2, 20), "رحیمی", "IR-MBR-1002", "+98-912-100-1002", "Active", "Customer", "تهران", "ایران", "1418845671", "تهران", "بلوار کشاورز، پلاک ۴۵" },
                    { new Guid("77777777-7777-7777-7777-777777777773"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "niloofar.kazemi@example.ir", "نیلوفر", new DateOnly(2025, 3, 8), "کاظمی", "IR-MBR-1003", "+98-912-100-1003", "Active", "Customer", "شیراز", "ایران", "7188832145", "فارس", "خیابان قصردشت، کوچه سرو" },
                    { new Guid("77777777-7777-7777-7777-777777777774"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "reza.mousavi@example.ir", "رضا", new DateOnly(2025, 4, 18), "موسوی", "IR-MBR-1004", "+98-912-100-1004", "Active", "Customer", "اصفهان", "ایران", "8174678912", "اصفهان", "خیابان چهارباغ بالا، پلاک ۹" },
                    { new Guid("77777777-7777-7777-7777-777777777775"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "sara.noori@example.ir", "سارا", new DateOnly(2025, 5, 4), "نوری", "IR-MBR-1005", "+98-912-100-1005", "Active", "Customer", "تهران", "ایران", "1967743215", "تهران", "خیابان ولیعصر، کوچه نیلوفر" },
                    { new Guid("77777777-7777-7777-7777-777777777776"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "kaveh.moradi@example.ir", "کاوه", new DateOnly(2025, 6, 10), "مرادی", "IR-MBR-1006", "+98-912-100-1006", "Suspended", "Customer", "شیراز", "ایران", "7134889123", "فارس", "خیابان زند، پلاک ۱۸" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "leila.sadeghi@example.ir", "لیلا", new DateOnly(2025, 7, 22), "صادقی", "IR-MBR-1007", "+98-912-100-1007", "Active", "Customer", "اصفهان", "ایران", "8145672341", "اصفهان", "خیابان آمادگاه، پلاک ۲۲" },
                    { new Guid("77777777-7777-7777-7777-777777777778"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "arash.karimi@example.ir", "آرش", new DateOnly(2025, 8, 15), "کریمی", "IR-MBR-1008", "+98-912-100-1008", "Active", "Customer", "تهران", "ایران", "1934678911", "تهران", "خیابان شریعتی، پلاک ۷۰" }
                });

            migrationBuilder.InsertData(
                table: "LibraryUsers",
                columns: new[] { "Id", "CreatedAtUtc", "Email", "EmployeeNumber", "FirstName", "HiredOn", "LastName", "Phone", "UserType" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888881"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "sara.yousefi@persian-library.local", "IR-EMP-001", "سارا", new DateOnly(2023, 4, 1), "یوسفی", "+98-21-8899-2001", "Staff" },
                    { new Guid("88888888-8888-8888-8888-888888888882"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "mehrdad.jafari@persian-library.local", "IR-EMP-002", "مهرداد", new DateOnly(2023, 9, 10), "جعفری", "+98-71-3222-2002", "Staff" },
                    { new Guid("88888888-8888-8888-8888-888888888883"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "parisa.hosseini@persian-library.local", "IR-EMP-003", "پریسا", new DateOnly(2024, 2, 5), "حسینی", "+98-31-3666-2003", "Staff" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "Name", "Website" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222221"), "انتشارات امیرکبیر", "https://amirkabirpub.ir" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "انتشارات سخن", "https://sokhanpub.net" },
                    { new Guid("22222222-2222-2222-2222-222222222223"), "نشر نیلوفر", "https://niloofarbooks.com" },
                    { new Guid("22222222-2222-2222-2222-222222222224"), "نشر مرکز", "https://markazpub.com" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "CreatedAtUtc", "Description", "Edition", "IsDeleted", "Isbn", "Pages", "PublicationYear", "PublisherId", "ReplacementCost", "Title", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555551"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "رمان کوتاه و تاثیرگذار صادق هدایت در ادبیات مدرن فارسی.", 5, false, "9789640010011", 128, 1937, new Guid("22222222-2222-2222-2222-222222222223"), 350000m, "بوف کور", null },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "رمانی اجتماعی از سیمین دانشور درباره شیراز در سال‌های جنگ جهانی دوم.", 12, false, "9789640010028", 304, 1969, new Guid("22222222-2222-2222-2222-222222222222"), 520000m, "سووشون", null },
                    { new Guid("55555555-5555-5555-5555-555555555553"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "رمان بلند محمود دولت‌آبادی درباره زندگی روستایی و عشایری خراسان.", 8, false, "9789640010035", 2836, 1984, new Guid("22222222-2222-2222-2222-222222222222"), 2200000m, "کلیدر", null },
                    { new Guid("55555555-5555-5555-5555-555555555554"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "رمانی ساختارمند از هوشنگ گلشیری درباره فروپاشی یک خاندان قاجاری.", 7, false, "9789640010042", 112, 1969, new Guid("22222222-2222-2222-2222-222222222223"), 310000m, "شازده احتجاب", null },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "نثر آهنگین و حکایت‌های اخلاقی سعدی شیرازی.", 20, false, "9789640010059", 360, 1258, new Guid("22222222-2222-2222-2222-222222222221"), 480000m, "گلستان سعدی", null },
                    { new Guid("55555555-5555-5555-5555-555555555556"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "گزیده غزل‌های حافظ شیرازی با شرح واژگان.", 15, false, "9789640010066", 640, 1399, new Guid("22222222-2222-2222-2222-222222222221"), 680000m, "دیوان حافظ", null },
                    { new Guid("55555555-5555-5555-5555-555555555557"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "متن برگزیده شاهنامه برای مطالعه عمومی.", 10, false, "9789640010073", 1216, 1010, new Guid("22222222-2222-2222-2222-222222222222"), 1350000m, "شاهنامه فردوسی", null },
                    { new Guid("55555555-5555-5555-5555-555555555558"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "مجموعه‌ای از شعرهای فروغ فرخزاد در جریان شعر نو فارسی.", 9, false, "9789640010080", 176, 1964, new Guid("22222222-2222-2222-2222-222222222224"), 390000m, "تولدی دیگر", null }
                });

            migrationBuilder.InsertData(
                table: "Rentals",
                columns: new[] { "Id", "BranchId", "ClosedAtUtc", "CustomerId", "DueAtUtc", "RentalNumber", "RentedAtUtc", "StaffId", "Status" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999991"), new Guid("11111111-1111-1111-1111-111111111111"), null, new Guid("77777777-7777-7777-7777-777777777771"), new DateTime(2026, 6, 15, 10, 0, 0, 0, DateTimeKind.Utc), "IR-RNT-2026-0001", new DateTime(2026, 6, 1, 10, 0, 0, 0, DateTimeKind.Utc), new Guid("88888888-8888-8888-8888-888888888881"), "Open" },
                    { new Guid("99999999-9999-9999-9999-999999999993"), new Guid("11111111-1111-1111-1111-111111111113"), new DateTime(2026, 5, 20, 16, 0, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777774"), new DateTime(2026, 5, 24, 9, 0, 0, 0, DateTimeKind.Utc), "IR-RNT-2026-0002", new DateTime(2026, 5, 10, 9, 0, 0, 0, DateTimeKind.Utc), new Guid("88888888-8888-8888-8888-888888888883"), "Closed" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId", "SortOrder" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), new Guid("55555555-5555-5555-5555-555555555551"), 1 },
                    { new Guid("33333333-3333-3333-3333-333333333332"), new Guid("55555555-5555-5555-5555-555555555552"), 1 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("55555555-5555-5555-5555-555555555553"), 1 },
                    { new Guid("33333333-3333-3333-3333-333333333334"), new Guid("55555555-5555-5555-5555-555555555554"), 1 },
                    { new Guid("33333333-3333-3333-3333-333333333335"), new Guid("55555555-5555-5555-5555-555555555555"), 1 },
                    { new Guid("33333333-3333-3333-3333-333333333336"), new Guid("55555555-5555-5555-5555-555555555556"), 1 },
                    { new Guid("33333333-3333-3333-3333-333333333337"), new Guid("55555555-5555-5555-5555-555555555557"), 1 },
                    { new Guid("33333333-3333-3333-3333-333333333338"), new Guid("55555555-5555-5555-5555-555555555558"), 1 }
                });

            migrationBuilder.InsertData(
                table: "BookCategories",
                columns: new[] { "BookId", "CategoryId" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555551"), new Guid("44444444-4444-4444-4444-444444444441") },
                    { new Guid("55555555-5555-5555-5555-555555555551"), new Guid("44444444-4444-4444-4444-444444444446") },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new Guid("44444444-4444-4444-4444-444444444441") },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new Guid("44444444-4444-4444-4444-444444444446") },
                    { new Guid("55555555-5555-5555-5555-555555555553"), new Guid("44444444-4444-4444-4444-444444444441") },
                    { new Guid("55555555-5555-5555-5555-555555555553"), new Guid("44444444-4444-4444-4444-444444444446") },
                    { new Guid("55555555-5555-5555-5555-555555555554"), new Guid("44444444-4444-4444-4444-444444444441") },
                    { new Guid("55555555-5555-5555-5555-555555555554"), new Guid("44444444-4444-4444-4444-444444444446") },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("44444444-4444-4444-4444-444444444442") },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("44444444-4444-4444-4444-444444444445") },
                    { new Guid("55555555-5555-5555-5555-555555555556"), new Guid("44444444-4444-4444-4444-444444444442") },
                    { new Guid("55555555-5555-5555-5555-555555555556"), new Guid("44444444-4444-4444-4444-444444444443") },
                    { new Guid("55555555-5555-5555-5555-555555555557"), new Guid("44444444-4444-4444-4444-444444444442") },
                    { new Guid("55555555-5555-5555-5555-555555555557"), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("55555555-5555-5555-5555-555555555558"), new Guid("44444444-4444-4444-4444-444444444443") },
                    { new Guid("55555555-5555-5555-5555-555555555558"), new Guid("44444444-4444-4444-4444-444444444446") }
                });

            migrationBuilder.InsertData(
                table: "BookCopies",
                columns: new[] { "Id", "AcquiredOn", "Barcode", "BookId", "BranchId", "PurchasePrice", "ShelfLocation", "Status" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666661"), new DateOnly(2024, 2, 10), "IR-BOF-001", new Guid("55555555-5555-5555-5555-555555555551"), new Guid("11111111-1111-1111-1111-111111111111"), 250000m, "تهران-A1", "Rented" },
                    { new Guid("66666666-6666-6666-6666-666666666662"), new DateOnly(2024, 2, 10), "IR-BOF-002", new Guid("55555555-5555-5555-5555-555555555551"), new Guid("11111111-1111-1111-1111-111111111112"), 250000m, "شیراز-B2", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666663"), new DateOnly(2024, 3, 5), "IR-SUV-001", new Guid("55555555-5555-5555-5555-555555555552"), new Guid("11111111-1111-1111-1111-111111111112"), 410000m, "شیراز-A3", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666664"), new DateOnly(2024, 3, 5), "IR-SUV-002", new Guid("55555555-5555-5555-5555-555555555552"), new Guid("11111111-1111-1111-1111-111111111111"), 410000m, "تهران-C1", "Reserved" },
                    { new Guid("66666666-6666-6666-6666-666666666665"), new DateOnly(2024, 4, 12), "IR-KEL-001", new Guid("55555555-5555-5555-5555-555555555553"), new Guid("11111111-1111-1111-1111-111111111111"), 1850000m, "تهران-D4", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateOnly(2024, 4, 12), "IR-KEL-002", new Guid("55555555-5555-5555-5555-555555555553"), new Guid("11111111-1111-1111-1111-111111111113"), 1850000m, "اصفهان-E1", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666667"), new DateOnly(2024, 5, 3), "IR-SHAZ-001", new Guid("55555555-5555-5555-5555-555555555554"), new Guid("11111111-1111-1111-1111-111111111111"), 230000m, "تهران-B1", "Rented" },
                    { new Guid("66666666-6666-6666-6666-666666666668"), new DateOnly(2024, 5, 3), "IR-SHAZ-002", new Guid("55555555-5555-5555-5555-555555555554"), new Guid("11111111-1111-1111-1111-111111111113"), 230000m, "اصفهان-A2", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666669"), new DateOnly(2024, 6, 1), "IR-GOL-001", new Guid("55555555-5555-5555-5555-555555555555"), new Guid("11111111-1111-1111-1111-111111111112"), 360000m, "شیراز-G1", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666670"), new DateOnly(2024, 6, 1), "IR-GOL-002", new Guid("55555555-5555-5555-5555-555555555555"), new Guid("11111111-1111-1111-1111-111111111113"), 360000m, "اصفهان-C3", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666671"), new DateOnly(2024, 7, 8), "IR-HAF-001", new Guid("55555555-5555-5555-5555-555555555556"), new Guid("11111111-1111-1111-1111-111111111112"), 520000m, "شیراز-H1", "Reserved" },
                    { new Guid("66666666-6666-6666-6666-666666666672"), new DateOnly(2024, 7, 8), "IR-HAF-002", new Guid("55555555-5555-5555-5555-555555555556"), new Guid("11111111-1111-1111-1111-111111111111"), 520000m, "تهران-F2", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666673"), new DateOnly(2024, 8, 20), "IR-SHN-001", new Guid("55555555-5555-5555-5555-555555555557"), new Guid("11111111-1111-1111-1111-111111111111"), 1100000m, "تهران-S1", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666674"), new DateOnly(2024, 8, 20), "IR-SHN-002", new Guid("55555555-5555-5555-5555-555555555557"), new Guid("11111111-1111-1111-1111-111111111113"), 1100000m, "اصفهان-S2", "Available" },
                    { new Guid("66666666-6666-6666-6666-666666666675"), new DateOnly(2024, 9, 14), "IR-FOR-001", new Guid("55555555-5555-5555-5555-555555555558"), new Guid("11111111-1111-1111-1111-111111111111"), 290000m, "تهران-P1", "Reserved" },
                    { new Guid("66666666-6666-6666-6666-666666666676"), new DateOnly(2024, 9, 14), "IR-FOR-002", new Guid("55555555-5555-5555-5555-555555555558"), new Guid("11111111-1111-1111-1111-111111111112"), 290000m, "شیراز-P3", "Available" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "BookId", "Comment", "CreatedAtUtc", "CustomerId", "Rating" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), new Guid("55555555-5555-5555-5555-555555555551"), "فضاسازی بوف کور هنوز تکان‌دهنده و متفاوت است.", new DateTime(2026, 6, 4, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777772"), 5 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), new Guid("55555555-5555-5555-5555-555555555552"), "روایت شیراز و شخصیت‌پردازی زری بسیار ماندگار است.", new DateTime(2026, 6, 5, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777771"), 5 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), new Guid("55555555-5555-5555-5555-555555555553"), "طولانی اما پرجزئیات و ارزشمند.", new DateTime(2026, 6, 6, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777773"), 4 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), new Guid("55555555-5555-5555-5555-555555555556"), "نسخه خوش‌خوان و مناسب برای مراجعه روزانه.", new DateTime(2026, 6, 7, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777775"), 5 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), new Guid("55555555-5555-5555-5555-555555555557"), "برای شروع مطالعه شاهنامه انتخاب خوبی است.", new DateTime(2026, 6, 8, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777778"), 5 },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"), new Guid("55555555-5555-5555-5555-555555555558"), "شعرها زنده، مستقیم و تاثیرگذارند.", new DateTime(2026, 6, 9, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("77777777-7777-7777-7777-777777777777"), 4 }
                });

            migrationBuilder.InsertData(
                table: "RentalLines",
                columns: new[] { "Id", "BookCopyId", "DailyLateFee", "IsReturned", "LateFeeCharged", "RentalId", "ReturnedAtUtc" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999992"), new Guid("66666666-6666-6666-6666-666666666661"), 25000m, false, 0m, new Guid("99999999-9999-9999-9999-999999999991"), null },
                    { new Guid("99999999-9999-9999-9999-999999999994"), new Guid("66666666-6666-6666-6666-666666666667"), 25000m, true, 0m, new Guid("99999999-9999-9999-9999-999999999993"), new DateTime(2026, 5, 20, 16, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "BookCopyId", "CustomerId", "ExpiresAtUtc", "ReservedAtUtc", "Status" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"), new Guid("66666666-6666-6666-6666-666666666671"), new Guid("77777777-7777-7777-7777-777777777772"), new DateTime(2026, 6, 10, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 3, 9, 0, 0, 0, DateTimeKind.Utc), "Active" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"), new Guid("66666666-6666-6666-6666-666666666675"), new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 6, 11, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 4, 11, 0, 0, 0, DateTimeKind.Utc), "Active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_LastName_FirstName",
                table: "Authors",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthors_AuthorId_SortOrder",
                table: "BookAuthors",
                columns: new[] { "AuthorId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BookCategories_CategoryId",
                table: "BookCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_Barcode",
                table: "BookCopies",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_BookId",
                table: "BookCopies",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_BranchId",
                table: "BookCopies",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Isbn",
                table: "Books",
                column: "Isbn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherId",
                table: "Books",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Name",
                table: "Branches",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryUsers_Email",
                table: "LibraryUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LibraryUsers_EmployeeNumber",
                table: "LibraryUsers",
                column: "EmployeeNumber",
                unique: true,
                filter: "[EmployeeNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryUsers_MembershipNumber",
                table: "LibraryUsers",
                column: "MembershipNumber",
                unique: true,
                filter: "[MembershipNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_Name",
                table: "Publishers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalLines_BookCopyId",
                table: "RentalLines",
                column: "BookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalLines_RentalId",
                table: "RentalLines",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_BranchId",
                table: "Rentals",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CustomerId",
                table: "Rentals",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_RentalNumber",
                table: "Rentals",
                column: "RentalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_StaffId",
                table: "Rentals",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookCopyId",
                table: "Reservations",
                column: "BookCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerId_BookCopyId_Status",
                table: "Reservations",
                columns: new[] { "CustomerId", "BookCopyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookId_CustomerId",
                table: "Reviews",
                columns: new[] { "BookId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthors");

            migrationBuilder.DropTable(
                name: "BookCategories");

            migrationBuilder.DropTable(
                name: "RentalLines");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "BookCopies");

            migrationBuilder.DropTable(
                name: "LibraryUsers");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
