using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Diplomka.Pages
{
    public class LogAndRegPageModel : PageModel
    {
        public string TypePage { get; set; }
        public IActionResult OnGet(string namePage) 
        {
            if (namePage == "Log" || namePage == "Reg" || namePage == "Forget")
            {
                TypePage = namePage;
                return Page();
            }
            else 
            {
                return RedirectToPage("Index");
            }
        }
        public IActionResult OnPostLog() 
        {
            return RedirectToPage("Index");
        }

        public IActionResult OnPostReg() 
        {
            return RedirectToPage("Index");
        }

        public IActionResult OnPostForget() 
        {
            var urlPageLog = Url.Page("LogAndRegPage", new { namePage = "Log" });

            if (urlPageLog != null) 
            {
                return Redirect(urlPageLog);
            }

            return RedirectToPage("Index"); 
        }
    }
}
