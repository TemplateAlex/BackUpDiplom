using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diplomka.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;

namespace Diplomka.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DiplomDBContext _context;
        public IndexModel(DiplomDBContext context)
        {
            this._context = context;
        }
        public List<SelectListItem> Options { get; set; }
        public string SelectedSubject { get; set; }
        public void OnGet()
        {
            Options = _context.Subjects.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id,
                                      Text = a.SubjectName
                                  }).ToList();
            Console.WriteLine(Options.Count);
        }
        
        public IActionResult OnPost(string SelectedSubject, string points)
        {
            this.SelectedSubject = SelectedSubject;
            Console.WriteLine(SelectedSubject);
            OnGet();
            return Page();
        }
    }
}