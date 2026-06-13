using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;
using SSSClientTickets.Services;

namespace SSSClientTickets.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SssclientContext _context;
        private readonly ICurrentUserService _currentUserService;

        public IndexModel(ILogger<IndexModel> logger, SssclientContext context, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _context = context;
            _currentUserService = currentUserService;
        }

        public IList<Ticket> OpenTickets { get; set; } = new List<Ticket>();

        public async Task OnGetAsync()
        {
            var openStatuses = new[] { "Open", "In Progress", "Waiting for Client" };
            OpenTickets = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .Include(t => t.StatusRecNavigation)
                .Where(t => openStatuses.Contains(t.StatusRecNavigation.Status)
                    && t.AssignedToUserId == _currentUserService.UserId)
                .OrderByDescending(t => t.DateLogged)
                .ToListAsync();
        }
    }
}
