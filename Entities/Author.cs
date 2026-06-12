namespace SampleAPI.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? NickName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Biography { get; set; }

    public ICollection<BookAuthor> Books { get; set; } = new List<BookAuthor>();
}
