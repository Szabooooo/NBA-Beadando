using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.TeamPages;

public class IndexModel : PageModel
{
    private readonly NBADbContext _context;

    public IndexModel(NBADbContext context)
    {
        _context = context;
    }

    public IList<Team> Team { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Team = await _context.Team.ToListAsync();
    }
}
