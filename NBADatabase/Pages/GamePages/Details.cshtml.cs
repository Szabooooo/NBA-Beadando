using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

public class DetailsModel : PageModel
{
    private readonly NBADbContext _context;
    public DetailsModel(NBADbContext context)
    {
        _context = context;
    }

    public Game Game { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var game = await _context.Games
            .Include(g => g.HomeTeam)
            .Include(g => g.AwayTeam)
            .Include(g => g.PlayerStats).ThenInclude(s => s.Player)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (game is null)
        {
            return NotFound();
        }
        else
        {
            Game = game;
        }

        return Page();
    }
}
