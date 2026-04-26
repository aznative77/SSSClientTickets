using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Sites
{
    public class EditModel : PageModel
    {
        private readonly SssclientContext _context;

        public EditModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Site Site { get; set; } = new Site();

        public SelectList ClientList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site == null)
                return NotFound();

            Site = site;

            ClientList = new SelectList(
                await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                "ClientRec", "ClientName", Site.ClientRec);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Site.ClientRecNavigation");

            if (!ModelState.IsValid)
            {
                ClientList = new SelectList(
                    await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                    "ClientRec", "ClientName", Site.ClientRec);
                return Page();
            }

            _context.Attach(Site).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Sites.Any(s => s.SiteRec == Site.SiteRec))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }
    }
}
