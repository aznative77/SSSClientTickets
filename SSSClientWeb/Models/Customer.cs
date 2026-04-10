using System;
using System.Collections.Generic;

namespace SSSClientWeb.Models;

public partial class Customer
{
    public int ClientRec { get; set; }

    public int CustomerRec { get; set; }

    public string? CustomerName { get; set; }

    public virtual Client ClientRecNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
