using SampleAPI.Entities;

namespace SampleAPI.Dtos;

public record AddressDto(string Street, string City, string State, string PostalCode, string Country);

public record AuthorDto(Guid Id, string FirstName, string LastName, DateOnly? BirthDate);
public record CategoryDto(Guid Id, string Name, string? Description);
public record PublisherDto(Guid Id, string Name, string? Website);
public record BranchDto(Guid Id, string Name, string Phone, AddressDto Address);
public record StaffDto(Guid Id, string FirstName, string LastName, string Email, string EmployeeNumber);

public record BookSummaryDto(
    Guid Id,
    string Isbn,
    string Title,
    int PublicationYear,
    string Publisher,
    int TotalCopies,
    int AvailableCopies);

public record BookDetailDto(
    Guid Id,
    string Isbn,
    string Title,
    string? Description,
    int PublicationYear,
    int Edition,
    int Pages,
    decimal ReplacementCost,
    PublisherDto Publisher,
    IReadOnlyCollection<AuthorDto> Authors,
    IReadOnlyCollection<CategoryDto> Categories,
    IReadOnlyCollection<BookCopyDto> Copies,
    double AverageRating);

public record BookCopyDto(
    Guid Id,
    string Barcode,
    BookCopyStatus Status,
    string ShelfLocation,
    Guid BranchId,
    string BranchName);

public record CreateBookRequest(
    string Isbn,
    string Title,
    string? Description,
    int PublicationYear,
    int Edition,
    int Pages,
    decimal ReplacementCost,
    Guid PublisherId,
    IReadOnlyCollection<Guid> AuthorIds,
    IReadOnlyCollection<Guid> CategoryIds);

public record UpdateBookRequest(
    string Title,
    string? Description,
    int PublicationYear,
    int Edition,
    int Pages,
    decimal ReplacementCost,
    Guid PublisherId,
    IReadOnlyCollection<Guid> AuthorIds,
    IReadOnlyCollection<Guid> CategoryIds);

public record CreateCopyRequest(Guid BookId, Guid BranchId, string Barcode, DateOnly AcquiredOn, decimal PurchasePrice, string ShelfLocation);
public record UpdateCopyStatusRequest(BookCopyStatus Status);

public record CustomerDto(
    Guid Id,
    string MembershipNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    CustomerStatus Status,
    DateOnly JoinedOn,
    AddressDto Address);

public record CreateCustomerRequest(
    string MembershipNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateOnly JoinedOn,
    AddressDto Address);

public record UpdateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    CustomerStatus Status,
    AddressDto Address);

public record CreateReservationRequest(Guid CustomerId, Guid BookCopyId, DateTime ExpiresAtUtc);
public record ReservationDto(Guid Id, Guid CustomerId, string CustomerName, Guid BookCopyId, string BookTitle, DateTime ReservedAtUtc, DateTime ExpiresAtUtc, ReservationStatus Status);

public record CreateRentalRequest(Guid CustomerId, Guid StaffId, Guid BranchId, DateTime DueAtUtc, IReadOnlyCollection<Guid> BookCopyIds);
public record ReturnRentalRequest(IReadOnlyCollection<Guid>? BookCopyIds);

public record RentalDto(
    Guid Id,
    string RentalNumber,
    DateTime RentedAtUtc,
    DateTime DueAtUtc,
    DateTime? ClosedAtUtc,
    RentalStatus Status,
    string CustomerName,
    string StaffName,
    string BranchName,
    IReadOnlyCollection<RentalLineDto> Lines);

public record RentalLineDto(Guid Id, Guid BookCopyId, string Barcode, string BookTitle, bool IsReturned, DateTime? ReturnedAtUtc, decimal LateFeeCharged);

public record CreateReviewRequest(Guid CustomerId, int Rating, string? Comment);
public record ReviewDto(Guid Id, Guid BookId, string BookTitle, Guid CustomerId, string CustomerName, int Rating, string? Comment, DateTime CreatedAtUtc);

public record CreateLookupRequest(string Name, string? Description);
public record CreatePublisherRequest(string Name, string? Website);
public record CreateAuthorRequest(string FirstName, string LastName, DateOnly? BirthDate, string? Biography);
