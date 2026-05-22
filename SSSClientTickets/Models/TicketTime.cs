using System;
using System.Collections.Generic;

namespace SSSClientTickets.Models;

public partial class TicketTime
{
    public int TimeRec { get; set; }

    public int TicketRec { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Notes { get; set; }

    public int? TimeRecordedByUserId { get; set; }

    public virtual AppUser? TimeRecordedByUser { get; set; }

    public virtual Ticket TicketRecNavigation { get; set; } = null!;
}
