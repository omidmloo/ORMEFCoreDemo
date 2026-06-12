namespace SampleAPI.Entities;

public class LibraryBranch
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public Address Address { get; set; } = new();

    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
