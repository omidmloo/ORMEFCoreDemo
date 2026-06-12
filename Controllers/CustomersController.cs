using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleAPI.Context;
using SampleAPI.Dtos;
using SampleAPI.Entities;

namespace SampleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(LiberaryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CustomerDto>>> GetCustomers(string? search, CancellationToken cancellationToken)
    {
        var query = db.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FirstName.Contains(search) ||
                x.LastName.Contains(search) ||
                x.Email.Contains(search) ||
                x.MembershipNumber.Contains(search));
        }

        return Ok(await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new CustomerDto(
                x.Id,
                x.MembershipNumber,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Phone,
                x.Status,
                x.JoinedOn,
                new AddressDto(x.Address.Street, x.Address.City, x.Address.State, x.Address.PostalCode, x.Address.Country)))
            .ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(Guid id, CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CustomerDto(
                x.Id,
                x.MembershipNumber,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Phone,
                x.Status,
                x.JoinedOn,
                new AddressDto(x.Address.Street, x.Address.City, x.Address.State, x.Address.PostalCode, x.Address.Country)))
            .FirstOrDefaultAsync(cancellationToken);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        if (await db.Customers.AnyAsync(x => x.MembershipNumber == request.MembershipNumber || x.Email == request.Email, cancellationToken))
        {
            return Conflict("A customer with this membership number or email already exists.");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            MembershipNumber = request.MembershipNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            JoinedOn = request.JoinedOn,
            CreatedAtUtc = DateTime.UtcNow,
            Address = FromDto(request.Address)
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, ToDto(customer));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (customer is null)
        {
            return NotFound();
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Status = request.Status;
        customer.Address = FromDto(request.Address);

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/rentals")]
    public async Task<ActionResult<IReadOnlyCollection<RentalDto>>> GetCustomerRentals(Guid id, CancellationToken cancellationToken)
    {
        if (!await db.Customers.AnyAsync(x => x.Id == id, cancellationToken))
        {
            return NotFound();
        }

        return Ok(await RentalProjection(db.Rentals.AsNoTracking().Where(x => x.CustomerId == id))
            .ToListAsync(cancellationToken));
    }

    private static CustomerDto ToDto(Customer customer) =>
        new(
            customer.Id,
            customer.MembershipNumber,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Phone,
            customer.Status,
            customer.JoinedOn,
            new AddressDto(customer.Address.Street, customer.Address.City, customer.Address.State, customer.Address.PostalCode, customer.Address.Country));

    private static Address FromDto(AddressDto dto) =>
        new()
        {
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            Country = dto.Country
        };

    private static IQueryable<RentalDto> RentalProjection(IQueryable<Rental> query) =>
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
                x.Lines.Select(l => new RentalLineDto(l.Id, l.BookCopyId, l.BookCopy.Barcode, l.BookCopy.Book.Title, l.IsReturned, l.ReturnedAtUtc, l.LateFeeCharged)).ToList()));
}
