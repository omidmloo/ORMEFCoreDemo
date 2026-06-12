namespace SampleAPI.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BookCopyId { get; set; }
    public DateTime ReservedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;

    public Customer Customer { get; set; } = null!;
    public BookCopy BookCopy { get; set; } = null!;
}
