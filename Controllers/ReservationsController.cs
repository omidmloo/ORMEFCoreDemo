using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Context;
using SampleAPI.Dtos;
using SampleAPI.Entities;

namespace SampleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(LiberaryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ReservationDto>>> GetReservations(ReservationStatus? status, CancellationToken cancellationToken)
    {
        var query = db.Reservations.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return Ok(await ProjectReservations(query).ToListAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        if (!await db.Customers.AnyAsync(x => x.Id == request.CustomerId && x.Status == CustomerStatus.Active, cancellationToken))
        {
            return BadRequest("Active customer does not exist.");
        }

        var copy = await db.BookCopies.FirstOrDefaultAsync(x => x.Id == request.BookCopyId, cancellationToken);
        if (copy is null)
        {
            return BadRequest("Book copy does not exist.");
        }

        if (copy.Status != BookCopyStatus.Available)
        {
            return BadRequest("Only available copies can be reserved.");
        }

        copy.Status = BookCopyStatus.Reserved;
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            BookCopyId = request.BookCopyId,
            ReservedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = request.ExpiresAtUtc,
            Status = ReservationStatus.Active
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetReservations), null, await ProjectReservations(db.Reservations.AsNoTracking().Where(x => x.Id == reservation.Id)).FirstAsync(cancellationToken));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelReservation(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await db.Reservations
            .Include(x => x.BookCopy)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (reservation is null)
        {
            return NotFound();
        }

        if (reservation.Status != ReservationStatus.Active)
        {
            return BadRequest("Only active reservations can be cancelled.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        reservation.BookCopy.Status = BookCopyStatus.Available;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static IQueryable<ReservationDto> ProjectReservations(IQueryable<Reservation> query) =>
        query.OrderByDescending(x => x.ReservedAtUtc)
            .Select(x => new ReservationDto(
                x.Id,
                x.CustomerId,
                x.Customer.FirstName + " " + x.Customer.LastName,
                x.BookCopyId,
                x.BookCopy.Book.Title,
                x.ReservedAtUtc,
                x.ExpiresAtUtc,
                x.Status));
}
