using System;
using System.Collections.Generic;

namespace SSSClientWeb.Models;

public partial class TicketStatus
{
    public int StatusRec { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
