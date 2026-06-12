namespace SampleAPI.Entities;

public class RentalLine
{
    public Guid Id { get; set; }
    public Guid RentalId { get; set; }
    public Guid BookCopyId { get; set; }
    public DateTime? ReturnedAtUtc { get; set; }
    public decimal DailyLateFee { get; set; }
    public decimal LateFeeCharged { get; set; }
    public bool IsReturned { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Rental Rental { get; set; } = null!;
    public BookCopy BookCopy { get; set; } = null!;
}
