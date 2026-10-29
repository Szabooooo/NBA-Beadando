using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NBADatabase.Data;
using NBADatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace NBADatabase.Pages
{
    // A stat the leaders board can be sorted by
    public record LeaderStat(string Key, string Label, string Unit, Func<Player, double> Value);

    // One row of the leaders board
    public record Leader(int Rank, Player Player, double Value, int GamesPlayed);

    public class IndexModel : PageModel
    {
        private readonly NBADbContext _context;
        public IndexModel(NBADbContext context) => _context = context;

        public static readonly List<LeaderStat> Stats = new()
        {
            new("points", "Points", "PPG", p => p.Ppg),
            new("rebounds", "Rebounds", "RPG", p => p.Rpg),
            new("assists", "Assists", "APG", p => p.Apg),
            new("steals", "Steals", "SPG", p => p.Spg),
            new("blocks", "Blocks", "BPG", p => p.Bpg),
            new("plusminus", "+/-", "+/-", p => p.Bpm),
        };

        [BindProperty(SupportsGet = true)]
        public string Stat { get; set; } = "points";

        public LeaderStat SelectedStat { get; set; } = Stats[0];
        public List<Leader> Leaders { get; set; } = new();
        public List<Team> Teams { get; set; } = new();
        public List<Game> RecentGames { get; set; } = new();
        public int PlayerCount { get; set; }
        public int GameCount { get; set; }

        public async Task OnGetAsync()
        {
            SelectedStat = Stats.FirstOrDefault(s => s.Key == Stat) ?? Stats[0];

            var players = await _context.Players.Include(p => p.Team).ToListAsync();
            var gamesPlayed = await _context.PlayerStats
                .GroupBy(s => s.PlayerId)
                .Select(g => new { PlayerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PlayerId, x => x.Count);

            // Top 10 players in the selected stat; players without that stat are left out
            Leaders = players
                .Where(p => SelectedStat.Value(p) != 0)
                .OrderByDescending(p => SelectedStat.Value(p))
                .ThenBy(p => p.Name)
                .Take(10)
                .Select((p, i) => new Leader(i + 1, p, SelectedStat.Value(p), gamesPlayed.GetValueOrDefault(p.Id)))
                .ToList();

            var teams = await _context.Team.ToListAsync();
            Teams = teams.OrderByDescending(t => t.PCT).ThenByDescending(t => t.Wins).ThenBy(t => t.Name).ToList();

            RecentGames = await _context.Games
                .Include(g => g.HomeTeam)
                .Include(g => g.AwayTeam)
                .OrderByDescending(g => g.GameDate).ThenByDescending(g => g.Id)
                .Take(5)
                .ToListAsync();

            PlayerCount = players.Count;
            GameCount = await _context.Games.CountAsync();
        }
    }
}
