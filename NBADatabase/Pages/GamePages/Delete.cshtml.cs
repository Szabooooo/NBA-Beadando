using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

public class DeleteModel : PageModel
{
    private readonly NBADbContext _context;

    public DeleteModel(NBADbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var game = await _context.Games.Include(g => g.PlayerStats).FirstOrDefaultAsync(g => g.Id == id);
        if (game != null)
        {
            var playerIds = game.PlayerStats.Select(s => s.PlayerId).ToList();

            // The box score lines are deleted with the game (cascade delete)
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            await StatsCalculator.RecalculatePlayersAsync(_context, playerIds);
            await StatsCalculator.RecalculateTeamsAsync(_context, new[] { game.HomeTeamId, game.AwayTeamId });
        }

        return RedirectToPage("./Index");
    }
}
