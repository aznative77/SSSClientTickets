using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientWeb.Models;

namespace SSSClientWeb.Pages.Customers
{
    public class DeleteModel : PageModel
    {
        private readonly SssclientContext _context;

        public DeleteModel(SssclientContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.ClientRecNavigation)
                .FirstOrDefaultAsync(c => c.CustomerRec == id);

            if (customer == null)
                return NotFound();

            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var customer = await _context.Customers.FindAsync(Customer.CustomerRec);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
