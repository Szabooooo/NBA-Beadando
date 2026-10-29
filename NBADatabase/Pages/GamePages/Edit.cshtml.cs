using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

public class EditModel : PageModel
{
    private readonly NBADbContext _context;
    public SelectList TeamOptions { get; set; } = default!;

    public EditModel(NBADbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Game Game { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var game = await _context.Games.FirstOrDefaultAsync(m => m.Id == id);
        if (game is null)
        {
            return NotFound();
        }
        Game = game;
        await LoadTeamsAsync();
        return Page();
    }

    private async Task LoadTeamsAsync()
    {
        var teams = await _context.Team.OrderBy(x => x.Name).ToListAsync();
        TeamOptions = new SelectList(teams, "Id", "Name");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadTeamsAsync();
            return Page();
        }

        _context.Attach(Game).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!GameExists(Game.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool GameExists(int id)
    {
        return _context.Games.Any(e => e.Id == id);
    }
}
