using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PDFReaderClassLibrary;
using PythonCaller;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using System.Text;
using Diplomka.Models;
using EncryptorLib;
using System.Net.Mail;
using System.Net;

namespace Diplomka.Pages
{
    public class AdminPageModel : PageModel
    {
        private IWebHostEnvironment _environment;

        private readonly string _dbConnection;

        private readonly DiplomDBContext _context;

        public List<ENTAVGPredictions> AVGPrediction { get; set; }

        public List<ENTMINPredictions> MINPrediction { get; set; }

        public List<Resume> Resumes { get; set; }

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        [BindProperty]
        public string? Year { get; set; }

        public string TypePage { get; set; }

        public bool IsYearExist { get; set; } = false;

        public bool IsSuccess { get; set; } = false;

        public AdminPageModel(IWebHostEnvironment enviroment, DiplomDBContext context)
        {
            this._environment = enviroment;
            this._dbConnection = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";
            this._context = context;
        }

        public void OnGet(string namePage)
        {
            this.TypePage = namePage;

            if (namePage == "Download")
            {
                AVGPrediction = _context.ENTAVGPredictions.ToList();
                MINPrediction = _context.ENTMINPredictions.ToList();
            }

            if (namePage == "Resumes")
            {
                Resumes = _context.Resumes.ToList();
            }
        }

        public async Task<IActionResult> OnPostCreatePredictions()
        {
            string path = this._environment.ContentRootPath + "\\backUpFiles\\" + "backUpPoints.csv";
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
            }

            WriteFile(path);

            DeleteAllPredictions();

            await PyFileCaller.CallPythonFileAsync();

            OnGet("FillPoints");
            return Page();
        }

        public IActionResult OnPostDownload()
        {
            string path = this._environment.ContentRootPath + "\\backUpFiles\\" + "PresentPoints.csv";
            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                file.Delete();
            }

            WriteFile(path);
            OnGet("Download");
            return PhysicalFile(path, "application/octet-stream", "PredictionPoints.csv");
        }
        public async Task<IActionResult> OnPost()
        {
            if (UploadFile != null && !string.IsNullOrEmpty(Year)) 
            {
                string path = this._environment.ContentRootPath + "\\uploads\\" + UploadFile.FileName;
                Match match = (new Regex(@"\d{4}")).Match(Year);
                bool isExistingYear = IsExistingYear(this._dbConnection);
                var file = Path.Combine(this._environment.ContentRootPath, "uploads", UploadFile.FileName);

                if (match.Success && !isExistingYear)
                {
                    using (FileStream fileStream = new FileStream(file, FileMode.Create)) 
                    {
                        UploadFile.CopyTo(fileStream);
                    }

                    await PDFReader.CreateOrdersInDBByPDFText(path, Year, this._dbConnection);
                    IsSuccess = true;
                }
                else if (isExistingYear)
                {
                    IsYearExist = true;
                }

                FileInfo fileInfo = new FileInfo(path);

                if (fileInfo.Exists) 
                {
                    fileInfo.Delete();
                }
            }
            OnGet("FillPoints");
            return Page();
        }

        public async Task<IActionResult> OnPostSendResumes()
        {

            StringBuilder sbMailMessage = new StringBuilder("<h2>Резюме потенциальных работников</h2><ul>");

            List<Resume> resumes = _context.Resumes.ToList();

            foreach(Resume resume in resumes)
            {
                sbMailMessage.Append($"<li>Имя: {resume.AuthorName}, Email: {resume.Email}</li>");
            }

            sbMailMessage.Append("</ul>");

            List<HREmployees> hrEmployees = _context.HREmployees.ToList();

            foreach(HREmployees hr in hrEmployees)
            {
                string? authId = _context.Users.FirstOrDefault(u => u.Id == hr.UserId).AuthenticationId;

                string email = _context.Authentications.FirstOrDefault(a => a.Id == authId).Email;

                if (email != null)
                {
                    MailAddress fromUser = new MailAddress("robotaident@yandex.ru", "Administrator");

                    MailAddress toUser = new MailAddress(email);

                    MailMessage mail = new MailMessage(fromUser, toUser);

                    mail.Subject = "Новые резюме";
                    mail.Body = sbMailMessage.ToString();
                    mail.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp.yandex.ru";
                    client.Port = 25;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(fromUser.Address, "gzjonkouzkxyahje");

                    await client.SendMailAsync(mail);
                }
            }
            
            foreach(Resume resume in resumes)
            {
                _context.Resumes.Remove(resume);
                await _context.SaveChangesAsync();
            }

            OnGet("Resumes");
            return Page();
        }

        private bool IsExistingYear(string connectionString)
        {
            string querySelect = "SELECT DISTINCT(Year) FROM ENTScoresByYear WHERE Year = '" + Year + "'"; 

            using(SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand(querySelect, sqlConnection);
                SqlDataReader reader = sqlCommand.ExecuteReader();

                return reader.HasRows;
            }
        }

        private void DeleteAllPredictions()
        {
            List<Task> tasks = new List<Task>();

            string queryDeleteAVG = "DELETE FROM ENTAVGPredictions;";
            string queryDeleteMIN = "DELETE FROM ENTMINPredictions;";

            tasks.Add(DeletePredictionsFromTable(queryDeleteAVG));
            tasks.Add(DeletePredictionsFromTable(queryDeleteMIN));

            Task.WaitAll(tasks.ToArray());
        }

        private async Task DeletePredictionsFromTable(string query)
        {
            using (SqlConnection connection = new SqlConnection(this._dbConnection))
            {
                await connection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand(query, connection);
                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        private void WriteFile(string path)
        {
            string queryAVGPoints = "SELECT Id, AVGPrediction, Code, Year FROM ENTAVGPredictions;";
            string queryMINPoints = "SELECT Id, MINPrediction, Code, Year FROM ENTMINPredictions;";

            List<Task> tasks = new List<Task>();

            tasks.Add(WritePredictionsByTable(path, queryAVGPoints, "AVG"));
            tasks.Add(WritePredictionsByTable(path, queryMINPoints, "MIN"));

            Task.WaitAll(tasks.ToArray());


        }

        public async Task WritePredictionsByTable(string path, string query, string typeTable)
        {
            StringBuilder sbResult = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(this._dbConnection))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                sbResult.Append(typeTable + "\nId, AVGPrediction, Code, Year\n");

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        sbResult.Append($"{reader.GetValue(0).ToString()}, {reader.GetValue(1).ToString()}," +
                                        $" {reader.GetValue(2).ToString()}, {reader.GetValue(3).ToString()}\n");
                    }
                }
            }

            using (FileStream fsStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] buffer = Encoding.Default.GetBytes(sbResult.ToString());
                fsStream.Seek(0, SeekOrigin.End);
                await fsStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
