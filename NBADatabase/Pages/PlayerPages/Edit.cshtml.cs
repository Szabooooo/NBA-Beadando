using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.PlayerPages;

public class EditModel : PageModel
{
    private readonly NBADbContext _context;
    public SelectList TeamOptions { get; set; } = default!;

    public EditModel(NBADbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Player Player { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var player = await _context.Players.FirstOrDefaultAsync(m => m.Id == id);
        if (player is null)
        {
            return NotFound();
        }
        Player = player;
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

        _context.Attach(Player).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlayerExists(Player.Id))
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

    private bool PlayerExists(int id)
    {
        return _context.Players.Any(e => e.Id == id);
    }
}
