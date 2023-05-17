using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diplomka.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Diplomka.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DiplomDBContext _context;
        private readonly string _dbConnection = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";
        public IndexModel(DiplomDBContext context)
        {
            this._context = context;
        }
        public List<SelectListItem> Options { get; set; }
        public string SelectedSubject { get; set; }
        public string Points { get; set; }
        public void OnGet()
        {
            Options = _context.Subjects.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Number,
                                      Text = a.SubjectName
                                  }).ToList();
        }
        
        public IActionResult OnPost(string SelectedSubject, string points)
        {
            return RedirectToPage("AnalyzePage", new { number = SelectedSubject, points = points });
        }
    }
}