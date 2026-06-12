using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Context;
using SampleAPI.Dtos;
using SampleAPI.Entities;

namespace SampleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController(LiberaryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RentalDto>>> GetRentals(RentalStatus? status, CancellationToken cancellationToken)
    {
        var query = db.Rentals.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return Ok(await ProjectRentals(query).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RentalDto>> GetRental(Guid id, CancellationToken cancellationToken)
    {
        var rental = await ProjectRentals(db.Rentals.AsNoTracking().Where(x => x.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

        return rental is null ? NotFound() : Ok(rental);
    }

    [HttpPost]
    public async Task<ActionResult<RentalDto>> CreateRental(CreateRentalRequest request, CancellationToken cancellationToken)
    {
        if (!request.BookCopyIds.Any())
        {
            return BadRequest("At least one copy is required.");
        }

        if (request.DueAtUtc <= DateTime.UtcNow)
        {
            return BadRequest("Due date must be in the future.");
        }

        var strategy = db.Database.CreateExecutionStrategy();
        Rental? rental = null;

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

                var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
                if (customer is null || customer.Status != CustomerStatus.Active)
                {
                    throw new InvalidOperationException("Customer does not exist or is not active.");
                }

                if (!await db.Staff.AnyAsync(x => x.Id == request.StaffId, cancellationToken))
                {
                    throw new InvalidOperationException("Staff member does not exist.");
                }

                if (!await db.Branches.AnyAsync(x => x.Id == request.BranchId, cancellationToken))
                {
                    throw new InvalidOperationException("Branch does not exist.");
                }

                var copyIds = request.BookCopyIds.Distinct().ToList();
                var copies = await db.BookCopies
                    .Where(x => copyIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

                if (copies.Count != copyIds.Count)
                {
                    throw new InvalidOperationException("One or more copies do not exist.");
                }

                if (copies.Any(x => x.Status != BookCopyStatus.Available))
                {
                    throw new InvalidOperationException("All copies must be available before rental.");
                }

                foreach (var copy in copies)
                {
                    copy.Status = BookCopyStatus.Rented;
                }

                rental = new Rental
                {
                    Id = Guid.NewGuid(),
                    RentalNumber = $"RNT-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    CustomerId = request.CustomerId,
                    StaffId = request.StaffId,
                    BranchId = request.BranchId,
                    RentedAtUtc = DateTime.UtcNow,
                    DueAtUtc = request.DueAtUtc,
                    Status = RentalStatus.Open,
                    Lines = copies.Select(copy => new RentalLine
                    {
                        Id = Guid.NewGuid(),
                        BookCopyId = copy.Id,
                        DailyLateFee = 0.50m
                    }).ToList()
                };

                db.Rentals.Add(rental);
                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return CreatedAtAction(nameof(GetRental), new { id = rental!.Id }, await ProjectRentals(db.Rentals.AsNoTracking().Where(x => x.Id == rental.Id)).FirstAsync(cancellationToken));
    }

    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<RentalDto>> ReturnRental(Guid id, ReturnRentalRequest request, CancellationToken cancellationToken)
    {
        var rental = await db.Rentals
            .Include(x => x.Lines)
            .ThenInclude(x => x.BookCopy)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (rental is null)
        {
            return NotFound();
        }

        if (rental.Status is RentalStatus.Closed or RentalStatus.Cancelled)
        {
            return BadRequest("Rental is already closed or cancelled.");
        }

        var returnIds = request.BookCopyIds?.Distinct().ToHashSet()
            ?? rental.Lines.Where(x => !x.IsReturned).Select(x => x.BookCopyId).ToHashSet();

        foreach (var line in rental.Lines.Where(x => returnIds.Contains(x.BookCopyId) && !x.IsReturned))
        {
            line.IsReturned = true;
            line.ReturnedAtUtc = DateTime.UtcNow;
            line.LateFeeCharged = CalculateLateFee(rental.DueAtUtc, line.ReturnedAtUtc.Value, line.DailyLateFee);
            line.BookCopy.Status = BookCopyStatus.Available;
        }

        if (rental.Lines.All(x => x.IsReturned))
        {
            rental.Status = RentalStatus.Closed;
            rental.ClosedAtUtc = DateTime.UtcNow;
        }
        else if (rental.DueAtUtc < DateTime.UtcNow)
        {
            rental.Status = RentalStatus.Overdue;
        }

        await db.SaveChangesAsync(cancellationToken);

        return Ok(await ProjectRentals(db.Rentals.AsNoTracking().Where(x => x.Id == id)).FirstAsync(cancellationToken));
    }

    private static decimal CalculateLateFee(DateTime dueAtUtc, DateTime returnedAtUtc, decimal dailyFee)
    {
        if (returnedAtUtc <= dueAtUtc)
        {
            return 0m;
        }

        return Math.Ceiling((decimal)(returnedAtUtc - dueAtUtc).TotalDays) * dailyFee;
    }

    private static IQueryable<RentalDto> ProjectRentals(IQueryable<Rental> query) =>
        query.OrderByDescending(x => x.RentedAtUtc)
            .Select(x => new RentalDto(
                x.Id,
                x.RentalNumber,
                x.RentedAtUtc,
                x.DueAtUtc,
                x.ClosedAtUtc,
                x.Status,
                x.Customer.FirstName + " " + x.Customer.LastName,
                x.Staff.FirstName + " " + x.Staff.LastName,
                x.Branch.Name,
                x.Lines
                    .OrderBy(l => l.BookCopy.Barcode)
                    .Select(l => new RentalLineDto(l.Id, l.BookCopyId, l.BookCopy.Barcode, l.BookCopy.Book.Title, l.IsReturned, l.ReturnedAtUtc, l.LateFeeCharged))
                    .ToList()));
}
