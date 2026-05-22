using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class ResetPasswordModel : PageModel
{
    private readonly SssclientContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public ResetPasswordModel(SssclientContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int UserId { get; set; }

    public string UserDisplayName { get; set; } = string.Empty;

    [BindProperty, Required, Display(Name = "New Password"), DataType(DataType.Password), MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    [BindProperty, Required, Display(Name = "Confirm Password"), DataType(DataType.Password), Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        UserId = user.UserId;
        UserDisplayName = $"{user.FullName} ({user.Email})";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _context.AppUsers.FindAsync(UserId);
        if (user == null)
        {
            return NotFound();
        }

        UserDisplayName = $"{user.FullName} ({user.Email})";

        if (!ModelState.IsValid)
        {
            return Page();
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, NewPassword);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Password reset for {user.FullName}.";
        return RedirectToPage("Index");
    }
}
