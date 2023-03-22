using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Diplomka.Models;

namespace Diplomka.Pages
{
    public class LogAndRegPageModel : PageModel
    {
        private readonly DiplomDBContext _context;
        public string? TypePage { get; set; }

        //Properties for block "Log"
        public string ErrorWithLogPAge { get; set; }

        //Properties for block "Reg"
        public string ErrorWithRegPage { get; set; }

        //Properties for block "Forget"
        public string ErrorWithForgetPage { get; set; }


        public LogAndRegPageModel(DiplomDBContext context) 
        {
            this._context = context;
        }
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
        public IActionResult OnPostLog(string loginLog, string psswdLog) 
        {
            Authentications? userAuth = _context.Authentications.FirstOrDefault(a => a.LoginName == loginLog && a.Password == psswdLog);

            if (userAuth != null) 
            {
                return RedirectToPage("Index");
            }



            return Page();
        }

        public IActionResult OnPostReg(string nameReg, string surnameReg, string emailReg, string loginReg, string psswdReg, string rpsswdReg) 
        {

            Authentications? auth = _context.Authentications.FirstOrDefault(a => a.Email != emailReg && a.LoginName != loginReg);

            if (auth == null && psswdReg == rpsswdReg) 
            {
                Users newUser = new Users();
                Authentications authNewUser = new Authentications();
                
            }

            return Page();
        }

        public IActionResult OnPostForget(string emailForget) 
        {

            Authentications? userAuth = _context.Authentications.FirstOrDefault(a => a.Email == emailForget);

            if (userAuth != null) 
            {

            }

            ErrorWithForgetPage = "Sorry problem with email. Email doesn't correct";
            /*var urlPageLog = Url.Page("LogAndRegPage", new { namePage = "Log" });

            if (urlPageLog != null) 
            {
                return Redirect(urlPageLog);
            }

            return RedirectToPage("Index"); */

            return Page();
        }
    }
}
