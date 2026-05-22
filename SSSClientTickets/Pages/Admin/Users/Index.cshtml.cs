using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly SssclientContext _context;

    public IndexModel(SssclientContext context)
    {
        _context = context;
    }

    public IList<AppUser> Users { get; set; } = new List<AppUser>();

    public int? CurrentUserId { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadUsersAsync();
    }

    public async Task<IActionResult> OnPostToggleAdminAsync(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (id == GetCurrentUserId())
        {
            StatusMessage = "You cannot change your own admin role.";
            return RedirectToPage();
        }

        if (user.IsAdmin && user.IsActive && user.IsApproved && await _context.AppUsers.CountAsync(u => u.IsAdmin && u.IsActive && u.IsApproved) <= 1)
        {
            StatusMessage = "At least one active approved admin is required.";
            return RedirectToPage();
        }

        user.IsAdmin = !user.IsAdmin;
        await _context.SaveChangesAsync();

        StatusMessage = $"{user.FullName} is now {(user.IsAdmin ? "an admin" : "not an admin")}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleApprovedAsync(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (id == GetCurrentUserId())
        {
            StatusMessage = "You cannot change your own approval status.";
            return RedirectToPage();
        }

        if (user.IsApproved && user.IsAdmin && await _context.AppUsers.CountAsync(u => u.IsAdmin && u.IsActive && u.IsApproved) <= 1)
        {
            StatusMessage = "At least one active approved admin is required.";
            return RedirectToPage();
        }

        user.IsApproved = !user.IsApproved;
        await _context.SaveChangesAsync();

        StatusMessage = $"{user.FullName} is now {(user.IsApproved ? "approved" : "pending approval")}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (id == GetCurrentUserId())
        {
            StatusMessage = "You cannot deactivate your own account.";
            return RedirectToPage();
        }

        if (user.IsActive && user.IsAdmin && user.IsApproved && await _context.AppUsers.CountAsync(u => u.IsAdmin && u.IsActive && u.IsApproved) <= 1)
        {
            StatusMessage = "At least one active approved admin is required.";
            return RedirectToPage();
        }

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();

        StatusMessage = $"{user.FullName} is now {(user.IsActive ? "active" : "inactive")}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        if (id == GetCurrentUserId())
        {
            StatusMessage = "You cannot delete your own account.";
            return RedirectToPage();
        }

        if (user.IsAdmin && user.IsActive && user.IsApproved && await _context.AppUsers.CountAsync(u => u.IsAdmin && u.IsActive && u.IsApproved) <= 1)
        {
            StatusMessage = "At least one active approved admin is required.";
            return RedirectToPage();
        }

        _context.AppUsers.Remove(user);

        try
        {
            await _context.SaveChangesAsync();
            StatusMessage = $"{user.FullName} was deleted.";
        }
        catch (DbUpdateException)
        {
            _context.Entry(user).State = EntityState.Unchanged;
            StatusMessage = $"{user.FullName} could not be deleted because they are connected to existing tickets, time entries, or change logs. Mark them inactive instead.";
        }

        return RedirectToPage();
    }

    private async Task LoadUsersAsync()
    {
        CurrentUserId = GetCurrentUserId();
        Users = await _context.AppUsers
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ThenBy(u => u.Email)
            .ToListAsync();
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }
}
