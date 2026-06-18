namespace SampleAPI.Entities;

public class Book
{
    public Guid Id { get; set; }
    public int TotalSales { get; set; }
    public string Isbn { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int PublicationYear { get; set; }
    public int Edition { get; set; } = 1;
    public int Pages { get; set; }
    public decimal ReplacementCost { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid PublisherId { get; set; }

    public Publisher Publisher { get; set; } = null!;
    public ICollection<BookAuthor> Authors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();
    public ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
