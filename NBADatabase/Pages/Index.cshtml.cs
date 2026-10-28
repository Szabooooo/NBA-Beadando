using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NBADatabase.Data;
using NBADatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace NBADatabase.Pages
{
    public class IndexModel : PageModel
    {
        private readonly NBADbContext _context;
        public IndexModel(NBADbContext context) => _context = context;
        public List<Team> Teams { get; set; } = new();
        //public void OnGet()
        //{

        //}

        public async Task OnGetAsync()
        {
            Teams = await _context.Team.ToListAsync();
        }
    }
}
