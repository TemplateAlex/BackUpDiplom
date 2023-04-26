using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diplomka.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Diplomka.Pages
{
    public class TempPageModel : PageModel
    {
        private readonly DiplomDBContext _context;
        public TempPageModel(DiplomDBContext context)
        {
            this._context = context;
        }

        public void OnGet()
        {
        }
    }
}
