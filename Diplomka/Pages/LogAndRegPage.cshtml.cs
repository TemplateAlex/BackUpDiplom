using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diplomka.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Diplomka.Pages
{
    public class LogAndRegPageModel : PageModel
    {
        private readonly DiplomDBContext _context;
        public string TypePage { get; set; }
        public bool isErrExistUser { get; set; }
        public bool isErrPsswdEqual { get; set; }
        public bool isErrUserLog { get; set; }
        public bool isErrRole { get; set; }

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
            var auth = _context.Authentications.FirstOrDefault(a => a.LoginName == loginLog && a.Password == psswdLog);
            if (auth != null) 
            {
                string roleId = _context.Users.FirstOrDefault(u => u.AuthenticationId == auth.Id).RoleId;
                string roleName = _context.Roles.FirstOrDefault(r => r.Id == roleId).RoleName;

                HttpContext.Session.SetString("Login", loginLog);
                HttpContext.Session.SetString("role", roleName);
                return RedirectToPage("Index");
            }
            isErrUserLog = true;
            return Page();
        }

        public async Task<IActionResult> OnPostReg(string nameReg, string surnameReg, string emailReg, string loginReg, string psswdReg, string rpsswdReg) 
        {
            var auth = _context.Authentications.FirstOrDefault(a => a.LoginName == loginReg);
            if (string.IsNullOrEmpty(psswdReg))
            {
                Console.WriteLine("Xyi ty che tut proverish");
                return Page();
            }
            if (psswdReg.Equals(rpsswdReg))
            {
                string patternIdIncoming = @"{\d{1}}";
                Regex regex = new Regex(patternIdIncoming);
                Match match = regex.Match(nameReg);
                if (string.IsNullOrEmpty(nameReg) || match.Success)
                {
                    return Page();
                }
                match = regex.Match(surnameReg);
                if (string.IsNullOrEmpty(surnameReg)|| match.Success)
                {
                    Console.WriteLine("хуйня твоя фамилия, переделывай");
                    return Page();
                }
                patternIdIncoming = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
                regex = new Regex(patternIdIncoming);
                match = regex.Match(emailReg);
                if (string.IsNullOrEmpty(emailReg) || !match.Success)
                {
                    Console.WriteLine("я тебе блять не почтовый голубь, нормально имэйл пиши");
                    return Page();
                }
                if (string.IsNullOrEmpty(loginReg))
                {
                    Console.WriteLine("ты долбаеб хуйня логин");
                    return Page();
                }
                if(string.IsNullOrEmpty(psswdReg))
                {
                    Console.WriteLine("минимум 4 символа, еблан");
                    return Page();
                }
                    //красава авторизуем тебя
                if (auth == null)
                {
                    string authenId = Guid.NewGuid().ToString().ToUpper();
                    Authentications authen = new Authentications();
                    Users user = new Users();
                    authen.Id = authenId;
                    authen.Name = nameReg;
                    authen.Surname = surnameReg;
                    authen.Email = emailReg;
                    authen.LoginName = loginReg;
                    authen.Password = psswdReg;
                    user.Id = Guid.NewGuid().ToString().ToUpper();
                    user.AuthenticationId = authenId;
                    user.RoleId = _context.Roles.FirstOrDefault(r => r.RoleName == "User").Id;
                    _context.Authentications.Add(authen);
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    isErrExistUser = true;
                    return Page();
                }
               
            }
            else
            {
                isErrPsswdEqual = true;
                return Page();
            }
            return RedirectToPage("Index");
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
