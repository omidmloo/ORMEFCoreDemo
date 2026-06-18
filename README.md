# EF Core Library API Showcase

ASP.NET Core Web API that models a Persian library system and demonstrates EF Core with SQL Server.

## Diagrams

### Domain Class Diagram

![Domain class diagram](docs/class-diagram.svg)

[PlantUML source](docs/class-diagram.puml)

### Database Diagram

![Database diagram](docs/db-diagram.svg)

[PlantUML source](docs/db-diagram.puml)

## Stack

- .NET 10
- ASP.NET Core controllers
- EF Core 10 with SQL Server
- EF Core migrations and seed data
- Swagger UI / OpenAPI
- Mapster DTO mapping

## Run

Update the `LibraryDb` connection string in `appsettings.json` for your SQL Server instance. The checked-in default points to SQL Server on `127.0.0.1,14331`:

```json
"LibraryDb": "Server=127.0.0.1,14331;Initial Catalog=LibraryDB;User ID=sa;Password=PassTest1!@#;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True"
```

Then restore and run the API:

```powershell
dotnet restore
dotnet run
```

On startup, `Database.Migrate()` applies the included migration and seed data.

With the included launch profile, the API runs at:

```text
http://localhost:5227
https://localhost:7277
```

Swagger UI is served from the application root.
OpenAPI JSON is available at `/openapi/v1.json`.

The seed data includes Persian library content: 3 Iranian branches, 8 Persian books, 8 customers, 3 staff members, 8 authors, 4 publishers, 6 categories, 16 book copies, rentals, reservations, and reviews.

## Main Endpoints

- `GET /api/books`
- `GET /api/books/{id}`
- `POST /api/books`
- `PUT /api/books/{id}`
- `DELETE /api/books/{id}` soft deletes a book
- `POST /api/books/copies`
- `PATCH /api/books/copies/{copyId}/status`
- `POST /api/books/{bookId}/reviews`
- `GET /api/customers`
- `GET /api/customers/{id}`
- `POST /api/customers`
- `PUT /api/customers/{id}`
- `GET /api/customers/{id}/rentals`
- `GET /api/rentals`
- `GET /api/rentals/{id}`
- `POST /api/rentals`
- `POST /api/rentals/{id}/return`
- `GET /api/reservations`
- `POST /api/reservations`
- `POST /api/reservations/{id}/cancel`
- `GET /api/lookups/authors`
- `POST /api/lookups/authors`
- `GET /api/lookups/categories`
- `POST /api/lookups/categories`
- `GET /api/lookups/publishers`
- `POST /api/lookups/publishers`
- `GET /api/lookups/branches`
- `GET /api/lookups/staff`

## EF Core Features Demonstrated

- SQL Server provider and migrations
- Fluent API configuration
- Seed data with `HasData`
- One-to-many relationships
- Many-to-many relationships through join entities with payload
- Owned entity types for addresses
- Table-per-hierarchy inheritance for library users
- Enum-to-string value conversions
- Global query filters for soft-deleted books
- Matching filters for dependent entities
- Unique and composite indexes
- Check constraints
- Computed columns
- Decimal precision configuration
- Rowversion concurrency tokens
- Restrictive delete behavior
- Projection queries with `AsNoTracking`
- `Include` / `ThenInclude` workflow updates
- `ExecuteDeleteAsync` for set-based join cleanup
- Execution strategy and explicit transaction in rental creation
