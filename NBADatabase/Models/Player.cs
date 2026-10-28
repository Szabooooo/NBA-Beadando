using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace NBADatabase.Models
{
    public class Player
    {
        public int Id {  get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; } = new();

        [Required(ErrorMessage = "Player name must be set"), Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "PPG")]
        public double Ppg {  get; set; }

        [Display(Name = "RPG")]
        public double Rpg { get; set; }

        [Display(Name = "APG")]
        public double Apg { get; set; }

        [Display(Name = "SPG")]
        public double Spg { get; set; }

        [Display(Name = "BPG")]
        public double Bpg { get; set; }
        
        [Display(Name = "FG%")]
        public double Fgp { get; set; }

        [Display(Name = "3P%")]
        public double Tpp { get; set; }

        [Display(Name = "FT%")]
        public double Ftp { get; set; }

        [Display(Name = "TS%")]
        public double Tsp { get; set; }

        [Display(Name = "Box+/-")]
        public double Bpm { get; set; }
    }
}
