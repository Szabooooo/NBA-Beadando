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
        [Required(ErrorMessage = "City must be set!"), Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Wins can't be negative!"), Display(Name = "Wins")]
        public int Wins { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Losses can't be negative!"), Display(Name = "Losses")]
        public int Losses { get; set; }
        
        [NotMapped, Display(Name = "PCT"), DisplayFormat(DataFormatString = "{0:0.000}")]
        public double PCT => Wins + Losses == 0 ? 0 : (double)Wins / (Losses + Wins);

    }
}
