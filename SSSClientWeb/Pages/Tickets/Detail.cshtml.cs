using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Tickets
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
                .Include(t => t.TicketTimes)
                .FirstOrDefaultAsync(t => t.TicketRec == id);

            if (ticket == null)
                return NotFound();

            Ticket = ticket;
            return Page();
        }
    }
}
