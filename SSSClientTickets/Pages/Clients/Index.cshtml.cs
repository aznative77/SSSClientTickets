using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly SssclientContext _context;

        public IndexModel(SssclientContext context)
        {
            _context = context;
        }

        public IList<Client> Clients { get; set; } = new List<Client>();

        public async Task OnGetAsync()
        {
            Clients = await _context.Clients
                .OrderBy(c => c.ClientName)
                .ToListAsync();
        }
    }
}
