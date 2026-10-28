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

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var team = await _context.Team.FirstOrDefaultAsync(m => m.Id == id);
        if (team is null)
        {
            return NotFound();
        }
        else
        {
            Team = team;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var team = await _context.Team.FindAsync(id);
        if (team != null)
        {
            Team = team;
            _context.Team.Remove(Team);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
