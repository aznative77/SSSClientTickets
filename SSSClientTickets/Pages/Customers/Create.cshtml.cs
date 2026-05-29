using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Customers
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

        public async Task<IActionResult> OnPostAjaxAsync()
        {
            // Remove ClientRecNavigation from validation since it's a nav property
            ModelState.Remove("Customer.ClientRecNavigation");
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Customer.ClientRecNavigation", StringComparison.OrdinalIgnoreCase)).ToList())
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

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            return new JsonResult(new { 
                success = true, 
                customerRec = Customer.CustomerRec, 
                customerName = Customer.CustomerName 
            });
        }
    }
}
