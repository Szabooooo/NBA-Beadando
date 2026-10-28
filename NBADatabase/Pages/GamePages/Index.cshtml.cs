using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

public class IndexModel : PageModel
{
    private readonly NBADbContext _context;

    public IndexModel(NBADbContext context)
    {
        _context = context;
    }

    public IList<Game> Game { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Game = await _context.Games.ToListAsync();
    }
}
