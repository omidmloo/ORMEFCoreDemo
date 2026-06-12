namespace SampleAPI.Entities;

public class Customer : LibraryUser
{
    public string MembershipNumber { get; set; } = null!;
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public DateOnly JoinedOn { get; set; }
    public Address Address { get; set; } = new();

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
