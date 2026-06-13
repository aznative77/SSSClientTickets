using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;
using SSSClientTickets.Services;

namespace SSSClientTickets.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly SssclientContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateModel(SssclientContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        [BindProperty]
        public bool HourlyRateWasChanged { get; set; }

        public SelectList ClientList { get; set; } = default!;
        public SelectList CustomerList { get; set; } = default!;
        public SelectList StatusList { get; set; } = default!;
        public SelectList SiteList { get; set; } = default!;
        public SelectList UserList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            await PopulateDropdownsAsync();
            Ticket.DateLogged = DateTime.Today;
            Ticket.StatusRec = 1; // Default to Open
            Ticket.AssignedToUserId = _currentUserService.UserId;
            HourlyRateWasChanged = false;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Ticket.ClientRecNavigation");
            ModelState.Remove("Ticket.CustomerRecNavigation");
            ModelState.Remove("Ticket.StatusRecNavigation");
            ModelState.Remove("Ticket.AssignedToUser");

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

            UserList = new SelectList(
                await _context.AppUsers
                    .Where(u => u.IsActive)
                    .OrderByDescending(u => _currentUserService.UserId.HasValue && u.UserId == _currentUserService.UserId.Value)
                    .ThenBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ThenBy(u => u.Email)
                    .ToListAsync(),
                "UserId", "FullName", Ticket.AssignedToUserId ?? _currentUserService.UserId);
        }
    }
}
