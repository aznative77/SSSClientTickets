# SSSClientTickets

A ticket management system for managing client support tickets, built with ASP.NET Core 8 and SQL Server.

## Overview

SSSClientTickets is an ASP.NET Core web application for tracking and managing support tickets across multiple clients and sites. It provides an intuitive interface for creating, editing, and monitoring ticket status and time tracking.

## Features

- **Client Management** — Create and manage client records
- **Customer Management** — Track customer information associated with clients
- **Site Management** — Organize tickets by client sites
- **Ticket Management** — Create, edit, and delete support tickets
- **Ticket Time Tracking** — Log time spent on each ticket
- **Status Tracking** — Monitor ticket status changes
- **RESTful API** — Ticket endpoints for programmatic access

## Prerequisites

- **.NET 8 SDK** or later — [Download](https://dotnet.microsoft.com/download)
- **SQL Server 2019** or later (Express or Full)
- **Visual Studio 2022** or **VS Code** with C# extension

## Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd SSSClientTickets
```

### 2. Configure Database Connection

Copy the example settings file:

```bash
copy SSSClientTickets\appsettings.Example.json SSSClientTickets\appsettings.json
```

Then edit `SSSClientTickets/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "SSSClientConnection": "Server=YOUR_SERVER;Database=SSSClient;Trusted_Connection=True;TrustServerCertificate=True"
  },
  ...
}
```

Replace:
- `YOUR_SERVER` — Your SQL Server instance name (e.g., `localhost`, `.\SQLEXPRESS`, or server hostname)
- Ensure the database `SSSClient` exists or Entity Framework will create it

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run
```

The application will:
- Start on `https://localhost:5001` or `http://localhost:5000`
- Automatically apply any pending database migrations

## Configuration

### appsettings.json

The `appsettings.json` file contains sensitive configuration and **should never be committed to version control**. It is in `.gitignore` by default.

For development or deployment, create your own `appsettings.json` based on `appsettings.Example.json`.

### Environment-Specific Settings

You can create environment-specific configuration files:
- `appsettings.Development.json` — Development settings
- `appsettings.Production.json` — Production settings

The application will load the appropriate file based on the `ASPNETCORE_ENVIRONMENT` variable.

## Database

The application uses Entity Framework Core with SQL Server.

### Migrations

Migrations are automatically applied on startup. To manually manage migrations:

```bash
# Add a new migration
dotnet ef migrations add <MigrationName>

# Update the database
dotnet ef database update

# Revert to previous migration
dotnet ef database update <PreviousMigrationName>
```

## Project Structure

```
SSSClientTickets/
├── Controllers/        # API controllers
├── Models/            # Database models and DbContext
├── Pages/             # Razor Pages (UI)
│   ├── Clients/       # Client management pages
│   ├── Customers/     # Customer management pages
│   ├── Sites/         # Site management pages
│   ├── Tickets/       # Ticket management pages
│   └── TicketTime/    # Ticket time tracking pages
├── Migrations/        # EF Core database migrations
├── wwwroot/           # Static files (CSS, JavaScript)
└── appsettings.json   # Configuration (not in version control)
```

## Usage

1. **Navigate to the home page** — `https://localhost:5001`
2. **Manage Clients** — Add and edit client records
3. **Manage Sites** — Associate sites with clients
4. **Create Tickets** — Create new support tickets for clients
5. **Track Time** — Log time spent on each ticket
6. **Monitor Status** — View ticket status and details

## API Endpoints

The `TicketsController` provides RESTful endpoints for ticket management:

- `GET /api/tickets` — Get all tickets
- `GET /api/tickets/{id}` — Get ticket details
- `POST /api/tickets` — Create a new ticket
- `PUT /api/tickets/{id}` — Update a ticket
- `DELETE /api/tickets/{id}` — Delete a ticket

## Troubleshooting

### Database Connection Error

- Verify SQL Server is running
- Check the connection string in `appsettings.json`
- Ensure the database exists or has been created by EF Core

### Port Already in Use

If port 5001/5000 is already in use, you can specify a different port:

```bash
dotnet run --urls="http://localhost:5002"
```

### Migrations Won't Apply

Clear the local migrations and re-apply:

```bash
dotnet ef database drop
dotnet ef database update
```

## Contributing

1. Create a new branch for your feature
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## License

This project is licensed under the MIT License with the Commons Clause — see the [LICENSE](LICENSE) file for details.

**Summary:**
- ✓ You can use, modify, and distribute this for **non-profit purposes**
- ✗ Commercial use requires written permission from the author

## Support

For questions or issues, please contact [aznative77@gmail.com](mailto:aznative77@gmail.com).
