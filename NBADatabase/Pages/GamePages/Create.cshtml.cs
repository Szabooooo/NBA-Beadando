using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBADatabase.Models;
using NBADatabase.Data;

namespace NBADatabase.Pages.GamePages;

public class CreateModel : PageModel
{
    private readonly NBADbContext _context;
    public SelectList TeamOptions { get; set; } = default!;

    public CreateModel(NBADbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadTeamsAsync();
        return Page();
    }

    [BindProperty]
    public Game Game { get; set; } = new();

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

        _context.Games.Add(Game);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
