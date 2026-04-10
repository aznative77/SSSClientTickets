using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly SssclientContext _context;

        public CreateModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Client Client { get; set; } = new Client();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Clients.Add(Client);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
