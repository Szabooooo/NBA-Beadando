using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.PlayerPages;

public class DetailsModel : PageModel
{
    private readonly NBADbContext _context;
    public DetailsModel(NBADbContext context)
    {
        _context = context;
    }

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
        else
        {
            Player = player;
        }

        return Page();
    }
}
