using System;
using System.Collections.Generic;

namespace SSSClientWeb.Models;

public partial class TicketTime
{
    public int TimeRec { get; set; }

    public int TicketRec { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Notes { get; set; }

    public virtual Ticket TicketRecNavigation { get; set; } = null!;
}
