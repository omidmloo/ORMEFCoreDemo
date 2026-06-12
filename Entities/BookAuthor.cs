namespace SampleAPI.Entities;

public class BookAuthor
{
    public Guid BookId { get; set; }
    public Guid AuthorId { get; set; }
    public int SortOrder { get; set; }

    public Book Book { get; set; } = null!;
    public Author Author { get; set; } = null!;
}
