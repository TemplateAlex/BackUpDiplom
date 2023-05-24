using Diplomka.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Diplomka.Pages
{
    public class AboutUsModel : PageModel
    {
        private readonly DiplomDBContext _context;
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Message { get; set; }

        [BindProperty]
        public DateTime Date { get; set; }

        public AboutUsModel(DiplomDBContext context)
        {
            this._context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSendForm()
        {
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Message))
            {
                Resume resume = new Resume();

                resume.Id = Guid.NewGuid().ToString().ToUpper();
                resume.AuthorName = Name;
                resume.Email = Email;
                resume.About = Message;
                resume.Birthday = Date;

                _context.Resumes.Add(resume);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
