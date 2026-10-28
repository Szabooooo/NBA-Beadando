using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBADatabase.Models
{
    public class Team
    {
        public int Id { get; set; }
        public List<Player> Players { get; set; } = new();

        [Required(ErrorMessage = "Team name must be set!"), Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        
        [NotMapped]
        public double PCT => Wins + Losses == 0 ? 0 : (double)Wins / (Losses + Wins);

    }
}
