# Brewery API (.NET 9)

A scalable, production‑ready RESTful API for querying brewery information, built with a focus on code quality, maintainability, and extensibility.

## Table of Contents

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

This API was developed as part of a recruitment assignment for a .NET developer role.
It integrates with the public [OpenBreweryDB](https://www.openbrewerydb.org/documentation), stores data locally in SQLite via Entity Framework Core, and exposes clean endpoints for brewery listing, search, sorting, and autocomplete.

The project follows SOLID principles, is fully test‑ready, and includes production‑grade features such as global error handling, structured logging, in‑memory caching, API versioning, and JWT‑based authentication.

---

## Features

### Core

- **Query breweries by name or city** (case‑insensitive)
- **Sort** results by **name**, **city**, or **distance** (when user coordinates are supplied)
- **Search** functionality with 10‑minute in‑memory cache of OpenBreweryDB data
- **Dependency Injection** throughout (services wired via interfaces)
- **Global error handling** middleware returning consistent JSON responses

### Bonus Implementations

- **Autocomplete** endpoint for fast type‑ahead suggestions (top 15 matches)
- **API versioning** (`v1`, `v2` – v2 demonstrates an extended response DTO)
- **Structured logging** with Serilog (console sink; easily extendable)
- **SQLite + EF Core** persistent store with automatic migration & seeding
- **JWT authentication** securing primary endpoints (sample user included)

> **Note** : The phone number field from OpenBreweryDB is included in responses, but it is **not** a search/filter criterion.

---

## Project Structure

```text
BreweryAPI/
├── Controllers/
│   ├── BreweriesController.cs         # v1 & v2 endpoints
│   └── AuthController.cs              # JWT token issuance
├── Data/
│   ├── BreweryDbContext.cs            # EF Core context (SQLite)
│   └── DbSeeder.cs                    # Initial data load from OpenBreweryDB
├── Infrastructure/
│   └── ErrorHandlingMiddleware.cs     # Global exception handling
├── Logic/
│   ├── BreweryLogic.cs                # Business logic (filter, sort, paginate)
│   └── Interfaces/IBreweryLogic.cs
├── Models/                            # Domain & DTO classes
├── Repositories/
│   ├── BreweryEfRepository.cs         # SQLite implementation
│   ├── BreweryApiRepository.cs        # External API implementation
│   └── Interfaces/IBreweryRepository.cs
├── StartupConfiguration/              # Extension methods
│   ├── DependencyInjectionExtensions.cs
│   ├── ErrorHandlingExtensions.cs
│   └── AuthenticationExtensions.cs
├── appsettings.json                   # Configuration (DB, JWT, logging)
├── Program.cs                         # Entry point, Serilog bootstrap
└── Startup.cs                         # ConfigureServices / Configure
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- No external database server needed (SQLite file `brewery.db` is created automatically)

### Setup & Run

```bash
# 1. Clone repository
 git clone https://github.com/PatrykLazuk/BreweryAPI.git
 cd BreweryAPI

# 2. Restore dependencies
 dotnet restore

# 3. Run (applies migrations & seeds DB on first launch)
 dotnet run --launch-profile https
```

The API will be available at **[https://localhost:7146](https://localhost:7146)** (HTTPS profile).

Environment selection:

```bash
# Run with external API as the live data source instead of SQLite
DataSource=Api dotnet run --launch-profile https
```

---

## Authentication

| Endpoint          | Method | Description                                        |
| ----------------- | ------ | -------------------------------------------------- |
| `/api/auth/token` | `POST` | Obtain JWT token (send JSON body with credentials) |

**Sample request body**

```json
{ "username": "brewery", "password": "secret" }
```

- On success, the response contains an `access_token` to be passed in the
  `Authorization: Bearer <token>` header.
- Primary brewery endpoints require a valid token; autocomplete and version‑info are public.

### Demo Credentials

| Username | Password |
| -------- | -------- |
| brewery  | secret   |

---

## API Usage

### List Breweries (paginated, sortable, searchable)

```http
GET /api/v1/breweries?search=ale&city=Boston&sortBy=name&page=1&pageSize=20
Authorization: Bearer <token>
```

**Query parameters**

| Name                 | Description                    | Example                                        |
| -------------------- | ------------------------------ | ---------------------------------------------- |
| `search`             | Partial name filter            | `search=ale`                                   |
| `city`               | Filter by exact city           | `city=Boston`                                  |
| `sortBy`             | `name` \| `city` \| `distance` | `sortBy=distance&userLat=42.36&userLng=-71.06` |
| `userLat`, `userLng` | Coordinates for distance sort  |                                                |
| `page`, `pageSize`   | Pagination (default 1 / 20)    |                                                |

### Get Brewery by ID

```http
GET /api/v1/breweries/{id}
GET /api/v2/breweries/{id}   # v2 wraps the response in an extended DTO
```

### Autocomplete (public)

```http
GET /api/v1/breweries/autocomplete?query=dog
```

Returns up to 15 matching `{ id, name }` pairs.

---

## API Versioning

Implemented via **ASP.NET API Versioning** using URL segments:

- **v1** – baseline contract
- **v2** – example of a backward‑compatible extension (extra field in response)

Clients can continue calling `/api/v1/...` unchanged while newer clients adopt `/api/v2/...`.

---

## Error Handling & Logging

- **Global error middleware** captures unhandled exceptions and returns a sanitized JSON error (HTTP 500), while logging details for diagnostics.
- **Serilog** is configured via _appsettings.json_ to log to the console with a minimum level of Information. Additional sinks (file, Seq, cloud) can be added with a one‑line change.

---

## Design Decisions

- **SOLID & Clean Architecture** – responsibilities divided among Controllers, Logic, and Repositories; dependencies inverted via interfaces.
- **Extensibility** – swapping the data source (SQLite ↔︎ live API) is a configuration change, not a code change.
- **Performance** – leverages in‑memory caching, pagination, and indexed queries; ready for future distributed cache if needed.
- **Security** – JWT authentication follows standard validation (issuer, audience, key) and can integrate with external identity providers.
- **AI‑assisted Development** – GitHub Copilot was used as an AI pair‑programming tool to boost productivity; all suggestions were critically reviewed and refactored to meet the project’s coding standards and architectural guidelines.

---

## Testing & Extensibility

- **Unit Testing Ready** – abstractions allow mocking repositories or logic for isolated tests.
- **Database Agnostic** – replace SQLite with SQL Server/PostgreSQL by changing EF Core provider and connection string.
- **CI/CD Friendly** – deterministic startup (automatic migrations, seeding) enables repeatable deployments and containerization.

---

## Contact

For questions or feedback, please open an issue or reach out via GitHub.

**Author:** [Patryk Lazuk](https://github.com/PatrykLazuk)

---

© 2025 Patryk Lazuk. Licensed for evaluation purposes.
