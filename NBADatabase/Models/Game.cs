using System.ComponentModel.DataAnnotations.Schema;

namespace NBADatabase.Models
{
    public class Game
    {
        public int Id { get; set; }
        
        public int HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }
        public int HomePoints { get; set; }
        
        public int AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }
        public int AwayPoints { get; set; }

        public DateOnly GameDate {  get; set; }

        [NotMapped]
        public string? Winner => (HomeTeam != null && AwayTeam != null) ? ((HomePoints > AwayPoints) ? HomeTeam.Name : AwayTeam.Name) : null;
    }
}
