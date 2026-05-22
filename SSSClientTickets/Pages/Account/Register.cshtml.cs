using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SssclientContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public RegisterModel(SssclientContext context)
    {
        _context = context;
    }

    [BindProperty, Required, Display(Name = "First Name"), StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty, Required, Display(Name = "Last Name"), StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [BindProperty, Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required, DataType(DataType.Password), MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedEmail = Email.Trim().ToLowerInvariant();
        var existingUser = await _context.AppUsers.AnyAsync(u => u.Email == normalizedEmail);
        if (existingUser)
        {
            ModelState.AddModelError(nameof(Email), "An account already exists for this email.");
            return Page();
        }

        var isFirstUser = !await _context.AppUsers.AnyAsync();
        var user = new AppUser
        {
            FirstName = FirstName.Trim(),
            LastName = LastName.Trim(),
            Email = normalizedEmail,
            IsAdmin = isFirstUser,
            IsApproved = isFirstUser
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, Password);

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = isFirstUser
            ? "Account created. You can log in now."
            : "Account created. An admin must approve it before you can access the site.";

        return RedirectToPage("/Account/Login");
    }
}
