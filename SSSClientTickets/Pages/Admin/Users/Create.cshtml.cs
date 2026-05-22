using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly SssclientContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public CreateModel(SssclientContext context)
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

    [BindProperty, Display(Name = "Approved")]
    public bool IsApproved { get; set; } = true;

    [BindProperty, Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [BindProperty, Display(Name = "Admin")]
    public bool IsAdmin { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedEmail = Email.Trim().ToLowerInvariant();
        if (await _context.AppUsers.AnyAsync(u => u.Email == normalizedEmail))
        {
            ModelState.AddModelError(nameof(Email), "An account already exists for this email.");
            return Page();
        }

        var user = new AppUser
        {
            FirstName = FirstName.Trim(),
            LastName = LastName.Trim(),
            Email = normalizedEmail,
            IsApproved = IsApproved,
            IsActive = IsActive,
            IsAdmin = IsAdmin
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, Password);

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"{user.FullName} was added.";
        return RedirectToPage("Index");
    }
}
