namespace SampleAPI.Entities;

public class Review
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid CustomerId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Book Book { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}
