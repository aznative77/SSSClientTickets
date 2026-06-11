using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [BindProperty]
        public int TicketStatusRec { get; set; }

        public Ticket? TicketInfo { get; set; }
        public SelectList StatusList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var entry = await _context.TicketTimes.FindAsync(id);
            if (entry == null)
                return NotFound();

            TicketEntry = entry;

            if (!await LoadTicketInfoAsync(TicketEntry.TicketRec))
                return NotFound();

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

            var entryToUpdate = await _context.TicketTimes.FindAsync(TicketEntry.TimeRec);
            if (entryToUpdate == null)
                return NotFound();

            var ticket = await _context.Tickets.FindAsync(TicketEntry.TicketRec);
            if (ticket == null)
                return NotFound();

            ticket.StatusRec = TicketStatusRec;

            await TryUpdateModelAsync(
                entryToUpdate,
                "TicketEntry",
                t => t.TicketRec,
                t => t.StartTime,
                t => t.EndTime,
                t => t.Notes);

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

        private async Task<bool> LoadTicketInfoAsync(int ticketRec)
        {
            TicketInfo = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
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
