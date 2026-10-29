using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.PlayerPages;

public class DeleteModel : PageModel
{
    private readonly NBADbContext _context;

    public DeleteModel(NBADbContext context)
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

        var player = await _context.Players.Include(p => p.Team).FirstOrDefaultAsync(m => m.Id == id);
        if (player is null)
        {
            return NotFound();
        }
        else
        {
            Player = player;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var player = await _context.Players.FindAsync(id);
        if (player != null)
        {
            Player = player;
            _context.Players.Remove(Player);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
