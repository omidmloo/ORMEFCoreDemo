namespace SampleAPI.Entities;

public class Staff : LibraryUser
{
    public string EmployeeNumber { get; set; } = null!;
    public DateOnly HiredOn { get; set; }

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
