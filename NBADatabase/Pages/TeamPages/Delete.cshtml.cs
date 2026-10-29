using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.TeamPages;

public class DeleteModel : PageModel
{
    private readonly NBADbContext _context;

    public DeleteModel(NBADbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Team Team { get; set; } = default!;

    // Games reference teams with DeleteBehavior.Restrict, so a team that has played can't be deleted
    public int GameCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        return await LoadTeamAsync(id.Value) ? Page() : NotFound();
    }

    private async Task<bool> LoadTeamAsync(int id)
    {
        var team = await _context.Team.Include(t => t.Players).FirstOrDefaultAsync(m => m.Id == id);
        if (team is null)
        {
            return false;
        }

        Team = team;
        GameCount = await _context.Games.CountAsync(g => g.HomeTeamId == id || g.AwayTeamId == id);
        return true;
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        if (!await LoadTeamAsync(id.Value))
        {
            return RedirectToPage("./Index");
        }

        if (GameCount > 0)
        {
            ModelState.AddModelError(string.Empty, "This team has games recorded. Delete its games first.");
            return Page();
        }

        // The team's players are removed with it (cascade delete)
        _context.Team.Remove(Team);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
