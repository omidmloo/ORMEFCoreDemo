using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleAPI.Entities;

namespace SampleAPI.Context;

public class LiberaryDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<LibraryBranch> Branches => Set<LibraryBranch>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<RentalLine> RentalLines => Set<RentalLine>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Staff> Staff => Set<Staff>();

    public LiberaryDbContext(DbContextOptions<LiberaryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureCatalog(modelBuilder);
        ConfigureInventory(modelBuilder);
        ConfigureRentals(modelBuilder);
        ConfigureSeedData(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryUser>(entity =>
        {
            entity.ToTable("LibraryUsers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FirstName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            entity.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasDiscriminator<string>("UserType")
                .HasValue<Customer>("Customer")
                .HasValue<Staff>("Staff");
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.MembershipNumber).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(x => x.MembershipNumber).IsUnique();
            entity.OwnsOne(x => x.Address, ConfigureAddress);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.Property(x => x.EmployeeNumber).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => x.EmployeeNumber).IsUnique();
        });
    }

    private static void ConfigureCatalog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("Publishers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Website).HasMaxLength(300);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Authors");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FirstName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Biography).HasMaxLength(2000);
            entity.HasIndex(x => new { x.LastName, x.FirstName });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Isbn).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(250).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.ReplacementCost).HasPrecision(10, 2);
            entity.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasQueryFilter(x => !x.IsDeleted);
            entity.HasIndex(x => x.Isbn).IsUnique();
            entity.HasIndex(x => x.Title);
            entity.HasOne(x => x.Publisher)
                .WithMany(x => x.Books)
                .HasForeignKey(x => x.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.ToTable("BookAuthors");
            entity.HasKey(x => new { x.BookId, x.AuthorId });
            entity.HasQueryFilter(x => !x.Book.IsDeleted);
            entity.HasOne(x => x.Book).WithMany(x => x.Authors).HasForeignKey(x => x.BookId);
            entity.HasOne(x => x.Author).WithMany(x => x.Books).HasForeignKey(x => x.AuthorId);
            entity.HasIndex(x => new { x.AuthorId, x.SortOrder });
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.ToTable("BookCategories");
            entity.HasKey(x => new { x.BookId, x.CategoryId });
            entity.HasQueryFilter(x => !x.Book.IsDeleted);
            entity.HasOne(x => x.Book).WithMany(x => x.Categories).HasForeignKey(x => x.BookId);
            entity.HasOne(x => x.Category).WithMany(x => x.Books).HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews", table => table.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5"));
            entity.HasKey(x => x.Id);
            entity.HasQueryFilter(x => !x.Book.IsDeleted);
            entity.Property(x => x.Comment).HasMaxLength(1000);
            entity.HasIndex(x => new { x.BookId, x.CustomerId }).IsUnique();
            entity.HasOne(x => x.Book).WithMany(x => x.Reviews).HasForeignKey(x => x.BookId);
            entity.HasOne(x => x.Customer).WithMany(x => x.Reviews).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureInventory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LibraryBranch>(entity =>
        {
            entity.ToTable("Branches");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
            entity.OwnsOne(x => x.Address, ConfigureAddress);
        });

        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.ToTable("BookCopies");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Barcode).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.PurchasePrice).HasPrecision(10, 2);
            entity.Property(x => x.ShelfLocation).HasMaxLength(30).IsRequired();
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.Property<string>("InventoryLabel")
                .HasMaxLength(80)
                .HasComputedColumnSql("[Barcode] + ' @ ' + [ShelfLocation]");
            entity.HasQueryFilter(x => !x.Book.IsDeleted);
            entity.HasIndex(x => x.Barcode).IsUnique();
            entity.HasOne(x => x.Book).WithMany(x => x.Copies).HasForeignKey(x => x.BookId);
            entity.HasOne(x => x.Branch).WithMany(x => x.Copies).HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRentals(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.ToTable("Rentals");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RentalNumber).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(x => x.RentalNumber).IsUnique();
            entity.HasOne(x => x.Customer).WithMany(x => x.Rentals).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Staff).WithMany(x => x.Rentals).HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Branch).WithMany(x => x.Rentals).HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RentalLine>(entity =>
        {
            entity.ToTable("RentalLines");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.DailyLateFee).HasPrecision(10, 2);
            entity.Property(x => x.LateFeeCharged).HasPrecision(10, 2);
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.HasQueryFilter(x => !x.BookCopy.Book.IsDeleted);
            entity.HasOne(x => x.Rental).WithMany(x => x.Lines).HasForeignKey(x => x.RentalId);
            entity.HasOne(x => x.BookCopy).WithMany(x => x.RentalLines).HasForeignKey(x => x.BookCopyId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasQueryFilter(x => !x.BookCopy.Book.IsDeleted);
            entity.HasIndex(x => new { x.CustomerId, x.BookCopyId, x.Status });
            entity.HasOne(x => x.Customer).WithMany(x => x.Reservations).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.BookCopy).WithMany(x => x.Reservations).HasForeignKey(x => x.BookCopyId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureAddress<T>(OwnedNavigationBuilder<T, Address> address) where T : class
    {
        address.Property(x => x.Street).HasMaxLength(180).HasColumnName("Street").IsRequired();
        address.Property(x => x.City).HasMaxLength(80).HasColumnName("City").IsRequired();
        address.Property(x => x.State).HasMaxLength(80).HasColumnName("State").IsRequired();
        address.Property(x => x.PostalCode).HasMaxLength(20).HasColumnName("PostalCode").IsRequired();
        address.Property(x => x.Country).HasMaxLength(80).HasColumnName("Country").IsRequired();
    }

    private static void ConfigureSeedData(ModelBuilder modelBuilder)
    {
        var branchCentral = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var branchShiraz = Guid.Parse("11111111-1111-1111-1111-111111111112");
        var branchIsfahan = Guid.Parse("11111111-1111-1111-1111-111111111113");

        var publisherAmirkabir = Guid.Parse("22222222-2222-2222-2222-222222222221");
        var publisherSokhan = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var publisherNiloufar = Guid.Parse("22222222-2222-2222-2222-222222222223");
        var publisherMarkaz = Guid.Parse("22222222-2222-2222-2222-222222222224");

        var authorHedayat = Guid.Parse("33333333-3333-3333-3333-333333333331");
        var authorDaneshvar = Guid.Parse("33333333-3333-3333-3333-333333333332");
        var authorDolatabadi = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var authorGolshiri = Guid.Parse("33333333-3333-3333-3333-333333333334");
        var authorSaadi = Guid.Parse("33333333-3333-3333-3333-333333333335");
        var authorHafez = Guid.Parse("33333333-3333-3333-3333-333333333336");
        var authorFerdowsi = Guid.Parse("33333333-3333-3333-3333-333333333337");
        var authorForough = Guid.Parse("33333333-3333-3333-3333-333333333338");

        var categoryNovel = Guid.Parse("44444444-4444-4444-4444-444444444441");
        var categoryClassic = Guid.Parse("44444444-4444-4444-4444-444444444442");
        var categoryPoetry = Guid.Parse("44444444-4444-4444-4444-444444444443");
        var categoryEpic = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var categoryShortStory = Guid.Parse("44444444-4444-4444-4444-444444444445");
        var categoryContemporary = Guid.Parse("44444444-4444-4444-4444-444444444446");

        var bookBlindOwl = Guid.Parse("55555555-5555-5555-5555-555555555551");
        var bookSuvashun = Guid.Parse("55555555-5555-5555-5555-555555555552");
        var bookKelidar = Guid.Parse("55555555-5555-5555-5555-555555555553");
        var bookPrince = Guid.Parse("55555555-5555-5555-5555-555555555554");
        var bookGolestan = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var bookDivanHafez = Guid.Parse("55555555-5555-5555-5555-555555555556");
        var bookShahnameh = Guid.Parse("55555555-5555-5555-5555-555555555557");
        var bookAnotherBirth = Guid.Parse("55555555-5555-5555-5555-555555555558");

        var copyBlindOwl1 = Guid.Parse("66666666-6666-6666-6666-666666666661");
        var copyBlindOwl2 = Guid.Parse("66666666-6666-6666-6666-666666666662");
        var copySuvashun1 = Guid.Parse("66666666-6666-6666-6666-666666666663");
        var copySuvashun2 = Guid.Parse("66666666-6666-6666-6666-666666666664");
        var copyKelidar1 = Guid.Parse("66666666-6666-6666-6666-666666666665");
        var copyKelidar2 = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var copyPrince1 = Guid.Parse("66666666-6666-6666-6666-666666666667");
        var copyPrince2 = Guid.Parse("66666666-6666-6666-6666-666666666668");
        var copyGolestan1 = Guid.Parse("66666666-6666-6666-6666-666666666669");
        var copyGolestan2 = Guid.Parse("66666666-6666-6666-6666-666666666670");
        var copyHafez1 = Guid.Parse("66666666-6666-6666-6666-666666666671");
        var copyHafez2 = Guid.Parse("66666666-6666-6666-6666-666666666672");
        var copyShahnameh1 = Guid.Parse("66666666-6666-6666-6666-666666666673");
        var copyShahnameh2 = Guid.Parse("66666666-6666-6666-6666-666666666674");
        var copyForough1 = Guid.Parse("66666666-6666-6666-6666-666666666675");
        var copyForough2 = Guid.Parse("66666666-6666-6666-6666-666666666676");

        var customerMina = Guid.Parse("77777777-7777-7777-7777-777777777771");
        var customerOmid = Guid.Parse("77777777-7777-7777-7777-777777777772");
        var customerNiloofar = Guid.Parse("77777777-7777-7777-7777-777777777773");
        var customerReza = Guid.Parse("77777777-7777-7777-7777-777777777774");
        var customerSara = Guid.Parse("77777777-7777-7777-7777-777777777775");
        var customerKaveh = Guid.Parse("77777777-7777-7777-7777-777777777776");
        var customerLeila = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var customerArash = Guid.Parse("77777777-7777-7777-7777-777777777778");

        var staffSara = Guid.Parse("88888888-8888-8888-8888-888888888881");
        var staffMehrdad = Guid.Parse("88888888-8888-8888-8888-888888888882");
        var staffParisa = Guid.Parse("88888888-8888-8888-8888-888888888883");

        var rentalOpen = Guid.Parse("99999999-9999-9999-9999-999999999991");
        var rentalClosed = Guid.Parse("99999999-9999-9999-9999-999999999993");
        var rentalLineOpen = Guid.Parse("99999999-9999-9999-9999-999999999992");
        var rentalLineClosed = Guid.Parse("99999999-9999-9999-9999-999999999994");
        var reservationHafez = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        var reservationForough = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");

        modelBuilder.Entity<LibraryBranch>().HasData(
            new { Id = branchCentral, Name = "کتابخانه مرکزی تهران", Phone = "+98-21-88990011" },
            new { Id = branchShiraz, Name = "کتابخانه حافظ شیراز", Phone = "+98-71-32220011" },
            new { Id = branchIsfahan, Name = "کتابخانه نقش جهان اصفهان", Phone = "+98-31-36660011" });

        modelBuilder.Entity<LibraryBranch>().OwnsOne(x => x.Address).HasData(
            new { LibraryBranchId = branchCentral, Street = "خیابان انقلاب، نبش وصال", City = "تهران", State = "تهران", PostalCode = "1417933161", Country = "ایران" },
            new { LibraryBranchId = branchShiraz, Street = "خیابان حافظیه، روبروی باغ جهان‌نما", City = "شیراز", State = "فارس", PostalCode = "7136453111", Country = "ایران" },
            new { LibraryBranchId = branchIsfahan, Street = "میدان نقش جهان، گذر کتاب", City = "اصفهان", State = "اصفهان", PostalCode = "8146734611", Country = "ایران" });

        modelBuilder.Entity<Publisher>().HasData(
            new Publisher { Id = publisherAmirkabir, Name = "انتشارات امیرکبیر", Website = "https://amirkabirpub.ir" },
            new Publisher { Id = publisherSokhan, Name = "انتشارات سخن", Website = "https://sokhanpub.net" },
            new Publisher { Id = publisherNiloufar, Name = "نشر نیلوفر", Website = "https://niloofarbooks.com" },
            new Publisher { Id = publisherMarkaz, Name = "نشر مرکز", Website = "https://markazpub.com" });

        modelBuilder.Entity<Author>().HasData(
            new Author { Id = authorHedayat, FirstName = "صادق", LastName = "هدایت", BirthDate = new DateOnly(1903, 2, 17), Biography = "نویسنده برجسته داستان مدرن فارسی." },
            new Author { Id = authorDaneshvar, FirstName = "سیمین", LastName = "دانشور", BirthDate = new DateOnly(1921, 4, 28), Biography = "نویسنده و مترجم ایرانی و خالق رمان سووشون." },
            new Author { Id = authorDolatabadi, FirstName = "محمود", LastName = "دولت‌آبادی", BirthDate = new DateOnly(1940, 8, 1), Biography = "رمان‌نویس معاصر ایرانی و نویسنده کلیدر." },
            new Author { Id = authorGolshiri, FirstName = "هوشنگ", LastName = "گلشیری", BirthDate = new DateOnly(1938, 3, 16), Biography = "نویسنده تاثیرگذار داستان‌نویسی معاصر فارسی." },
            new Author { Id = authorSaadi, FirstName = "سعدی", LastName = "شیرازی", Biography = "شاعر و نویسنده بزرگ فارسی‌زبان." },
            new Author { Id = authorHafez, FirstName = "حافظ", LastName = "شیرازی", Biography = "غزل‌سرای نامدار ایرانی." },
            new Author { Id = authorFerdowsi, FirstName = "ابوالقاسم", LastName = "فردوسی", Biography = "سراینده شاهنامه و احیاگر زبان فارسی." },
            new Author { Id = authorForough, FirstName = "فروغ", LastName = "فرخزاد", BirthDate = new DateOnly(1935, 1, 5), Biography = "شاعر نوگرای معاصر ایران." });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = categoryNovel, Name = "رمان", Description = "رمان‌های فارسی و آثار داستانی بلند." },
            new Category { Id = categoryClassic, Name = "ادبیات کلاسیک", Description = "آثار ماندگار ادبیات فارسی." },
            new Category { Id = categoryPoetry, Name = "شعر", Description = "مجموعه‌های شعر کلاسیک و معاصر." },
            new Category { Id = categoryEpic, Name = "حماسه", Description = "آثار حماسی و اسطوره‌ای." },
            new Category { Id = categoryShortStory, Name = "داستان کوتاه", Description = "داستان‌های کوتاه فارسی." },
            new Category { Id = categoryContemporary, Name = "ادبیات معاصر", Description = "آثار نویسندگان معاصر ایران." });

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = bookBlindOwl, Isbn = "9789640010011", Title = "بوف کور", Description = "رمان کوتاه و تاثیرگذار صادق هدایت در ادبیات مدرن فارسی.", PublicationYear = 1937, Edition = 5, Pages = 128, ReplacementCost = 350000m, PublisherId = publisherNiloufar, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { Id = bookSuvashun, Isbn = "9789640010028", Title = "سووشون", Description = "رمانی اجتماعی از سیمین دانشور درباره شیراز در سال‌های جنگ جهانی دوم.", PublicationYear = 1969, Edition = 12, Pages = 304, ReplacementCost = 520000m, PublisherId = publisherSokhan, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { Id = bookKelidar, Isbn = "9789640010035", Title = "کلیدر", Description = "رمان بلند محمود دولت‌آبادی درباره زندگی روستایی و عشایری خراسان.", PublicationYear = 1984, Edition = 8, Pages = 2836, ReplacementCost = 2200000m, PublisherId = publisherSokhan, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { Id = bookPrince, Isbn = "9789640010042", Title = "شازده احتجاب", Description = "رمانی ساختارمند از هوشنگ گلشیری درباره فروپاشی یک خاندان قاجاری.", PublicationYear = 1969, Edition = 7, Pages = 112, ReplacementCost = 310000m, PublisherId = publisherNiloufar, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { Id = bookGolestan, Isbn = "9789640010059", Title = "گلستان سعدی", Description = "نثر آهنگین و حکایت‌های اخلاقی سعدی شیرازی.", PublicationYear = 1258, Edition = 20, Pages = 360, ReplacementCost = 480000m, PublisherId = publisherAmirkabir, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { Id = bookDivanHafez, Isbn = "9789640010066", Title = "دیوان حافظ", Description = "گزیده غزل‌های حافظ شیرازی با شرح واژگان.", PublicationYear = 1399, Edition = 15, Pages = 640, ReplacementCost = 680000m, PublisherId = publisherAmirkabir, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { Id = bookShahnameh, Isbn = "9789640010073", Title = "شاهنامه فردوسی", Description = "متن برگزیده شاهنامه برای مطالعه عمومی.", PublicationYear = 1010, Edition = 10, Pages = 1216, ReplacementCost = 1350000m, PublisherId = publisherSokhan, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { Id = bookAnotherBirth, Isbn = "9789640010080", Title = "تولدی دیگر", Description = "مجموعه‌ای از شعرهای فروغ فرخزاد در جریان شعر نو فارسی.", PublicationYear = 1964, Edition = 9, Pages = 176, ReplacementCost = 390000m, PublisherId = publisherMarkaz, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<BookAuthor>().HasData(
            new BookAuthor { BookId = bookBlindOwl, AuthorId = authorHedayat, SortOrder = 1 },
            new BookAuthor { BookId = bookSuvashun, AuthorId = authorDaneshvar, SortOrder = 1 },
            new BookAuthor { BookId = bookKelidar, AuthorId = authorDolatabadi, SortOrder = 1 },
            new BookAuthor { BookId = bookPrince, AuthorId = authorGolshiri, SortOrder = 1 },
            new BookAuthor { BookId = bookGolestan, AuthorId = authorSaadi, SortOrder = 1 },
            new BookAuthor { BookId = bookDivanHafez, AuthorId = authorHafez, SortOrder = 1 },
            new BookAuthor { BookId = bookShahnameh, AuthorId = authorFerdowsi, SortOrder = 1 },
            new BookAuthor { BookId = bookAnotherBirth, AuthorId = authorForough, SortOrder = 1 });

        modelBuilder.Entity<BookCategory>().HasData(
            new BookCategory { BookId = bookBlindOwl, CategoryId = categoryNovel },
            new BookCategory { BookId = bookBlindOwl, CategoryId = categoryContemporary },
            new BookCategory { BookId = bookSuvashun, CategoryId = categoryNovel },
            new BookCategory { BookId = bookSuvashun, CategoryId = categoryContemporary },
            new BookCategory { BookId = bookKelidar, CategoryId = categoryNovel },
            new BookCategory { BookId = bookKelidar, CategoryId = categoryContemporary },
            new BookCategory { BookId = bookPrince, CategoryId = categoryNovel },
            new BookCategory { BookId = bookPrince, CategoryId = categoryContemporary },
            new BookCategory { BookId = bookGolestan, CategoryId = categoryClassic },
            new BookCategory { BookId = bookGolestan, CategoryId = categoryShortStory },
            new BookCategory { BookId = bookDivanHafez, CategoryId = categoryClassic },
            new BookCategory { BookId = bookDivanHafez, CategoryId = categoryPoetry },
            new BookCategory { BookId = bookShahnameh, CategoryId = categoryClassic },
            new BookCategory { BookId = bookShahnameh, CategoryId = categoryEpic },
            new BookCategory { BookId = bookAnotherBirth, CategoryId = categoryPoetry },
            new BookCategory { BookId = bookAnotherBirth, CategoryId = categoryContemporary });

        modelBuilder.Entity<BookCopy>().HasData(
            new BookCopy { Id = copyBlindOwl1, BookId = bookBlindOwl, BranchId = branchCentral, Barcode = "IR-BOF-001", Status = BookCopyStatus.Rented, AcquiredOn = new DateOnly(2024, 2, 10), PurchasePrice = 250000m, ShelfLocation = "تهران-A1" },
            new BookCopy { Id = copyBlindOwl2, BookId = bookBlindOwl, BranchId = branchShiraz, Barcode = "IR-BOF-002", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 2, 10), PurchasePrice = 250000m, ShelfLocation = "شیراز-B2" },
            new BookCopy { Id = copySuvashun1, BookId = bookSuvashun, BranchId = branchShiraz, Barcode = "IR-SUV-001", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 3, 5), PurchasePrice = 410000m, ShelfLocation = "شیراز-A3" },
            new BookCopy { Id = copySuvashun2, BookId = bookSuvashun, BranchId = branchCentral, Barcode = "IR-SUV-002", Status = BookCopyStatus.Reserved, AcquiredOn = new DateOnly(2024, 3, 5), PurchasePrice = 410000m, ShelfLocation = "تهران-C1" },
            new BookCopy { Id = copyKelidar1, BookId = bookKelidar, BranchId = branchCentral, Barcode = "IR-KEL-001", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 4, 12), PurchasePrice = 1850000m, ShelfLocation = "تهران-D4" },
            new BookCopy { Id = copyKelidar2, BookId = bookKelidar, BranchId = branchIsfahan, Barcode = "IR-KEL-002", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 4, 12), PurchasePrice = 1850000m, ShelfLocation = "اصفهان-E1" },
            new BookCopy { Id = copyPrince1, BookId = bookPrince, BranchId = branchCentral, Barcode = "IR-SHAZ-001", Status = BookCopyStatus.Rented, AcquiredOn = new DateOnly(2024, 5, 3), PurchasePrice = 230000m, ShelfLocation = "تهران-B1" },
            new BookCopy { Id = copyPrince2, BookId = bookPrince, BranchId = branchIsfahan, Barcode = "IR-SHAZ-002", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 5, 3), PurchasePrice = 230000m, ShelfLocation = "اصفهان-A2" },
            new BookCopy { Id = copyGolestan1, BookId = bookGolestan, BranchId = branchShiraz, Barcode = "IR-GOL-001", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 6, 1), PurchasePrice = 360000m, ShelfLocation = "شیراز-G1" },
            new BookCopy { Id = copyGolestan2, BookId = bookGolestan, BranchId = branchIsfahan, Barcode = "IR-GOL-002", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 6, 1), PurchasePrice = 360000m, ShelfLocation = "اصفهان-C3" },
            new BookCopy { Id = copyHafez1, BookId = bookDivanHafez, BranchId = branchShiraz, Barcode = "IR-HAF-001", Status = BookCopyStatus.Reserved, AcquiredOn = new DateOnly(2024, 7, 8), PurchasePrice = 520000m, ShelfLocation = "شیراز-H1" },
            new BookCopy { Id = copyHafez2, BookId = bookDivanHafez, BranchId = branchCentral, Barcode = "IR-HAF-002", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 7, 8), PurchasePrice = 520000m, ShelfLocation = "تهران-F2" },
            new BookCopy { Id = copyShahnameh1, BookId = bookShahnameh, BranchId = branchCentral, Barcode = "IR-SHN-001", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 8, 20), PurchasePrice = 1100000m, ShelfLocation = "تهران-S1" },
            new BookCopy { Id = copyShahnameh2, BookId = bookShahnameh, BranchId = branchIsfahan, Barcode = "IR-SHN-002", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 8, 20), PurchasePrice = 1100000m, ShelfLocation = "اصفهان-S2" },
            new BookCopy { Id = copyForough1, BookId = bookAnotherBirth, BranchId = branchCentral, Barcode = "IR-FOR-001", Status = BookCopyStatus.Reserved, AcquiredOn = new DateOnly(2024, 9, 14), PurchasePrice = 290000m, ShelfLocation = "تهران-P1" },
            new BookCopy { Id = copyForough2, BookId = bookAnotherBirth, BranchId = branchShiraz, Barcode = "IR-FOR-002", Status = BookCopyStatus.Available, AcquiredOn = new DateOnly(2024, 9, 14), PurchasePrice = 290000m, ShelfLocation = "شیراز-P3" });

        modelBuilder.Entity<Customer>().HasData(
            new { Id = customerMina, FirstName = "مینا", LastName = "احمدی", Email = "mina.ahmadi@example.ir", Phone = "+98-912-100-1001", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1001", Status = CustomerStatus.Active, JoinedOn = new DateOnly(2025, 1, 12) },
            new { Id = customerOmid, FirstName = "امید", LastName = "رحیمی", Email = "omid.rahimi@example.ir", Phone = "+98-912-100-1002", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1002", Status = CustomerStatus.Active, JoinedOn = new DateOnly(2025, 2, 20) },
            new { Id = customerNiloofar, FirstName = "نیلوفر", LastName = "کاظمی", Email = "niloofar.kazemi@example.ir", Phone = "+98-912-100-1003", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1003", Status = CustomerStatus.Active, JoinedOn = new DateOnly(2025, 3, 8) },
            new { Id = customerReza, FirstName = "رضا", LastName = "موسوی", Email = "reza.mousavi@example.ir", Phone = "+98-912-100-1004", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1004", Status = CustomerStatus.Active, JoinedOn = new DateOnly(2025, 4, 18) },
            new { Id = customerSara, FirstName = "سارا", LastName = "نوری", Email = "sara.noori@example.ir", Phone = "+98-912-100-1005", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1005", Status = CustomerStatus.Active, JoinedOn = new DateOnly(2025, 5, 4) },
            new { Id = customerKaveh, FirstName = "کاوه", LastName = "مرادی", Email = "kaveh.moradi@example.ir", Phone = "+98-912-100-1006", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1006", Status = CustomerStatus.Suspended, JoinedOn = new DateOnly(2025, 6, 10) },
            new { Id = customerLeila, FirstName = "لیلا", LastName = "صادقی", Email = "leila.sadeghi@example.ir", Phone = "+98-912-100-1007", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1007", Status = CustomerStatus.Active, JoinedOn = new DateOnly(2025, 7, 22) },
            new { Id = customerArash, FirstName = "آرش", LastName = "کریمی", Email = "arash.karimi@example.ir", Phone = "+98-912-100-1008", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), MembershipNumber = "IR-MBR-1008", Status = CustomerStatus.Active, JoinedOn = new DateOnly(2025, 8, 15) });

        modelBuilder.Entity<Customer>().OwnsOne(x => x.Address).HasData(
            new { CustomerId = customerMina, Street = "خیابان مطهری، پلاک ۱۲", City = "تهران", State = "تهران", PostalCode = "1587654312", Country = "ایران" },
            new { CustomerId = customerOmid, Street = "بلوار کشاورز، پلاک ۴۵", City = "تهران", State = "تهران", PostalCode = "1418845671", Country = "ایران" },
            new { CustomerId = customerNiloofar, Street = "خیابان قصردشت، کوچه سرو", City = "شیراز", State = "فارس", PostalCode = "7188832145", Country = "ایران" },
            new { CustomerId = customerReza, Street = "خیابان چهارباغ بالا، پلاک ۹", City = "اصفهان", State = "اصفهان", PostalCode = "8174678912", Country = "ایران" },
            new { CustomerId = customerSara, Street = "خیابان ولیعصر، کوچه نیلوفر", City = "تهران", State = "تهران", PostalCode = "1967743215", Country = "ایران" },
            new { CustomerId = customerKaveh, Street = "خیابان زند، پلاک ۱۸", City = "شیراز", State = "فارس", PostalCode = "7134889123", Country = "ایران" },
            new { CustomerId = customerLeila, Street = "خیابان آمادگاه، پلاک ۲۲", City = "اصفهان", State = "اصفهان", PostalCode = "8145672341", Country = "ایران" },
            new { CustomerId = customerArash, Street = "خیابان شریعتی، پلاک ۷۰", City = "تهران", State = "تهران", PostalCode = "1934678911", Country = "ایران" });

        modelBuilder.Entity<Staff>().HasData(
            new { Id = staffSara, FirstName = "سارا", LastName = "یوسفی", Email = "sara.yousefi@persian-library.local", Phone = "+98-21-8899-2001", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), EmployeeNumber = "IR-EMP-001", HiredOn = new DateOnly(2023, 4, 1) },
            new { Id = staffMehrdad, FirstName = "مهرداد", LastName = "جعفری", Email = "mehrdad.jafari@persian-library.local", Phone = "+98-71-3222-2002", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), EmployeeNumber = "IR-EMP-002", HiredOn = new DateOnly(2023, 9, 10) },
            new { Id = staffParisa, FirstName = "پریسا", LastName = "حسینی", Email = "parisa.hosseini@persian-library.local", Phone = "+98-31-3666-2003", CreatedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), EmployeeNumber = "IR-EMP-003", HiredOn = new DateOnly(2024, 2, 5) });

        modelBuilder.Entity<Rental>().HasData(
            new Rental { Id = rentalOpen, RentalNumber = "IR-RNT-2026-0001", CustomerId = customerMina, StaffId = staffSara, BranchId = branchCentral, RentedAtUtc = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc), DueAtUtc = new DateTime(2026, 6, 15, 10, 0, 0, DateTimeKind.Utc), Status = RentalStatus.Open },
            new Rental { Id = rentalClosed, RentalNumber = "IR-RNT-2026-0002", CustomerId = customerReza, StaffId = staffParisa, BranchId = branchIsfahan, RentedAtUtc = new DateTime(2026, 5, 10, 9, 0, 0, DateTimeKind.Utc), DueAtUtc = new DateTime(2026, 5, 24, 9, 0, 0, DateTimeKind.Utc), ClosedAtUtc = new DateTime(2026, 5, 20, 16, 0, 0, DateTimeKind.Utc), Status = RentalStatus.Closed });

        modelBuilder.Entity<RentalLine>().HasData(
            new RentalLine { Id = rentalLineOpen, RentalId = rentalOpen, BookCopyId = copyBlindOwl1, DailyLateFee = 25000m, LateFeeCharged = 0m, IsReturned = false },
            new RentalLine { Id = rentalLineClosed, RentalId = rentalClosed, BookCopyId = copyPrince1, DailyLateFee = 25000m, LateFeeCharged = 0m, IsReturned = true, ReturnedAtUtc = new DateTime(2026, 5, 20, 16, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<Reservation>().HasData(
            new Reservation { Id = reservationHafez, CustomerId = customerOmid, BookCopyId = copyHafez1, ReservedAtUtc = new DateTime(2026, 6, 3, 9, 0, 0, DateTimeKind.Utc), ExpiresAtUtc = new DateTime(2026, 6, 10, 9, 0, 0, DateTimeKind.Utc), Status = ReservationStatus.Active },
            new Reservation { Id = reservationForough, CustomerId = customerLeila, BookCopyId = copyForough1, ReservedAtUtc = new DateTime(2026, 6, 4, 11, 0, 0, DateTimeKind.Utc), ExpiresAtUtc = new DateTime(2026, 6, 11, 11, 0, 0, DateTimeKind.Utc), Status = ReservationStatus.Active });

        modelBuilder.Entity<Review>().HasData(
            new Review { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"), BookId = bookBlindOwl, CustomerId = customerOmid, Rating = 5, Comment = "فضاسازی بوف کور هنوز تکان‌دهنده و متفاوت است.", CreatedAtUtc = new DateTime(2026, 6, 4, 12, 0, 0, DateTimeKind.Utc) },
            new Review { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"), BookId = bookSuvashun, CustomerId = customerMina, Rating = 5, Comment = "روایت شیراز و شخصیت‌پردازی زری بسیار ماندگار است.", CreatedAtUtc = new DateTime(2026, 6, 5, 12, 0, 0, DateTimeKind.Utc) },
            new Review { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3"), BookId = bookKelidar, CustomerId = customerNiloofar, Rating = 4, Comment = "طولانی اما پرجزئیات و ارزشمند.", CreatedAtUtc = new DateTime(2026, 6, 6, 12, 0, 0, DateTimeKind.Utc) },
            new Review { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4"), BookId = bookDivanHafez, CustomerId = customerSara, Rating = 5, Comment = "نسخه خوش‌خوان و مناسب برای مراجعه روزانه.", CreatedAtUtc = new DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc) },
            new Review { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5"), BookId = bookShahnameh, CustomerId = customerArash, Rating = 5, Comment = "برای شروع مطالعه شاهنامه انتخاب خوبی است.", CreatedAtUtc = new DateTime(2026, 6, 8, 12, 0, 0, DateTimeKind.Utc) },
            new Review { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6"), BookId = bookAnotherBirth, CustomerId = customerLeila, Rating = 4, Comment = "شعرها زنده، مستقیم و تاثیرگذارند.", CreatedAtUtc = new DateTime(2026, 6, 9, 12, 0, 0, DateTimeKind.Utc) });
    }
}
