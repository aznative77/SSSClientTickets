using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SssclientContext _context;

    public LogoutModel(SssclientContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.Identity?.Name ?? "Unknown user";

        if (int.TryParse(userIdValue, out var userId))
        {
            var userExists = await _context.AppUsers.FindAsync(userId) != null;
            _context.ChangeLogs.Add(new ChangeLog
            {
                EntityName = "User",
                EntityRecordId = userId,
                Action = "Signed Out",
                Description = $"{userName} signed out",
                UserId = userExists ? userId : null,
                ChangedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Account/Login");
    }
}
