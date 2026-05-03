using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Tickets
{
    public class DetailModel : PageModel
    {
        private readonly SssclientContext _context;

        public DetailModel(SssclientContext context)
        {
            _context = context;
        }

        public Ticket Ticket { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .Include(t => t.StatusRecNavigation)
                .Include(t => t.SiteRecNavigation)
                .Include(t => t.TicketTimes)
                .Include(t => t.TicketAttachments)
                .FirstOrDefaultAsync(t => t.TicketRec == id);

            if (ticket == null)
                return NotFound();

            Ticket = ticket;
            return Page();
        }

        public async Task<IActionResult> OnPostToggleBilledAsync(int id, bool billed)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ticket.Billed = billed;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id });
        }
    }
}
