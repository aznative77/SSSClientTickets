using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SssclientContext _context;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public LoginModel(SssclientContext context)
    {
        _context = context;
    }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string? ReturnUrl { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var normalizedEmail = Email.Trim().ToLowerInvariant();
        var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "This account is currently inactive. Please contact the admin.");
            return Page();
        }

        if (!user.IsApproved)
        {
            ModelState.AddModelError(string.Empty, "This account is waiting for admin approval.");
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.FullName) ? user.Email : user.FullName),
            new(ClaimTypes.Email, user.Email)
        };

        if (user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        _context.ChangeLogs.Add(new ChangeLog
        {
            EntityName = "User",
            EntityRecordId = user.UserId,
            Action = "Signed In",
            Description = $"{user.FullName} signed in",
            UserId = user.UserId,
            ChangedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();

        return LocalRedirect(ReturnUrl ?? Url.Page("/Tickets/Index")!);
    }
}
