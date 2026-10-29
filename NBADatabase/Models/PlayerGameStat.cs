using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using NBADatabase.Infrastructure;

namespace NBADatabase.Models
{
    // One player's box score line in one game
    public class PlayerGameStat : IValidatableObject
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }

        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        // The team the player played for in this game (kept even if the player is traded later)
        public int TeamId { get; set; }
        public Team? Team { get; set; }

        // Only used by the game form: rows that weren't marked as played are not saved
        [NotMapped]
        public bool Played { get; set; }

        [Range(0, 200), Display(Name = "PTS"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int Points { get; set; }

        [Range(0, 100), Display(Name = "REB"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int Rebounds { get; set; }

        [Range(0, 100), Display(Name = "AST"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int Assists { get; set; }

        [Range(0, 100), Display(Name = "STL"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int Steals { get; set; }

        [Range(0, 100), Display(Name = "BLK"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int Blocks { get; set; }

        [Range(0, 100), Display(Name = "FGM"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int FieldGoalsMade { get; set; }

        [Range(0, 100), Display(Name = "FGA"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int FieldGoalsAttempted { get; set; }

        [Range(0, 100), Display(Name = "3PM"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int ThreePointersMade { get; set; }

        [Range(0, 100), Display(Name = "3PA"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int ThreePointersAttempted { get; set; }

        [Range(0, 100), Display(Name = "FTM"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int FreeThrowsMade { get; set; }

        [Range(0, 100), Display(Name = "FTA"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int FreeThrowsAttempted { get; set; }

        [Range(-100, 100), Display(Name = "+/-"), ModelBinder(typeof(EmptyAsZeroModelBinder))]
        public int PlusMinus { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FieldGoalsMade > FieldGoalsAttempted)
            {
                yield return new ValidationResult("FGM can't be more than FGA!", new[] { nameof(FieldGoalsMade) });
            }

            if (ThreePointersMade > ThreePointersAttempted)
            {
                yield return new ValidationResult("3PM can't be more than 3PA!", new[] { nameof(ThreePointersMade) });
            }

            if (ThreePointersAttempted > FieldGoalsAttempted || ThreePointersMade > FieldGoalsMade)
            {
                yield return new ValidationResult("Three pointers are field goals too, so 3PM/3PA can't be more than FGM/FGA!", new[] { nameof(ThreePointersMade) });
            }

            if (FreeThrowsMade > FreeThrowsAttempted)
            {
                yield return new ValidationResult("FTM can't be more than FTA!", new[] { nameof(FreeThrowsMade) });
            }

            // Every made field goal is worth 2, a three adds 1 more, and a free throw is worth 1
            int expectedPoints = 2 * FieldGoalsMade + ThreePointersMade + FreeThrowsMade;
            if (Points != expectedPoints)
            {
                yield return new ValidationResult(
                    $"PTS must match the made shots: 2 × {FieldGoalsMade} FGM + {ThreePointersMade} 3PM + {FreeThrowsMade} FTM = {expectedPoints}!",
                    new[] { nameof(Points) });
            }
        }
    }
}
