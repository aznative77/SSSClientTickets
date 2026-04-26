using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly SssclientContext _context;

        public DeleteModel(SssclientContext context)
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
            var client = await _context.Clients.FindAsync(Client.ClientRec);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
