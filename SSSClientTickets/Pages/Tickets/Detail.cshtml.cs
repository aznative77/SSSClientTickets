using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Tickets
{
    public class DetailModel : PageModel
    {
        private readonly SssclientContext _context;

        public DetailModel(SssclientContext context)
        {
            _context = context;
        }

        public Ticket Ticket { get; set; } = default!;
        public List<SelectListItem> AssignmentUserOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.ClientRecNavigation)
                .Include(t => t.CustomerRecNavigation)
                .Include(t => t.StatusRecNavigation)
                .Include(t => t.SiteRecNavigation)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Include(t => t.ResolvedByUser)
                .Include(t => t.TicketTimes)
                    .ThenInclude(tt => tt.TimeRecordedByUser)
                .Include(t => t.TicketAttachments)
                    .ThenInclude(a => a.UploadedByUser)
                .FirstOrDefaultAsync(t => t.TicketRec == id);

            if (ticket == null)
                return NotFound();

            Ticket = ticket;
            await PopulateAssignmentUsersAsync(ticket.AssignedToUserId);
            return Page();
        }

        private async Task PopulateAssignmentUsersAsync(int? selectedUserId)
        {
            var users = await _context.AppUsers
                .Where(u => u.IsActive || u.UserId == selectedUserId)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ThenBy(u => u.Email)
                .ToListAsync();

            AssignmentUserOptions = new List<SelectListItem>
            {
                new SelectListItem("-- Unassigned --", "")
            };

            AssignmentUserOptions.AddRange(users.Select(u => new SelectListItem(
                u.FullName,
                u.UserId.ToString(),
                selected: selectedUserId.HasValue && u.UserId == selectedUserId.Value)));
        }

        public async Task<IActionResult> OnPostUpdateAssignedToAsync(int id, int? assignedToUserId)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            if (assignedToUserId.HasValue)
            {
                var assignedToUserExists = await _context.AppUsers
                    .AnyAsync(u => u.UserId == assignedToUserId.Value && u.IsActive);

                if (!assignedToUserExists)
                {
                    ModelState.AddModelError("assignedToUserId", "Assigned user is not active.");
                    await OnGetAsync(id);
                    return Page();
                }
            }

            ticket.AssignedToUserId = assignedToUserId;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostUpdateDateBilledAsync(int id, DateTime? dateBilled)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ticket.DateBilled = dateBilled;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id });
        }
    }
}
