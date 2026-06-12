using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Context;
using SampleAPI.Dtos;
using SampleAPI.Entities;

namespace SampleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController(LiberaryDbContext db) : ControllerBase
{
    [HttpGet("authors")]
    public async Task<IReadOnlyCollection<AuthorDto>> GetAuthors(CancellationToken cancellationToken) =>
        await db.Authors.AsNoTracking()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new AuthorDto(x.Id, x.FirstName, x.LastName, x.BirthDate))
            .ToListAsync(cancellationToken);

    [HttpPost("authors")]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorRequest request, CancellationToken cancellationToken)
    {
        var author = new Author { Id = Guid.NewGuid(), FirstName = request.FirstName, LastName = request.LastName, BirthDate = request.BirthDate, Biography = request.Biography };
        db.Authors.Add(author);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetAuthors), null, new AuthorDto(author.Id, author.FirstName, author.LastName, author.BirthDate));
    }

    [HttpGet("categories")]
    public async Task<IReadOnlyCollection<CategoryDto>> GetCategories(CancellationToken cancellationToken) =>
        await db.Categories.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Description))
            .ToListAsync(cancellationToken);

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateLookupRequest request, CancellationToken cancellationToken)
    {
        var category = new Category { Id = Guid.NewGuid(), Name = request.Name, Description = request.Description };
        db.Categories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetCategories), null, new CategoryDto(category.Id, category.Name, category.Description));
    }

    [HttpGet("publishers")]
    public async Task<IReadOnlyCollection<PublisherDto>> GetPublishers(CancellationToken cancellationToken) =>
        await db.Publishers.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new PublisherDto(x.Id, x.Name, x.Website))
            .ToListAsync(cancellationToken);

    [HttpPost("publishers")]
    public async Task<ActionResult<PublisherDto>> CreatePublisher(CreatePublisherRequest request, CancellationToken cancellationToken)
    {
        var publisher = new Publisher { Id = Guid.NewGuid(), Name = request.Name, Website = request.Website };
        db.Publishers.Add(publisher);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetPublishers), null, new PublisherDto(publisher.Id, publisher.Name, publisher.Website));
    }

    [HttpGet("branches")]
    public async Task<IReadOnlyCollection<BranchDto>> GetBranches(CancellationToken cancellationToken) =>
        await db.Branches.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new BranchDto(x.Id, x.Name, x.Phone, new AddressDto(x.Address.Street, x.Address.City, x.Address.State, x.Address.PostalCode, x.Address.Country)))
            .ToListAsync(cancellationToken);

    [HttpGet("staff")]
    public async Task<IReadOnlyCollection<StaffDto>> GetStaff(CancellationToken cancellationToken) =>
        await db.Staff.AsNoTracking()
            .OrderBy(x => x.LastName)
            .Select(x => new StaffDto(x.Id, x.FirstName, x.LastName, x.Email, x.EmployeeNumber))
            .ToListAsync(cancellationToken);
}
