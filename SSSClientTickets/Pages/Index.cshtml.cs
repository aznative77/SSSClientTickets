using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SssclientContext _context;

        public IndexModel(ILogger<IndexModel> logger, SssclientContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Ticket> OpenTickets { get; set; } = new List<Ticket>();

        public async Task OnGetAsync()
        {
            var openStatuses = new[] { "Open", "In Progress", "Waiting for Client" };
            OpenTickets = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .Include(t => t.StatusRecNavigation)
                .Where(t => openStatuses.Contains(t.StatusRecNavigation.Status))
                .OrderByDescending(t => t.DateLogged)
                .ToListAsync();
        }
    }
}
