using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Sites
{
    public class DeleteModel : PageModel
    {
        private readonly SssclientContext _context;

        public DeleteModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Site Site { get; set; } = new Site();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var site = await _context.Sites
                .Include(s => s.ClientRecNavigation)
                .FirstOrDefaultAsync(s => s.SiteRec == id);

            if (site == null)
                return NotFound();

            Site = site;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var site = await _context.Sites.FindAsync(Site.SiteRec);
            if (site != null)
            {
                _context.Sites.Remove(site);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
