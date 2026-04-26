using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Tickets
{
    public class DeleteModel : PageModel
    {
        private readonly SssclientContext _context;

        public DeleteModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .Include(t => t.StatusRecNavigation)
                .FirstOrDefaultAsync(t => t.TicketRec == id);

            if (ticket == null)
                return NotFound();

            Ticket = ticket;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var ticket = await _context.Tickets
                .Include(t => t.TicketTimes)
                .FirstOrDefaultAsync(t => t.TicketRec == Ticket.TicketRec);

            if (ticket != null)
            {
                // Remove associated time logs first
                _context.TicketTimes.RemoveRange(ticket.TicketTimes);
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
