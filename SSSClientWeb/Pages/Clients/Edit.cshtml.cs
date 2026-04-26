using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly SssclientContext _context;

        public EditModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Client Client { get; set; } = new Client();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            Client = client;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clients.Any(c => c.ClientRec == Client.ClientRec))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }
    }
}
