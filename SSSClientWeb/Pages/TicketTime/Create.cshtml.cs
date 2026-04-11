using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.TicketTime
{
    public class CreateModel : PageModel
    {
        private readonly SssclientContext _context;

        public CreateModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.TicketTime TicketEntry { get; set; } = new Models.TicketTime();

        public Ticket? TicketInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int ticketRec)
        {
            TicketInfo = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .FirstOrDefaultAsync(t => t.TicketRec == ticketRec);

            if (TicketInfo == null)
                return NotFound();

            // Pre-populate TicketRec and default times
            TicketEntry.TicketRec = ticketRec;
            TicketEntry.StartTime = DateTime.Now;
            TicketEntry.EndTime = DateTime.Now;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("TicketEntry.TicketRecNavigation");

            if (!ModelState.IsValid)
            {
                TicketInfo = await _context.Tickets
                    .Include(t => t.ClientRecNavigation)
                    .Include(t => t.CustomerRecNavigation)
                    .FirstOrDefaultAsync(t => t.TicketRec == TicketEntry.TicketRec);
                return Page();
            }

            _context.TicketTimes.Add(TicketEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Tickets/Detail", new { id = TicketEntry.TicketRec });
        }
    }
}
