# SSSClientTickets

SSSClientTickets is an ASP.NET Core 8 Razor Pages application for managing client support tickets, ticket time, attachments, and client/customer/site records.

## Overview

The app tracks support work across clients, customers, and sites. It includes authenticated user accounts so ticket activity can be tied back to the person who created, resolved, recorded, uploaded, or changed information.

## Features

- **Client Management** - Create and manage client records.
- **Customer Management** - Track customer contact information associated with clients.
- **Site Management** - Organize client locations and associate tickets with sites.
- **Ticket Management** - Create, edit, view, and delete support tickets.
- **Ticket Time Tracking** - Log time spent on each ticket.
- **Ticket Attachments** - Upload, preview, download, and delete files attached to tickets.
- **User Authentication** - Login/register flow using cookie authentication and hashed passwords.
- **Admin User Management** - Admins can add users, approve users, activate/deactivate accounts, assign admin status, reset passwords, and delete unused users.
- **Approval Workflow** - New self-registered users must be approved by an admin before they can access the site.
- **Activity Attribution** - Tickets record who created and resolved them, ticket time records who logged time, and attachments record who uploaded files.
- **Change Log** - Changes to Clients, Customers, and Sites are logged with the user who made the change.
- **RESTful API** - Ticket and attachment endpoints support page behavior and programmatic access.

## Prerequisites

- **.NET 8 SDK** or later
- **SQL Server 2019** or later, Express or full SQL Server
- **Visual Studio 2022** or **VS Code** with the C# extension
- Optional: `dotnet-ef` for manual migration commands

## Installation

### 1. Clone the Repository

```bash
git clone <repository-url>
cd SSSClientTickets
```

### 2. Configure Database Connection

Copy the example settings file:

```powershell
copy SSSClientTickets\appsettings.Example.json SSSClientTickets\appsettings.json
```

Then edit `SSSClientTickets/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "SSSClientConnection": "Server=YOUR_SERVER;Database=SSSClient;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Replace `YOUR_SERVER` with your SQL Server instance, such as `localhost`, `.\SQLEXPRESS`, or a server hostname.

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run --project SSSClientTickets/SSSClientTickets.csproj
```

The application automatically applies pending Entity Framework migrations on startup.

## First Login

The first registered account is automatically approved and marked as an admin. After that:

- Self-registered users are created as pending approval.
- Pending users cannot access the site until an admin approves them.
- Inactive users cannot access the site and are told to contact the admin.
- Admins can manage users from the **Users** navigation item.

## User Management

Admins can use `/Admin/Users/Index` to:

- View all users.
- Add users manually.
- Approve pending users.
- Toggle active/inactive status.
- Toggle admin status.
- Reset passwords.
- Delete users that are not connected to tickets, time entries, attachments, or change logs.

If a user already has related history, mark them inactive instead of deleting them so historical records stay intact.

## Audit and Attribution

The app records user activity in several places:

- `Ticket.CreatedByUserId` - user who created the ticket.
- `Ticket.ResolvedByUserId` - user who resolved the ticket.
- `TicketTime.TimeRecordedByUserId` - user who recorded ticket time.
- `TicketAttachment.UploadedByUserId` - user who uploaded an attachment.
- `ChangeLog.UserId` - user who changed a Client, Customer, or Site record.

The Change Log page shows recent Client, Customer, and Site changes with the user and timestamp.

## Database

The application uses Entity Framework Core with SQL Server.

### Migrations

Migrations are automatically applied on startup. To manage them manually:

```bash
dotnet ef migrations add <MigrationName> --project SSSClientTickets/SSSClientTickets.csproj
dotnet ef database update --project SSSClientTickets/SSSClientTickets.csproj
```

Recent authentication and audit migrations include:

- `AddUsersAndAudit`
- `RequireFirstAndLastName`
- `AddUserApproval`
- `AddTicketAttachmentUploadedBy`

## Project Structure

```text
SSSClientTickets/
+-- Controllers/        # API controllers for tickets and attachments
+-- Models/             # EF models and SssclientContext
+-- Pages/              # Razor Pages UI
|   +-- Account/        # Login, register, logout
|   +-- Admin/Users/    # Admin user management
|   +-- ChangeLogs/     # Change log viewer
|   +-- Clients/        # Client management pages
|   +-- Customers/      # Customer management pages
|   +-- Sites/          # Site management pages
|   +-- Tickets/        # Ticket management pages
|   +-- TicketTime/     # Ticket time tracking pages
+-- Services/           # Current user helper service
+-- Migrations/         # EF Core database migrations
+-- wwwroot/            # Static files, uploads, CSS, JavaScript
+-- appsettings.json    # Local configuration, not committed
```

## API Endpoints

Ticket endpoints:

- `GET /api/tickets`
- `GET /api/tickets/{id}`
- `POST /api/tickets`
- `PUT /api/tickets/{id}`
- `DELETE /api/tickets/{id}`
- `GET /api/tickets/client-hourly-rate?clientId={id}`

Attachment endpoints:

- `POST /api/attachments/upload`
- `GET /api/attachments/ticket/{ticketRec}`
- `GET /api/attachments/file/{ticketRec}/{attachmentRec}`
- `DELETE /api/attachments/delete/{attachmentRec}`

## Troubleshooting

### Database Connection Error

- Verify SQL Server is running.
- Check `SSSClientTickets/appsettings.json`.
- Include `TrustServerCertificate=True` for local SQL Server development if the SQL client reports certificate or encryption issues.

### No Admin Account

The first registered account becomes the first admin. If users already exist but none are admins, update one directly in SQL:

```sql
UPDATE dbo.AppUser
SET IsAdmin = 1, IsActive = 1, IsApproved = 1
WHERE Email = 'your-email@example.com';
```

### Port Already in Use

Run on a different port:

```bash
dotnet run --project SSSClientTickets/SSSClientTickets.csproj --urls="http://localhost:5002"
```

### Reset Local Database

Only use this for disposable development data:

```bash
dotnet ef database drop --project SSSClientTickets/SSSClientTickets.csproj
dotnet ef database update --project SSSClientTickets/SSSClientTickets.csproj
```

## Contributing

1. Create a new branch for your feature
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## License

This project is licensed under the MIT License with the Commons Clause. See [LICENSE](LICENSE) for details.

**Summary:**
- ✓ You can use, modify, and distribute this for **non-profit purposes**
- ✗ Commercial use requires written permission from the author

## Support

For questions or issues, contact [aznative77@gmail.com](mailto:aznative77@gmail.com).
