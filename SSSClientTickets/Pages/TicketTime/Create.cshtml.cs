using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.TicketTime
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

        [BindProperty]
        public int TicketStatusRec { get; set; }

        public Ticket? TicketInfo { get; set; }
        public SelectList StatusList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int ticketRec)
        {
            if (!await LoadTicketInfoAsync(ticketRec))
                return NotFound();

            // Pre-populate TicketRec and default times
            TicketEntry.TicketRec = ticketRec;
            TicketEntry.StartTime = DateTime.Now;
            TicketEntry.EndTime = DateTime.Now;
            TicketStatusRec = TicketInfo!.StatusRec;
            await PopulateDropdownsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("TicketEntry.TicketRecNavigation");

            if (!ModelState.IsValid)
            {
                await LoadTicketInfoAsync(TicketEntry.TicketRec);
                await PopulateDropdownsAsync();
                return Page();
            }

            var ticket = await _context.Tickets.FindAsync(TicketEntry.TicketRec);
            if (ticket == null)
                return NotFound();

            ticket.StatusRec = TicketStatusRec;
            _context.TicketTimes.Add(TicketEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Tickets/Detail", new { id = TicketEntry.TicketRec });
        }

        private async Task<bool> LoadTicketInfoAsync(int ticketRec)
        {
            TicketInfo = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .Include(t => t.SiteRecNavigation)
                .FirstOrDefaultAsync(t => t.TicketRec == ticketRec);

            return TicketInfo != null;
        }

        private async Task PopulateDropdownsAsync()
        {
            StatusList = new SelectList(
                await _context.TicketStatuses.ToListAsync(),
                "StatusRec", "Status", TicketStatusRec);
        }
    }
}
