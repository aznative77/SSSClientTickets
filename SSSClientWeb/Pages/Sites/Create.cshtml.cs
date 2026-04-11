using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Sites
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
    }
}
