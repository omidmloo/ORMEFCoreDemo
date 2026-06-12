namespace SampleAPI.Entities;

public class BookCopy
{
    public Guid Id { get; set; }
    public string Barcode { get; set; } = null!;
    public BookCopyStatus Status { get; set; } = BookCopyStatus.Available;
    public DateOnly AcquiredOn { get; set; }
    public decimal PurchasePrice { get; set; }
    public string ShelfLocation { get; set; } = null!;
    public Guid BookId { get; set; }
    public Guid BranchId { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Book Book { get; set; } = null!;
    public LibraryBranch Branch { get; set; } = null!;
    public ICollection<RentalLine> RentalLines { get; set; } = new List<RentalLine>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
