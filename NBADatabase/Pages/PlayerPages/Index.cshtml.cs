using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.PlayerPages;

public class IndexModel : PageModel
{
    private readonly NBADbContext _context;

    public IndexModel(NBADbContext context)
    {
        _context = context;
    }

    public IList<Player> Player { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Player = await _context.Players.Include(p => p.Team).ToListAsync();
    }
}
