using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly SssclientContext _context;

        public CreateModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        [BindProperty]
        public bool HourlyRateWasChanged { get; set; }

        public SelectList ClientList { get; set; } = default!;
        public SelectList CustomerList { get; set; } = default!;
        public SelectList StatusList { get; set; } = default!;
        public SelectList SiteList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            await PopulateDropdownsAsync();
            Ticket.DateLogged = DateTime.Today;
            Ticket.StatusRec = 1; // Default to Open
            HourlyRateWasChanged = false;
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

            if (!HourlyRateWasChanged)
            {
                Ticket.HourlyRate = await _context.Clients
                    .Where(c => c.ClientRec == Ticket.ClientRec)
                    .Select(c => c.HourlyRate)
                    .FirstOrDefaultAsync();
            }

            _context.Tickets.Add(Ticket);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        private async Task PopulateDropdownsAsync()
        {
            ClientList = new SelectList(
                await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                "ClientRec", "ClientName");

            CustomerList = new SelectList(
                await _context.Customers.OrderBy(c => c.CustomerName).ToListAsync(),
                "CustomerRec", "CustomerName");

            StatusList = new SelectList(
                await _context.TicketStatuses.ToListAsync(),
                "StatusRec", "Status");

            SiteList = new SelectList(
                await _context.Sites
                    .Include(s => s.ClientRecNavigation)
                    .OrderBy(s => s.ClientRecNavigation.ClientName)
                    .ThenBy(s => s.SiteName)
                    .ToListAsync(),
                "SiteRec", "SiteName");
        }
    }
}
