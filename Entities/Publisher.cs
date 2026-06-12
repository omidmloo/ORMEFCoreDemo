namespace SampleAPI.Entities;

public class Publisher
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Website { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
