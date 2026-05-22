using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SSSClientTickets.Models;

namespace SSSClientTickets.Pages.ChangeLogs;

public class IndexModel : PageModel
{
    private readonly SssclientContext _context;

    public IndexModel(SssclientContext context)
    {
        _context = context;
    }

    public IList<ChangeLog> ChangeLogs { get; set; } = new List<ChangeLog>();

    public async Task OnGetAsync()
    {
        ChangeLogs = await _context.ChangeLogs
            .Include(c => c.User)
            .OrderByDescending(c => c.ChangedAt)
            .Take(250)
            .ToListAsync();
    }
}
