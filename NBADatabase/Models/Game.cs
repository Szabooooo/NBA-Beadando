using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBADatabase.Models
{
    public class Game : IValidatableObject
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Home team must be selected!"), Display(Name = "Home team")]
        public int HomeTeamId { get; set; }
        public Team? HomeTeam { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Points can't be negative!"), Display(Name = "Home points")]
        public int HomePoints { get; set; }
        
        [Required(ErrorMessage = "Away team must be selected!"), Display(Name = "Away team")]
        public int AwayTeamId { get; set; }
        public Team? AwayTeam { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Points can't be negative!"), Display(Name = "Away points")]
        public int AwayPoints { get; set; }

        [Display(Name = "Date")]
        public DateOnly GameDate {  get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [NotMapped, Display(Name = "Winner")]
        public string? Winner => (HomeTeam != null && AwayTeam != null) ? ((HomePoints > AwayPoints) ? HomeTeam.Name : AwayTeam.Name) : null;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HomeTeamId == AwayTeamId)
            {
                yield return new ValidationResult("A team can't play against itself!", new[] { nameof(AwayTeamId) });
            }

            if (HomePoints == AwayPoints)
            {
                yield return new ValidationResult("NBA games can't end in a tie!", new[] { nameof(AwayPoints) });
            }
        }
    }
}
