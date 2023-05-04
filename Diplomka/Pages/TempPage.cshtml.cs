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
        public string TypePage { get; set; }
        public TempPageModel(DiplomDBContext context)
        {
            this._context = context;
        }
        public bool IsVyvodData = false;
        public IActionResult OnPost(string tempdata1, string tempdata2, string tempdata3)
        {
            var auth = _context.Authentications.FirstOrDefault(a => a.Name == tempdata1 && a.Surname == tempdata2 && a.LoginName == tempdata3);
            if (auth != null)
            {
                IsVyvodData = true;
                return Page();
            }
            return Page();
        }
    }
}
