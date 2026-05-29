using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Sites
{
    public class CreateModel : PageModel
    {
        private readonly SssclientContext _context;

        public CreateModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Site Site { get; set; } = new Site();

        public SelectList ClientList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ClientList = new SelectList(
                await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                "ClientRec", "ClientName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Site.ClientRecNavigation");

            if (!ModelState.IsValid)
            {
                ClientList = new SelectList(
                    await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                    "ClientRec", "ClientName");
                return Page();
            }

            _context.Sites.Add(Site);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAjaxAsync()
        {
            ModelState.Remove("Site.ClientRecNavigation");
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Site.ClientRecNavigation", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return new JsonResult(new { success = false, message = string.Join(" ", errors) });
            }

            _context.Sites.Add(Site);
            await _context.SaveChangesAsync();

            return new JsonResult(new { 
                success = true, 
                siteRec = Site.SiteRec, 
                siteName = Site.SiteName 
            });
        }
    }
}
