using System;
using System.Collections.Generic;

namespace SSSClientWeb.Models;

public partial class VwOpenTicket
{
    public int TicketRec { get; set; }

    public string? ClientName { get; set; }

    public string? CustomerName { get; set; }

    public string? Issue { get; set; }

    public string Status { get; set; } = null!;
}
