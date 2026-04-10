using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly SssclientContext _context;

        public IndexModel(SssclientContext context)
        {
            _context = context;
        }

        public IList<Customer> Customers { get; set; } = new List<Customer>();

        public async Task OnGetAsync()
        {
            Customers = await _context.Customers
                .Include(c => c.ClientRecNavigation)
                .OrderBy(c => c.ClientRecNavigation.ClientName)
                .ThenBy(c => c.CustomerName)
                .ToListAsync();
        }
    }
}
