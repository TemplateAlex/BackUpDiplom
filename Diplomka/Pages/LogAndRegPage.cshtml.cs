using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diplomka.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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
            if (psswdReg.Equals(rpsswdReg))
            {
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
