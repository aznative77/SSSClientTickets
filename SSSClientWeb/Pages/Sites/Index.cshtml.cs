using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Sites
{
    public class IndexModel : PageModel
    {
        private readonly SssclientContext _context;

        public IndexModel(SssclientContext context)
        {
            _context = context;
        }

        public IList<Site> Sites { get; set; } = new List<Site>();

        public async Task OnGetAsync()
        {
            Sites = await _context.Sites
                .Include(s => s.ClientRecNavigation)
                .OrderBy(s => s.ClientRecNavigation.ClientName)
                .ThenBy(s => s.SiteName)
                .ToListAsync();
        }
    }
}
