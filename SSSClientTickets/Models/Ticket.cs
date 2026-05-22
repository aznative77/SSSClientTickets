using System;
using System.Collections.Generic;

namespace SSSClientTickets.Models;

public partial class Ticket
{
    public int TicketRec { get; set; }

    public int ClientRec { get; set; }

    public int CustomerRec { get; set; }

    public string? Issue { get; set; }

    public string? Resolution { get; set; }

    public DateTime? DateLogged { get; set; }

    public DateTime? DateResolved { get; set; }

    public int StatusRec { get; set; }
    
    public DateTime? DateBilled { get; set; }

    public int? SiteRec { get; set; }

    public decimal HourlyRate { get; set; }

    public int? CreatedByUserId { get; set; }

    public int? ResolvedByUserId { get; set; }

    public virtual Client ClientRecNavigation { get; set; } = null!;

    public virtual Customer CustomerRecNavigation { get; set; } = null!;

    public virtual Site? SiteRecNavigation { get; set; }

    public virtual TicketStatus StatusRecNavigation { get; set; } = null!;

    public virtual AppUser? CreatedByUser { get; set; }

    public virtual AppUser? ResolvedByUser { get; set; }

    public virtual ICollection<TicketTime> TicketTimes { get; set; } = new List<TicketTime>();

    public virtual ICollection<TicketAttachment> TicketAttachments { get; set; } = new List<TicketAttachment>();
}
