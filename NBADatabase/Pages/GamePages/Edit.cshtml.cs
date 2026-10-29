using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

public class EditModel : GameFormModel
{
    public EditModel(NBADbContext context) : base(context)
    {
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var game = await _context.Games
            .Include(g => g.PlayerStats).ThenInclude(s => s.Player)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (game is null)
        {
            return NotFound();
        }
        Game = game;
        HomeLines = await BuildLinesAsync(game.HomeTeamId, game.PlayerStats);
        AwayLines = await BuildLinesAsync(game.AwayTeamId, game.PlayerStats);
        await LoadTeamsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        IgnoreUnplayedRows();
        if (!ModelState.IsValid)
        {
            await RestoreFormAsync();
            return Page();
        }

        var existing = await _context.Games
            .Include(g => g.PlayerStats)
            .FirstOrDefaultAsync(g => g.Id == Game.Id);
        if (existing is null)
        {
            return NotFound();
        }

        // Everyone in the old or the new version of the game needs their stats recalculated
        var lines = PlayedLines();
        var playerIds = existing.PlayerStats.Select(s => s.PlayerId).Concat(lines.Select(s => s.PlayerId)).ToList();
        var teamIds = new[] { existing.HomeTeamId, existing.AwayTeamId, Game.HomeTeamId, Game.AwayTeamId };

        existing.GameDate = Game.GameDate;
        existing.HomeTeamId = Game.HomeTeamId;
        existing.HomePoints = Game.HomePoints;
        existing.AwayTeamId = Game.AwayTeamId;
        existing.AwayPoints = Game.AwayPoints;

        // The box score is replaced with the rows from the form
        _context.PlayerStats.RemoveRange(existing.PlayerStats);
        _context.PlayerStats.AddRange(lines);
        await _context.SaveChangesAsync();

        await StatsCalculator.RecalculatePlayersAsync(_context, playerIds);
        await StatsCalculator.RecalculateTeamsAsync(_context, teamIds);

        return RedirectToPage("./Index");
    }
}
