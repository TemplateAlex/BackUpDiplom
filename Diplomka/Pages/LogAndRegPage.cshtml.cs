using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diplomka.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using EncryptorLib;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Diplomka.Pages
{
    public class LogAndRegPageModel : PageModel
    {
        private readonly DiplomDBContext _context;
        public LogAndRegPageModel(DiplomDBContext context)
        {
            this._context = context;
            this.md5proxy = new md5Proxy();
        }

        private md5Proxy md5proxy;
        public string TypePage { get; set; }
        public bool isErrExistUser { get; set; }
        public bool isErrPsswdEqual { get; set; }
        public bool isErrUserLog { get; set; }
        public bool isErrRole { get; set; }
        
        public IActionResult OnGet(string namePage) 
        {
            if (namePage == "Log" || namePage == "Reg" || namePage == "Forget")
            {
                TypePage = namePage;
                return Page();
            }
            if (namePage == "Out")
            {
                TypePage = namePage;
                HttpContext.Session.Clear();
            }
            if (HttpContext.Session.Keys.Count() != 0) {
                return RedirectToPage("Index");
                
            }
            else
            {
                return RedirectToPage("Index");
            }
        }
        public IActionResult OnPostLog(string loginLog, string psswdLog) 
        {
            string hashpassword = md5proxy.HashPassword(psswdLog);
            var auth = _context.Authentications.FirstOrDefault(a => a.LoginName == loginLog && a.Password == hashpassword);

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
            string hashpassword = md5proxy.HashPassword(psswdReg);
            if (hashpassword == null)
            {
                Console.WriteLine("password not valid");
                return Page();
            }
            if (psswdReg.Equals(rpsswdReg))
            {
                string patternIdIncoming = @"[0-9]+";
                Regex regex = new Regex(patternIdIncoming);
                Match match = regex.Match(nameReg);
                if (string.IsNullOrEmpty(nameReg) || match.Success)
                {
                    Console.WriteLine("переделывайте имя");
                    return Page();
                }
                match = regex.Match(surnameReg);
                if (string.IsNullOrEmpty(surnameReg)|| match.Success)
                {
                    Console.WriteLine("переделывайте фамилию");
                    return Page();
                }
                patternIdIncoming = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
                regex = new Regex(patternIdIncoming);
                match = regex.Match(emailReg);
                if (string.IsNullOrEmpty(emailReg) || !match.Success)
                {
                    Console.WriteLine("нормально имэйл пишите");
                    return Page();
                }
                if (string.IsNullOrEmpty(loginReg))
                {
                    Console.WriteLine("заполните поле");
                    return Page();
                }
                    //authorizing
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
                    authen.Password = hashpassword;
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

        public async Task<IActionResult> OnPostForget(string emailForget) 
        {

            Authentications? userAuth = _context.Authentications.FirstOrDefault(a => a.Email == emailForget);

            if (userAuth != null)
            {
                string newPassword = CreateNewPassword();
                MailAddress fromUser = new MailAddress("robotaident@yandex.ru", "Administrator");

                MailAddress toUser = new MailAddress(emailForget);

                MailMessage mail = new MailMessage(fromUser, toUser);

                mail.Subject = "Дорогой пользователь, не забывай пароль";
                mail.Body = $"<h2>Прошу запомнить ваш новый пароль!</h2><p>Ваш новый пароль: {newPassword}</p>";
                mail.IsBodyHtml = true;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.yandex.ru";
                client.Port = 25;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(fromUser.Address, "gzjonkouzkxyahje");
                await client.SendMailAsync(mail);

                userAuth.Password = md5proxy.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return RedirectToPage("LogAndRegPage", new { namePage = "Log" });
            }
            return Page(); 
        }
        
        private string CreateNewPassword()
        {
            StringBuilder sbPassword = new StringBuilder();

            for(int i = 0; i < 8; i++)
            {
                Random random = new Random();
                sbPassword.Append((char)random.Next(35, 123));
            }

            return sbPassword.ToString();
        }
    }
}
