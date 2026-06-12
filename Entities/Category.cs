namespace SampleAPI.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<BookCategory> Books { get; set; } = new List<BookCategory>();
}
