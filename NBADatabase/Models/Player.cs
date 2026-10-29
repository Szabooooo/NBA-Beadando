using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using NBADatabase.Infrastructure;

namespace NBADatabase.Models
{
    public class Player
    {
        public int Id {  get; set; }

        [Required(ErrorMessage = "Team must be selected!"), Display(Name = "Team")]
        public int TeamId { get; set; }
        public Team? Team { get; set; }

        public List<PlayerGameStat> GameStats { get; set; } = new();

        [Required(ErrorMessage = "Player name must be set"), Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        // The season stats below are optional when a player is created (empty means 0)
        // and are recalculated from the player's box scores whenever a game they played in changes

        [Display(Name = "PPG"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Ppg {  get; set; }

        [Display(Name = "RPG"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Rpg { get; set; }

        [Display(Name = "APG"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Apg { get; set; }

        [Display(Name = "SPG"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Spg { get; set; }

        [Display(Name = "BPG"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Bpg { get; set; }

        [Display(Name = "FG%"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Fgp { get; set; }

        [Display(Name = "3P%"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Tpp { get; set; }

        [Display(Name = "FT%"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Ftp { get; set; }

        [Display(Name = "TS%"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Tsp { get; set; }

        [Display(Name = "+/-"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public double Bpm { get; set; }
    }
}
