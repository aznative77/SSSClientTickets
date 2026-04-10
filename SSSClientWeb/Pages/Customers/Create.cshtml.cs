using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly SssclientContext _context;

        public CreateModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        public SelectList ClientList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ClientList = new SelectList(
                await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                "ClientRec",
                "ClientName"
            );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove ClientRecNavigation from validation since it's a nav property
            ModelState.Remove("Customer.ClientRecNavigation");

            if (!ModelState.IsValid)
            {
                ClientList = new SelectList(
                    await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                    "ClientRec",
                    "ClientName"
                );
                return Page();
            }

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
