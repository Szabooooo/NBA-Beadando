using Microsoft.EntityFrameworkCore;

namespace NBADatabase.Data
{
    // Recalculates season stats from the games in the database.
    // Called after a game is created, edited or deleted, for the players and teams that game involved.
    public static class StatsCalculator
    {
        public static async Task RecalculatePlayersAsync(NBADbContext context, IEnumerable<int> playerIds)
        {
            var ids = playerIds.Distinct().ToList();
            var players = await context.Players
                .Include(p => p.GameStats)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            foreach (var player in players)
            {
                var lines = player.GameStats;
                int points = lines.Sum(s => s.Points);
                int fga = lines.Sum(s => s.FieldGoalsAttempted);
                int fta = lines.Sum(s => s.FreeThrowsAttempted);

                player.Ppg = Average(lines.Select(s => s.Points));
                player.Rpg = Average(lines.Select(s => s.Rebounds));
                player.Apg = Average(lines.Select(s => s.Assists));
                player.Spg = Average(lines.Select(s => s.Steals));
                player.Bpg = Average(lines.Select(s => s.Blocks));
                player.Bpm = Average(lines.Select(s => s.PlusMinus));

                player.Fgp = Percentage(lines.Sum(s => s.FieldGoalsMade), fga);
                player.Tpp = Percentage(lines.Sum(s => s.ThreePointersMade), lines.Sum(s => s.ThreePointersAttempted));
                player.Ftp = Percentage(lines.Sum(s => s.FreeThrowsMade), fta);

                // True shooting: PTS / (2 * (FGA + 0.44 * FTA))
                player.Tsp = Percentage(points, 2 * (fga + 0.44 * fta));
            }

            await context.SaveChangesAsync();
        }

        public static async Task RecalculateTeamsAsync(NBADbContext context, IEnumerable<int> teamIds)
        {
            var ids = teamIds.Distinct().ToList();
            var teams = await context.Team.Where(t => ids.Contains(t.Id)).ToListAsync();
            var games = await context.Games
                .Where(g => ids.Contains(g.HomeTeamId) || ids.Contains(g.AwayTeamId))
                .ToListAsync();

            foreach (var team in teams)
            {
                var played = games.Where(g => g.HomeTeamId == team.Id || g.AwayTeamId == team.Id).ToList();
                team.Wins = played.Count(g => g.HomeTeamId == team.Id
                    ? g.HomePoints > g.AwayPoints
                    : g.AwayPoints > g.HomePoints);
                team.Losses = played.Count - team.Wins;
            }

            await context.SaveChangesAsync();
        }

        private static double Average(IEnumerable<int> values)
        {
            var list = values.ToList();
            return list.Count == 0 ? 0 : Math.Round(list.Average(), 1);
        }

        private static double Percentage(double made, double attempted)
        {
            return attempted == 0 ? 0 : Math.Round(made / attempted * 100, 1);
        }
    }
}
