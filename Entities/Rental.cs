namespace SampleAPI.Entities;

public class Rental
{
    public Guid Id { get; set; }
    public string RentalNumber { get; set; } = null!;
    public DateTime RentedAtUtc { get; set; }
    public DateTime DueAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Open;
    public Guid CustomerId { get; set; }
    public Guid StaffId { get; set; }
    public Guid BranchId { get; set; }

    public Customer Customer { get; set; } = null!;
    public Staff Staff { get; set; } = null!;
    public LibraryBranch Branch { get; set; } = null!;
    public ICollection<RentalLine> Lines { get; set; } = new List<RentalLine>();
}
