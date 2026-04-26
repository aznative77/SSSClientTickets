using System;
using System.Collections.Generic;

namespace SSSClientTickets.Models;

public partial class Client
{
    public int ClientRec { get; set; }

    public string? ClientName { get; set; }

    public string? ClientAddr1 { get; set; }

    public string? ClientAddr2 { get; set; }

    public string? ClientCity { get; set; }

    public string? ClientState { get; set; }

    public string? ClientZip { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Site> Sites { get; set; } = new List<Site>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
