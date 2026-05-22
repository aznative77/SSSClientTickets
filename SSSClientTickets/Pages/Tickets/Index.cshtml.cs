using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;
using SSSClientTickets.Services;

namespace SSSClientTickets.Pages.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly SssclientContext _context;
        private readonly ICurrentUserService _currentUserService;

        public IndexModel(SssclientContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public IList<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ClientOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CustomerOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UserOptions { get; set; } = new List<SelectListItem>();

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public int[]? FilterStatusRec { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterClientRec { get; set; }

        [BindProperty(SupportsGet = true)]
        public int[]? FilterCustomerRec { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterCreatedByUserId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterResolvedByUserId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDateLoggedFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDateLoggedTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDateResolvedFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDateResolvedTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDateBilledFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDateBilledTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool FilterNotYetBilled { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterIssueSearch { get; set; }

        public async Task OnGetAsync()
        {
            // Load filter options
            var statuses = await _context.TicketStatuses.ToListAsync();
            StatusOptions = statuses
                .Select(s => new SelectListItem(s.Status, s.StatusRec.ToString()))
                .ToList();

            var clients = await _context.Clients.ToListAsync();
            ClientOptions = new SelectListItem[] { new SelectListItem("-- All Clients --", "") }
                .Concat(clients
                    .OrderBy(c => c.ClientName)
                    .Select(c => new SelectListItem(c.ClientName, c.ClientRec.ToString())))
                .ToList();

            // Load customers filtered by selected client if applicable
            var customersQuery = _context.Customers.AsQueryable();
            if (FilterClientRec.HasValue && FilterClientRec > 0)
            {
                customersQuery = customersQuery.Where(c => c.ClientRec == FilterClientRec);
            }
            var customers = await customersQuery.ToListAsync();
            CustomerOptions = customers
                .OrderBy(c => c.CustomerName)
                .Select(c => new SelectListItem(c.CustomerName, c.CustomerRec.ToString()))
                .ToList();

            var currentUserId = _currentUserService.UserId;
            var users = await _context.AppUsers
                .Where(u => u.IsActive)
                .ToListAsync();
            UserOptions = new SelectListItem[] { new SelectListItem("-- All Users --", "") }
                .Concat(users
                    .OrderByDescending(u => currentUserId.HasValue && u.UserId == currentUserId.Value)
                    .ThenBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ThenBy(u => u.Email)
                    .Select(u => new SelectListItem(
                        currentUserId.HasValue && u.UserId == currentUserId.Value
                            ? $"{u.FullName} (Me)"
                            : u.FullName,
                        u.UserId.ToString())))
                .ToList();

            // Build query
            var query = _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .Include(t => t.StatusRecNavigation)
                .Include(t => t.CreatedByUser)
                .Include(t => t.ResolvedByUser)
                .AsQueryable();

            // Apply filters
            if (FilterStatusRec != null && FilterStatusRec.Length > 0)
            {
                query = query.Where(t => FilterStatusRec.Contains(t.StatusRec));
            }

            if (FilterClientRec.HasValue && FilterClientRec > 0)
            {
                query = query.Where(t => t.ClientRec == FilterClientRec);
            }

            if (FilterCustomerRec != null && FilterCustomerRec.Length > 0)
            {
                query = query.Where(t => FilterCustomerRec.Contains(t.CustomerRec));
            }

            if (FilterCreatedByUserId.HasValue && FilterCreatedByUserId > 0)
            {
                query = query.Where(t => t.CreatedByUserId == FilterCreatedByUserId);
            }

            if (FilterResolvedByUserId.HasValue && FilterResolvedByUserId > 0)
            {
                query = query.Where(t => t.ResolvedByUserId == FilterResolvedByUserId);
            }

            if (FilterDateLoggedFrom.HasValue)
            {
                query = query.Where(t => t.DateLogged >= FilterDateLoggedFrom);
            }

            if (FilterDateLoggedTo.HasValue)
            {
                // Include the entire day
                var nextDay = FilterDateLoggedTo.Value.AddDays(1);
                query = query.Where(t => t.DateLogged < nextDay);
            }

            if (FilterDateResolvedFrom.HasValue)
            {
                query = query.Where(t => t.DateResolved >= FilterDateResolvedFrom);
            }

            if (FilterDateResolvedTo.HasValue)
            {
                // Include the entire day
                var nextDay = FilterDateResolvedTo.Value.AddDays(1);
                query = query.Where(t => t.DateResolved < nextDay);
            }

            var hasDateBilledRangeFilter = FilterDateBilledFrom.HasValue || FilterDateBilledTo.HasValue;
            if (hasDateBilledRangeFilter || FilterNotYetBilled)
            {
                // Keep Date Billed as one grouped filter: date range OR not billed.
                var dateBilledBefore = FilterDateBilledTo?.AddDays(1);
                query = query.Where(t =>
                    (hasDateBilledRangeFilter
                        && (!FilterDateBilledFrom.HasValue || t.DateBilled >= FilterDateBilledFrom)
                        && (!dateBilledBefore.HasValue || t.DateBilled < dateBilledBefore))
                    || (FilterNotYetBilled && t.DateBilled == null));
            }

            if (!string.IsNullOrWhiteSpace(FilterIssueSearch))
            {
                var searchTerm = FilterIssueSearch.ToLower();
                query = query.Where(t => t.Issue != null && t.Issue.ToLower().Contains(searchTerm));
            }

            Tickets = await query
                .OrderByDescending(t => t.DateLogged)
                .ThenByDescending(t => t.TicketRec)
                .ToListAsync();
        }
    }
}
