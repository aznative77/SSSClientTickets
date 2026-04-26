using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.TicketTime
{
    public class EditModel : PageModel
    {
        private readonly SssclientContext _context;

        public EditModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.TicketTime TicketEntry { get; set; } = new Models.TicketTime();

        public Ticket? TicketInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var entry = await _context.TicketTimes.FindAsync(id);
            if (entry == null)
                return NotFound();

            TicketEntry = entry;

            TicketInfo = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .FirstOrDefaultAsync(t => t.TicketRec == TicketEntry.TicketRec);

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

            _context.Attach(TicketEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TicketTimes.Any(t => t.TimeRec == TicketEntry.TimeRec))
                    return NotFound();
                throw;
            }

            return RedirectToPage("/Tickets/Detail", new { id = TicketEntry.TicketRec });
        }
    }
}
