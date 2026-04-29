using System;
using System.Collections.Generic;

namespace SSSClientTickets.Models;

public partial class TicketAttachment
{
    public int AttachmentRec { get; set; }

    public int TicketRec { get; set; }

    public string FileName { get; set; } = null!;

    public string FileExtension { get; set; } = null!;

    public long FileSizeBytes { get; set; }

    public DateTime UploadedDate { get; set; }

    public bool IsImage { get; set; }

    public virtual Ticket TicketRecNavigation { get; set; } = null!;
}
