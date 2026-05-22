using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Tickets
{
    public class EditModel : PageModel
    {
        private readonly SssclientContext _context;

        public EditModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        public SelectList ClientList { get; set; } = default!;
        public SelectList CustomerList { get; set; } = default!;
        public SelectList StatusList { get; set; } = default!;
        public SelectList SiteList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            Ticket = ticket;
            await PopulateDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Ticket.ClientRecNavigation");
            ModelState.Remove("Ticket.CustomerRecNavigation");
            ModelState.Remove("Ticket.StatusRecNavigation");

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return Page();
            }

            var ticketToUpdate = await _context.Tickets.FindAsync(Ticket.TicketRec);
            if (ticketToUpdate == null)
                return NotFound();

            await TryUpdateModelAsync(
                ticketToUpdate,
                "Ticket",
                t => t.ClientRec,
                t => t.CustomerRec,
                t => t.SiteRec,
                t => t.HourlyRate,
                t => t.Issue,
                t => t.Resolution,
                t => t.StatusRec,
                t => t.DateLogged,
                t => t.DateResolved,
                t => t.DateBilled);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tickets.Any(t => t.TicketRec == Ticket.TicketRec))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Detail", new { id = Ticket.TicketRec });
        }

        private async Task PopulateDropdownsAsync()
        {
            ClientList = new SelectList(
                await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                "ClientRec", "ClientName", Ticket.ClientRec);

            CustomerList = new SelectList(
                await _context.Customers.OrderBy(c => c.CustomerName).ToListAsync(),
                "CustomerRec", "CustomerName", Ticket.CustomerRec);

            StatusList = new SelectList(
                await _context.TicketStatuses.ToListAsync(),
                "StatusRec", "Status", Ticket.StatusRec);

            SiteList = new SelectList(
                await _context.Sites
                    .Include(s => s.ClientRecNavigation)
                    .OrderBy(s => s.ClientRecNavigation.ClientName)
                    .ThenBy(s => s.SiteName)
                    .ToListAsync(),
                "SiteRec", "SiteName", Ticket.SiteRec);
        }
    }
}
