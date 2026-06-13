using System;
using System.Collections.Generic;

namespace SSSClientTickets.Models;

public class AppUser
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName
    {
        get
        {
            var fullName = $"{FirstName} {LastName}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? Email : fullName;
        }
    }

    public bool IsActive { get; set; } = true;

    public bool IsApproved { get; set; }

    public bool IsAdmin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<Ticket> TicketsCreated { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketsAssigned { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketsResolved { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketTime> TicketTimesRecorded { get; set; } = new List<TicketTime>();

    public virtual ICollection<TicketAttachment> TicketAttachmentsUploaded { get; set; } = new List<TicketAttachment>();

    public virtual ICollection<ChangeLog> ChangeLogs { get; set; } = new List<ChangeLog>();
}
