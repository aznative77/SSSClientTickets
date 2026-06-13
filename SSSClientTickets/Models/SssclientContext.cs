using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Services;

namespace SSSClientTickets.Models;

public partial class SssclientContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private bool _savingChangeLogs;

    public SssclientContext()
    {
    }

    public SssclientContext(DbContextOptions<SssclientContext> options)
        : base(options)
    {
    }

    public SssclientContext(DbContextOptions<SssclientContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<ChangeLog> ChangeLogs { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<TicketTime> TicketTimes { get; set; }

    public virtual DbSet<VwOpenTicket> VwOpenTickets { get; set; }

    public virtual DbSet<VwTicketTime> VwTicketTimes { get; set; }

    public override int SaveChanges()
    {
        return SaveChangesAsync().GetAwaiter().GetResult();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_savingChangeLogs)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        StampTicketUsers();
        StampTicketTimeUsers();
        StampTicketAttachmentUsers();

        var pendingChangeLogs = BuildChangeLogs().ToList();
        var result = await base.SaveChangesAsync(cancellationToken);

        if (pendingChangeLogs.Count > 0)
        {
            _savingChangeLogs = true;
            foreach (var pendingChangeLog in pendingChangeLogs)
            {
                if (pendingChangeLog.ChangeLog.EntityRecordId == 0)
                {
                    pendingChangeLog.ChangeLog.EntityRecordId = GetPrimaryKeyValue(pendingChangeLog.Entry);
                }
            }

            ChangeLogs.AddRange(pendingChangeLogs.Select(p => p.ChangeLog));
            result += await base.SaveChangesAsync(cancellationToken);
            _savingChangeLogs = false;
        }

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("AppUser");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
        });

        modelBuilder.Entity<ChangeLog>(entity =>
        {
            entity.HasKey(e => e.ChangeLogId);

            entity.ToTable("ChangeLog");

            entity.Property(e => e.EntityName).HasMaxLength(50);
            entity.Property(e => e.Action).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ChangedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.ChangeLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ChangeLog_AppUser");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientRec);

            entity.ToTable("Client");

            entity.Property(e => e.ClientAddr1)
                .HasMaxLength(50)
                .HasColumnName("Client_Addr1");
            entity.Property(e => e.ClientAddr2)
                .HasMaxLength(50)
                .HasColumnName("Client_Addr2");
            entity.Property(e => e.ClientCity)
                .HasMaxLength(50)
                .HasColumnName("Client_City");
            entity.Property(e => e.ClientName)
                .HasMaxLength(50)
                .HasColumnName("Client_Name");
            entity.Property(e => e.ClientState)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("Client_State");
            entity.Property(e => e.ClientZip)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Client_Zip");
            entity.Property(e => e.HourlyRate)
                .HasColumnType("decimal(10, 2)")
                .HasDefaultValue(0m);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerRec);

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .HasColumnName("Customer_Name");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("Customer_Email");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("Customer_Phone");
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .HasColumnName("Customer_Mobile");

            entity.HasOne(d => d.ClientRecNavigation).WithMany(p => p.Customers)
                .HasForeignKey(d => d.ClientRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Client");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(e => e.SiteRec);

            entity.ToTable("Site");

            entity.Property(e => e.SiteAddress1)
                .HasMaxLength(50)
                .HasColumnName("Site_Address1");
            entity.Property(e => e.SiteAddress2)
                .HasMaxLength(50)
                .HasColumnName("Site_Address2");
            entity.Property(e => e.SiteCity)
                .HasMaxLength(50)
                .HasColumnName("Site_City");
            entity.Property(e => e.SiteName)
                .HasMaxLength(255)
                .HasColumnName("Site_Name");
            entity.Property(e => e.SiteState)
                .HasMaxLength(2)
                .HasColumnName("Site_State");
            entity.Property(e => e.SiteZip)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Site_Zip");

            entity.HasOne(d => d.ClientRecNavigation).WithMany(p => p.Sites)
                .HasForeignKey(d => d.ClientRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Site_Client");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketRec);

            entity.ToTable("Ticket");

            entity.Property(e => e.DateLogged).HasColumnType("datetime");
            entity.Property(e => e.DateResolved).HasColumnType("datetime");
            entity.Property(e => e.HourlyRate)
                .HasColumnType("decimal(10, 2)")
                .HasDefaultValue(0m);
            entity.Property(e => e.IsFlatRate).HasDefaultValue(false);
            entity.Property(e => e.StatusRec).HasDefaultValue(1);
            entity.Property(e => e.DateBilled).HasColumnType("datetime");

            entity.HasOne(d => d.ClientRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ClientRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Client");

            entity.HasOne(d => d.CustomerRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CustomerRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Customer");

            entity.HasOne(d => d.SiteRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SiteRec)
                .HasConstraintName("FK_Ticket_Site");

            entity.HasOne(d => d.StatusRecNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.StatusRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_TicketStatus");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.TicketsCreated)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_CreatedBy_AppUser");

            entity.HasOne(d => d.AssignedToUser).WithMany(p => p.TicketsAssigned)
                .HasForeignKey(d => d.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_AssignedTo_AppUser");

            entity.HasOne(d => d.ResolvedByUser).WithMany(p => p.TicketsResolved)
                .HasForeignKey(d => d.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ticket_ResolvedBy_AppUser");
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.StatusRec);

            entity.ToTable("TicketStatus");

            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasData(
                new TicketStatus { StatusRec = 1, Status = "Open" },
                new TicketStatus { StatusRec = 2, Status = "In Progress" },
                new TicketStatus { StatusRec = 3, Status = "Waiting for Client" },
                new TicketStatus { StatusRec = 4, Status = "Resolved" });
        });

        modelBuilder.Entity<TicketTime>(entity =>
        {
            entity.HasKey(e => e.TimeRec);

            entity.ToTable("TicketTime");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.TicketRecNavigation).WithMany(p => p.TicketTimes)
                .HasForeignKey(d => d.TicketRec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketTime_Ticket");

            entity.HasOne(d => d.TimeRecordedByUser).WithMany(p => p.TicketTimesRecorded)
                .HasForeignKey(d => d.TimeRecordedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TicketTime_TimeRecordedBy_AppUser");
        });

        modelBuilder.Entity<VwOpenTicket>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_OpenTickets");

            entity.Property(e => e.ClientName)
                .HasMaxLength(50)
                .HasColumnName("Client_Name");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(50)
                .HasColumnName("Customer_Name");
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<VwTicketTime>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_TicketTime");
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentRec);

            entity.ToTable("TicketAttachment");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileExtension).HasMaxLength(10);

            entity.HasOne(d => d.TicketRecNavigation).WithMany(p => p.TicketAttachments)
                .HasForeignKey(d => d.TicketRec)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TicketAttachment_Ticket");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.TicketAttachmentsUploaded)
                .HasForeignKey(d => d.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TicketAttachment_UploadedBy_AppUser");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    private void StampTicketUsers()
    {
        var userId = _currentUserService?.UserId;
        if (userId == null)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries<Ticket>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedByUserId == null)
            {
                entry.Entity.CreatedByUserId = userId;
            }

            if (entry.State == EntityState.Added && entry.Entity.AssignedToUserId == null)
            {
                entry.Entity.AssignedToUserId = entry.Entity.CreatedByUserId;
            }

            if (entry.State is EntityState.Added or EntityState.Modified
                && entry.Entity.ResolvedByUserId == null
                && IsResolved(entry.Entity))
            {
                entry.Entity.ResolvedByUserId = userId;
            }
        }
    }

    private void StampTicketTimeUsers()
    {
        var userId = _currentUserService?.UserId;
        if (userId == null)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries<TicketTime>())
        {
            if (entry.State == EntityState.Added && entry.Entity.TimeRecordedByUserId == null)
            {
                entry.Entity.TimeRecordedByUserId = userId;
            }
        }
    }

    private void StampTicketAttachmentUsers()
    {
        var userId = _currentUserService?.UserId;
        if (userId == null)
        {
            return;
        }

        foreach (var entry in ChangeTracker.Entries<TicketAttachment>())
        {
            if (entry.State == EntityState.Added && entry.Entity.UploadedByUserId == null)
            {
                entry.Entity.UploadedByUserId = userId;
            }
        }
    }

    private IEnumerable<PendingChangeLog> BuildChangeLogs()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ChangeLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
            {
                continue;
            }

            var entityName = entry.Entity switch
            {
                AppUser => "User",
                Client => "Client",
                Customer => "Customer",
                Site => "Site",
                Ticket => "Ticket",
                _ => null
            };

            if (entityName == null)
            {
                continue;
            }

            yield return new PendingChangeLog(
                entry,
                new ChangeLog
            {
                EntityName = entityName,
                EntityRecordId = GetPrimaryKeyValue(entry),
                Action = entry.State.ToString(),
                Description = BuildChangeDescription(entry, entityName),
                UserId = _currentUserService?.UserId,
                ChangedAt = DateTime.Now
            });
        }
    }

    private static int GetPrimaryKeyValue(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        var property = key?.Properties.FirstOrDefault();
        if (property == null)
        {
            return 0;
        }

        var value = entry.Property(property.Name).CurrentValue;
        return value is int intValue ? intValue : 0;
    }

    private static bool IsResolved(Ticket ticket)
    {
        return ticket.DateResolved.HasValue || ticket.StatusRec == 4;
    }

    private string BuildChangeDescription(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string entityName)
    {
        if (entry.Entity is AppUser user)
        {
            return BuildUserChangeDescription(entry, user);
        }

        if (entry.Entity is Ticket ticket)
        {
            return BuildTicketChangeDescription(entry, ticket);
        }

        return $"{entry.State} {entityName}";
    }

    private string BuildTicketChangeDescription(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, Ticket ticket)
    {
        if (entry.State == EntityState.Added)
        {
            return $"Created ticket #{ticket.TicketRec}";
        }

        if (entry.State == EntityState.Deleted)
        {
            return $"Deleted ticket #{ticket.TicketRec}";
        }

        var changes = new List<string>();

        if (entry.Property(nameof(Ticket.AssignedToUserId)).IsModified)
        {
            var originalId = entry.Property(nameof(Ticket.AssignedToUserId)).OriginalValue as int?;
            var currentId = entry.Property(nameof(Ticket.AssignedToUserId)).CurrentValue as int?;

            var originalName = GetUserNameById(originalId) ?? "Unassigned";
            var currentName = GetUserNameById(currentId) ?? "Unassigned";

            changes.Add($"changed assigned user from {originalName} to {currentName}");
        }

        return changes.Count == 0
            ? $"Updated ticket #{ticket.TicketRec}"
            : $"Updated ticket #{ticket.TicketRec}: {string.Join(", ", changes)}";
    }

    private string? GetUserNameById(int? userId)
    {
        if (!userId.HasValue)
        {
            return null;
        }

        return AppUsers.Local.FirstOrDefault(u => u.UserId == userId.Value)?.FullName
            ?? AppUsers.Find(userId.Value)?.FullName;
    }

    private static string BuildUserChangeDescription(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, AppUser user)
    {
        var userName = user.FullName;

        if (entry.State == EntityState.Added)
        {
            return $"Created user {userName}";
        }

        if (entry.State == EntityState.Deleted)
        {
            return $"Deleted user {userName}";
        }

        var changes = new List<string>();

        AddUserChange(entry, changes, nameof(AppUser.FirstName), "changed first name");
        AddUserChange(entry, changes, nameof(AppUser.LastName), "changed last name");
        AddUserChange(entry, changes, nameof(AppUser.Email), "changed email");
        AddBooleanUserChange(entry, changes, nameof(AppUser.IsApproved), "approved", "marked pending approval");
        AddBooleanUserChange(entry, changes, nameof(AppUser.IsActive), "activated", "deactivated");
        AddBooleanUserChange(entry, changes, nameof(AppUser.IsAdmin), "granted admin", "revoked admin");

        if (entry.Property(nameof(AppUser.PasswordHash)).IsModified)
        {
            changes.Add("reset password");
        }

        return changes.Count == 0
            ? $"Updated user {userName}"
            : $"Updated user {userName}: {string.Join(", ", changes)}";
    }

    private static void AddUserChange(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
        ICollection<string> changes,
        string propertyName,
        string description)
    {
        if (entry.Property(propertyName).IsModified)
        {
            changes.Add(description);
        }
    }

    private static void AddBooleanUserChange(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
        ICollection<string> changes,
        string propertyName,
        string trueDescription,
        string falseDescription)
    {
        var property = entry.Property(propertyName);
        if (!property.IsModified)
        {
            return;
        }

        changes.Add(property.CurrentValue is true ? trueDescription : falseDescription);
    }

    private sealed record PendingChangeLog(
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry,
        ChangeLog ChangeLog);

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
