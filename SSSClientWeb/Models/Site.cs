using System;
using System.Collections.Generic;

namespace SSSClientWeb.Models;

public partial class Site
{
    public int SiteRec { get; set; }

    public int ClientRec { get; set; }

    public string? SiteName { get; set; }

    public string? SiteAddress1 { get; set; }

    public string? SiteAddress2 { get; set; }

    public string? SiteCity { get; set; }

    public string? SiteState { get; set; }

    public string? SiteZip { get; set; }

    public virtual Client ClientRecNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
