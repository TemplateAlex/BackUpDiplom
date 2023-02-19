using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Diplomka.Pages
{
    public class LogAndRegPageModel : PageModel
    {
        public string TypePage { get; set; }
        public IActionResult OnGet(string namePage) 
        {
            if (namePage == "Log" || namePage == "Reg")
            {
                TypePage = namePage;
                return Page();
            }
            else 
            {
                return RedirectToPage("Index");
            }
        }
    }
}
