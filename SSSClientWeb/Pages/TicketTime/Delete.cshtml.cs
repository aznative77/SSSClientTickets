using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.TicketTime
{
    public class DeleteModel : PageModel
    {
        private readonly SssclientContext _context;

        public DeleteModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.TicketTime TicketEntry { get; set; } = new Models.TicketTime();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var entry = await _context.TicketTimes.FindAsync(id);
            if (entry == null)
                return NotFound();

            TicketEntry = entry;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var entry = await _context.TicketTimes.FindAsync(TicketEntry.TimeRec);
            int ticketRec = TicketEntry.TicketRec;

            if (entry != null)
            {
                ticketRec = entry.TicketRec;
                _context.TicketTimes.Remove(entry);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Tickets/Detail", new { id = ticketRec });
        }
    }
}
