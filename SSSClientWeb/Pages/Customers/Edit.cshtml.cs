using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Customers
{
    public class EditModel : PageModel
    {
        private readonly SssclientContext _context;

        public EditModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        public SelectList ClientList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            Customer = customer;

            ClientList = new SelectList(
                await _context.Clients.OrderBy(c => c.ClientName).ToListAsync(),
                "ClientRec",
                "ClientName",
                Customer.ClientRec
            );

            return Page();
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
                    "ClientName",
                    Customer.ClientRec
                );
                return Page();
            }

            _context.Attach(Customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(c => c.CustomerRec == Customer.CustomerRec))
                    return NotFound();
                throw;
            }

            return RedirectToPage("Index");
        }
    }
}
