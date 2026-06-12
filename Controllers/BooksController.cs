using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Context;
using SampleAPI.Dtos;
using SampleAPI.Entities;

namespace SampleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(LiberaryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<BookSummaryDto>>> GetBooks(
        string? search,
        Guid? authorId,
        Guid? categoryId,
        bool onlyAvailable = false,
        CancellationToken cancellationToken = default)
    {
        var query = db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Title.Contains(search) || x.Isbn.Contains(search));
        }

        if (authorId.HasValue)
        {
            query = query.Where(x => x.Authors.Any(a => a.AuthorId == authorId.Value));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.Categories.Any(c => c.CategoryId == categoryId.Value));
        }

        if (onlyAvailable)
        {
            query = query.Where(x => x.Copies.Any(c => c.Status == BookCopyStatus.Available));
        }

        var books = await query
            .OrderBy(x => x.Title)
            .Select(x => new BookSummaryDto(
                x.Id,
                x.Isbn,
                x.Title,
                x.PublicationYear,
                x.Publisher.Name,
                x.Copies.Count,
                x.Copies.Count(c => c.Status == BookCopyStatus.Available)))
            .ToListAsync(cancellationToken);

        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDetailDto>> GetBook(Guid id, CancellationToken cancellationToken)
    {
        var book = await db.Books
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new BookDetailDto(
                x.Id,
                x.Isbn,
                x.Title,
                x.Description,
                x.PublicationYear,
                x.Edition,
                x.Pages,
                x.ReplacementCost,
                new PublisherDto(x.Publisher.Id, x.Publisher.Name, x.Publisher.Website),
                x.Authors
                    .OrderBy(a => a.SortOrder)
                    .Select(a => new AuthorDto(a.Author.Id, a.Author.FirstName, a.Author.LastName, a.Author.BirthDate))
                    .ToList(),
                x.Categories
                    .OrderBy(c => c.Category.Name)
                    .Select(c => new CategoryDto(c.Category.Id, c.Category.Name, c.Category.Description))
                    .ToList(),
                x.Copies
                    .OrderBy(c => c.Barcode)
                    .Select(c => new BookCopyDto(c.Id, c.Barcode, c.Status, c.ShelfLocation, c.BranchId, c.Branch.Name))
                    .ToList(),
                x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0))
            .FirstOrDefaultAsync(cancellationToken);
        return book is null ? NotFound() : Ok(book);
         
    }

    [HttpPost]
    public async Task<ActionResult<BookDetailDto>> CreateBook(CreateBookRequest request, CancellationToken cancellationToken)
    {
        if (!await db.Publishers.AnyAsync(x => x.Id == request.PublisherId, cancellationToken))
        {
            return BadRequest("Publisher does not exist.");
        }

        if (await db.Books.IgnoreQueryFilters().AnyAsync(x => x.Isbn == request.Isbn, cancellationToken))
        {
            return Conflict("A book with this ISBN already exists.");
        }

        var authorIds = request.AuthorIds.Distinct().ToList();
        var categoryIds = request.CategoryIds.Distinct().ToList();

        if (await db.Authors.CountAsync(x => authorIds.Contains(x.Id), cancellationToken) != authorIds.Count)
        {
            return BadRequest("One or more authors do not exist.");
        }

        if (await db.Categories.CountAsync(x => categoryIds.Contains(x.Id), cancellationToken) != categoryIds.Count)
        {
            return BadRequest("One or more categories do not exist.");
        }

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Isbn = request.Isbn,
            Title = request.Title,
            Description = request.Description,
            PublicationYear = request.PublicationYear,
            Edition = request.Edition,
            Pages = request.Pages,
            ReplacementCost = request.ReplacementCost,
            PublisherId = request.PublisherId,
            CreatedAtUtc = DateTime.UtcNow,
            Authors = authorIds.Select((id, index) => new BookAuthor { AuthorId = id, SortOrder = index + 1 }).ToList(),
            Categories = categoryIds.Select(id => new BookCategory { CategoryId = id }).ToList()
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);

        var created = await ProjectBookDetails(db.Books.AsNoTracking().Where(x => x.Id == book.Id))
            .FirstAsync(cancellationToken);

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBook(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var book = await db.Books.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        if (!await db.Publishers.AnyAsync(x => x.Id == request.PublisherId, cancellationToken))
        {
            return BadRequest("Publisher does not exist.");
        }

        var authorIds = request.AuthorIds.Distinct().ToList();
        var categoryIds = request.CategoryIds.Distinct().ToList();

        book.Title = request.Title;
        book.Description = request.Description;
        book.PublicationYear = request.PublicationYear;
        book.Edition = request.Edition;
        book.Pages = request.Pages;
        book.ReplacementCost = request.ReplacementCost;
        book.PublisherId = request.PublisherId;
        book.UpdatedAtUtc = DateTime.UtcNow;

        await db.Set<BookAuthor>().Where(x => x.BookId == id).ExecuteDeleteAsync(cancellationToken);
        await db.Set<BookCategory>().Where(x => x.BookId == id).ExecuteDeleteAsync(cancellationToken);
        db.Set<BookAuthor>().AddRange(authorIds.Select((authorId, index) => new BookAuthor { BookId = id, AuthorId = authorId, SortOrder = index + 1 }));
        db.Set<BookCategory>().AddRange(categoryIds.Select(categoryId => new BookCategory { BookId = id, CategoryId = categoryId }));

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBook(Guid id, CancellationToken cancellationToken)
    {
        var book = await db.Books.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        book.IsDeleted = true;
        book.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("copies")]
    public async Task<ActionResult<BookCopyDto>> AddCopy(CreateCopyRequest request, CancellationToken cancellationToken)
    {
        if (!await db.Books.AnyAsync(x => x.Id == request.BookId, cancellationToken))
        {
            return BadRequest("Book does not exist.");
        }

        if (!await db.Branches.AnyAsync(x => x.Id == request.BranchId, cancellationToken))
        {
            return BadRequest("Branch does not exist.");
        }

        if (await db.BookCopies.AnyAsync(x => x.Barcode == request.Barcode, cancellationToken))
        {
            return Conflict("A copy with this barcode already exists.");
        }

        var copy = new BookCopy
        {
            Id = Guid.NewGuid(),
            BookId = request.BookId,
            BranchId = request.BranchId,
            Barcode = request.Barcode,
            AcquiredOn = request.AcquiredOn,
            PurchasePrice = request.PurchasePrice,
            ShelfLocation = request.ShelfLocation
        };

        db.BookCopies.Add(copy);
        await db.SaveChangesAsync(cancellationToken);

        var dto = await db.BookCopies
            .AsNoTracking()
            .Where(x => x.Id == copy.Id)
            .Select(x => new BookCopyDto(x.Id, x.Barcode, x.Status, x.ShelfLocation, x.BranchId, x.Branch.Name))
            .FirstAsync(cancellationToken);

        return CreatedAtAction(nameof(GetBook), new { id = request.BookId }, dto);
    }

    [HttpPatch("copies/{copyId:guid}/status")]
    public async Task<IActionResult> UpdateCopyStatus(Guid copyId, UpdateCopyStatusRequest request, CancellationToken cancellationToken)
    {
        var copy = await db.BookCopies.FirstOrDefaultAsync(x => x.Id == copyId, cancellationToken);
        if (copy is null)
        {
            return NotFound();
        }

        copy.Status = request.Status;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{bookId:guid}/reviews")]
    public async Task<ActionResult<ReviewDto>> AddReview(Guid bookId, CreateReviewRequest request, CancellationToken cancellationToken)
    {
        if (request.Rating is < 1 or > 5)
        {
            return BadRequest("Rating must be between 1 and 5.");
        }

        if (!await db.Books.AnyAsync(x => x.Id == bookId, cancellationToken))
        {
            return NotFound();
        }

        if (!await db.Customers.AnyAsync(x => x.Id == request.CustomerId, cancellationToken))
        {
            return BadRequest("Customer does not exist.");
        }

        if (await db.Reviews.AnyAsync(x => x.BookId == bookId && x.CustomerId == request.CustomerId, cancellationToken))
        {
            return Conflict("Customer already reviewed this book.");
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            CustomerId = request.CustomerId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync(cancellationToken);

        var dto = await db.Reviews.AsNoTracking()
            .Where(x => x.Id == review.Id)
            .Select(x => new ReviewDto(x.Id, x.BookId, x.Book.Title, x.CustomerId, x.Customer.FirstName + " " + x.Customer.LastName, x.Rating, x.Comment, x.CreatedAtUtc))
            .FirstAsync(cancellationToken);

        return CreatedAtAction(nameof(GetBook), new { id = bookId }, dto);
    }

    private static IQueryable<BookDetailDto> ProjectBookDetails(IQueryable<Book> query) =>
        query.Select(x => new BookDetailDto(
            x.Id,
            x.Isbn,
            x.Title,
            x.Description,
            x.PublicationYear,
            x.Edition,
            x.Pages,
            x.ReplacementCost,
            new PublisherDto(x.Publisher.Id, x.Publisher.Name, x.Publisher.Website),
            x.Authors
                .OrderBy(a => a.SortOrder)
                .Select(a => new AuthorDto(a.Author.Id, a.Author.FirstName, a.Author.LastName, a.Author.BirthDate))
                .ToList(),
            x.Categories
                .OrderBy(c => c.Category.Name)
                .Select(c => new CategoryDto(c.Category.Id, c.Category.Name, c.Category.Description))
                .ToList(),
            x.Copies
                .OrderBy(c => c.Barcode)
                .Select(c => new BookCopyDto(c.Id, c.Barcode, c.Status, c.ShelfLocation, c.BranchId, c.Branch.Name))
                .ToList(),
            x.Reviews.Any() ? x.Reviews.Average(r => r.Rating) : 0));
}
