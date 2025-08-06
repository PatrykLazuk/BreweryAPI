# Brewery API (.NET 9)

A scalable, production-ready RESTful API for querying brewery information, designed for high code quality, maintainability, and extensibility.

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Authentication](#authentication)
- [API Usage](#api-usage)
- [API Versioning](#api-versioning)
- [Error Handling & Logging](#error-handling--logging)
- [Design Decisions](#design-decisions)
- [Testing & Extensibility](#testing--extensibility)
- [Contact](#contact)

---

## Project Overview

This API was built as part of a recruitment assignment for a senior .NET developer role.  
It integrates with [OpenBreweryDB](https://www.openbrewerydb.org/documentation), stores data in SQLite via Entity Framework Core, and exposes well-structured endpoints for brewery lookup, search, sort, and autocomplete.

The project adheres to SOLID principles, is extensible, testable, and ready for production scenarios, including logging, error handling, and JWT-based authentication.

---

## Features

**Required:**

- Query breweries by name, city, or phone
- Sort by name, city, or distance (optionally by user location)
- Search functionality with in-memory caching
- Dependency injection and abstraction via interfaces
- 10-minute cache for data fetched from OpenBreweryDB
- Consistent error handling

**Bonus:**

- Autocomplete endpoint for search
- API Versioning (`v1`, `v2` with DTO extension example)
- Structured logging with Serilog (console, easy to extend to files/cloud)
- SQLite + Entity Framework Core as persistent backend
- JWT authentication for securing endpoints

---

## Project Structure

```
BreweryAPI/
│
├── Controllers/
│   ├── BreweriesController.cs
│   └── AuthController.cs
│
├── Data/
│   ├── BreweryDbContext.cs
│   └── DbSeeder.cs
│
├── Infrastructure/
│   └── ErrorHandlingMiddleware.cs
│
├── Logic/
│   ├── BreweryLogic.cs
│   └── Interfaces/IBreweryLogic.cs
│
├── Models/
│   ├── Brewery.cs
│   ├── BreweryAutocomplete.cs
│   ├── BreweryV2Dto.cs
│   ├── PagedResult.cs
│   └── BreweryLoginRequest.cs
│
├── Repositories/
│   ├── BreweryEfRepository.cs
│   ├── BreweryApiRepository.cs
│   └── Interfaces/IBreweryRepository.cs
│
├── StartupConfiguration/
│   ├── DependencyInjectionExtensions.cs
│   ├── ErrorHandlingExtensions.cs
│   └── AuthenticationExtensions.cs
│
├── appsettings.json
├── Program.cs
└── Startup.cs
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQLite (bundled; no manual setup required)

### Setup & Run

1. **Clone the repository**

   ```sh
   git clone https://github.com/yourusername/breweryapi.git
   cd breweryapi
   ```

2. **Restore dependencies**

   ```sh
   dotnet restore
   ```

3. **Apply migrations and seed the database (on first run)**

   ```sh
   dotnet run --launch-profile https
   ```

   - This will automatically create and seed the SQLite database with data from OpenBreweryDB.

4. **API will be available at**

   ```
   https://localhost:7146
   ```

---

## Authentication

- **Token Endpoint:**  
  `POST /api/auth/token`  
  Example request body:

  ```json
  { "username": "brewery", "password": "secret" }
  ```

  > Returns a JWT to be used in the `Authorization: Bearer ...` header for protected endpoints.

- **Secured Endpoints:**  
  The main `GET /api/v{version}/breweries` endpoint requires authentication.

- **Test User:**
  - Username: `brewery`
  - Password: `secret`

---

## API Usage

### Get all breweries (paginated, sorted, filtered)

```
GET /api/v1/breweries?search=ale&city=Boston&sortBy=name&page=1&pageSize=20
Authorization: Bearer {token}
```

### Get a brewery by ID

```
GET /api/v1/breweries/{id}
GET /api/v2/breweries/{id}  // v2 returns additional fields
```

### Autocomplete (open, unauthenticated)

```
GET /api/v1/breweries/autocomplete?query=dog
```

### API Versioning Example

- `/api/v1/breweries/{id}` - basic brewery info
- `/api/v2/breweries/{id}` - wraps the result in a DTO with extra fields

---

## API Versioning

Implemented using [Asp.Versioning.Mvc](https://github.com/dotnet/aspnet-api-versioning).

- **URL segment versioning**: e.g., `/api/v1/breweries`, `/api/v2/breweries`
- New versions can extend/override endpoints for backward compatibility.

---

## Error Handling & Logging

- **Global Error Handling:**  
  All unhandled exceptions are logged and returned as generic error responses (prevents leaking sensitive info).
- **Serilog:**  
  Logs to the console; configuration in `appsettings.json`.  
  Additional sinks (file, cloud, etc.) can be enabled with minimal changes.

---

## Design Decisions

- **SOLID Principles:**  
  Clear separation of concerns via Logic, Repositories, Controllers, etc.
- **Extensibility:**  
  Dependency injection for all services; new features/endpoints are easy to add.
- **Performance:**  
  Uses in-memory and distributed caching, pagination, and efficient database queries.
- **Testability:**  
  Logic and repository layers are fully abstracted, ready for unit/integration testing.

---

## Testing & Extensibility

- **Unit tests** can be easily added (project supports dependency injection throughout).
- **Database can be swapped** (e.g., to SQL Server/Postgres) by changing EF Core provider and connection string.
- **Authentication** uses industry-standard JWT, easy to integrate with other identity providers.

---
