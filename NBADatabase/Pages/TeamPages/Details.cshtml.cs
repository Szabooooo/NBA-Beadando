using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.TeamPages;

public class DetailsModel : PageModel
{
    private readonly NBADbContext _context;
    public DetailsModel(NBADbContext context)
    {
        _context = context;
    }

    public Team Team { get; set; } = default!;
    public IList<Game> Games { get; set; } = new List<Game>();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var team = await _context.Team
            .Include(t => t.Players.OrderBy(p => p.Name))
            .FirstOrDefaultAsync(m => m.Id == id);
        if (team is null)
        {
            return NotFound();
        }
        else
        {
            Team = team;
        }

        Games = await _context.Games
            .Include(g => g.HomeTeam)
            .Include(g => g.AwayTeam)
            .Where(g => g.HomeTeamId == id || g.AwayTeamId == id)
            .OrderByDescending(g => g.GameDate)
            .ToListAsync();

        return Page();
    }
}
