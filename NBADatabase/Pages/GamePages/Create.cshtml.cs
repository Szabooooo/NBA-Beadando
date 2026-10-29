using Microsoft.AspNetCore.Mvc;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

public class CreateModel : GameFormModel
{
    public CreateModel(NBADbContext context) : base(context)
    {
    }

    public async Task<IActionResult> OnGetAsync()
    {
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

        var lines = PlayedLines();
        Game.PlayerStats = lines;
        _context.Games.Add(Game);
        await _context.SaveChangesAsync();

        await StatsCalculator.RecalculatePlayersAsync(_context, lines.Select(s => s.PlayerId));
        await StatsCalculator.RecalculateTeamsAsync(_context, new[] { Game.HomeTeamId, Game.AwayTeamId });

        return RedirectToPage("./Index");
    }
}
