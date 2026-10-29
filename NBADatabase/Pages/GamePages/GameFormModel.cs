using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

// The box score table of one team in the game form
public record BoxScoreTable(string Side, List<PlayerGameStat> Lines);

// Shared code of the Create and Edit pages: team dropdowns and the two box score tables
public abstract class GameFormModel : PageModel
{
    protected readonly NBADbContext _context;
    public SelectList TeamOptions { get; set; } = default!;

    protected GameFormModel(NBADbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Game Game { get; set; } = new();

    [BindProperty]
    public List<PlayerGameStat> HomeLines { get; set; } = new();

    [BindProperty]
    public List<PlayerGameStat> AwayLines { get; set; } = new();

    // Called by the form's script when a team is selected, returns that team's box score rows
    public async Task<IActionResult> OnGetLinesAsync(int teamId, string side)
    {
        var lines = await BuildLinesAsync(teamId, Enumerable.Empty<PlayerGameStat>());
        return Partial("_BoxScoreForm", new BoxScoreTable(side == "Away" ? "Away" : "Home", lines));
    }

    protected async Task LoadTeamsAsync()
    {
        var teams = await _context.Team.OrderBy(x => x.Name).ToListAsync();
        TeamOptions = new SelectList(teams, "Id", "Name");
    }

    // One row for every player of the team; rows that already have stats in this game are marked as played
    protected async Task<List<PlayerGameStat>> BuildLinesAsync(int teamId, IEnumerable<PlayerGameStat> existing)
    {
        var lines = existing.Where(s => s.TeamId == teamId).ToList();
        lines.ForEach(s => s.Played = true);

        var roster = await _context.Players.Where(p => p.TeamId == teamId).ToListAsync();
        foreach (var player in roster.Where(p => lines.All(s => s.PlayerId != p.Id)))
        {
            lines.Add(new PlayerGameStat { PlayerId = player.Id, Player = player, TeamId = teamId });
        }

        return lines.OrderBy(s => s.Player?.Name).ToList();
    }

    // After a failed post the rows only contain ids, so the player names are loaded again
    protected async Task RestoreFormAsync()
    {
        await LoadTeamsAsync();
        var ids = HomeLines.Concat(AwayLines).Select(s => s.PlayerId).ToList();
        var players = await _context.Players.Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
        foreach (var line in HomeLines.Concat(AwayLines))
        {
            line.Player = players.GetValueOrDefault(line.PlayerId);
        }
    }

    // The rows marked as played become the game's box score
    protected List<PlayerGameStat> PlayedLines()
    {
        var home = HomeLines.Where(s => s.Played).ToList();
        home.ForEach(s => s.TeamId = Game.HomeTeamId);
        var away = AwayLines.Where(s => s.Played).ToList();
        away.ForEach(s => s.TeamId = Game.AwayTeamId);

        var lines = home.Concat(away).ToList();
        lines.ForEach(s => { s.Id = 0; s.GameId = Game.Id; s.Player = null; });
        return lines;
    }

    // Only the stats of played rows matter, so errors on the other rows are ignored
    protected void IgnoreUnplayedRows()
    {
        foreach (var (prefix, lines) in new[] { ("HomeLines", HomeLines), ("AwayLines", AwayLines) })
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var row = $"{prefix}[{i}]";
                if (!lines[i].Played)
                {
                    foreach (var key in ModelState.Keys.Where(k => k == row || k.StartsWith(row + ".")).ToList())
                    {
                        ModelState.Remove(key);
                    }
                }
            }
        }
    }
}
