using System;

namespace SSSClientTickets.Models;

public class ChangeLog
{
    public int ChangeLogId { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public int EntityRecordId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? UserId { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.Now;

    public virtual AppUser? User { get; set; }
}
