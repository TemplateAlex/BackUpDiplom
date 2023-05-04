using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PDFReaderClassLibrary;
using PythonCaller;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace Diplomka.Pages
{
    public class AdminPageModel : PageModel
    {
        private IWebHostEnvironment _environment;
        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        [BindProperty]
        public string? Year { get; set; }

        public string TypePage { get; set; }

        public bool IsYearExist { get; set; } = false;

        public bool IsSuccess { get; set; } = false;

        public AdminPageModel(IWebHostEnvironment enviroment)
        {
            this._environment = enviroment;
        }

        public void OnGet(string namePage)
        {
            this.TypePage = namePage;
        }

        public async Task<IActionResult> OnPostCreatePredictions()
        {
            await PyFileCaller.CallPythonFileAsync();

            OnGet("FillPoints");
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";

            if (UploadFile != null && !string.IsNullOrEmpty(Year)) 
            {
                string path = this._environment.ContentRootPath + "\\uploads\\" + UploadFile.FileName;
                Match match = (new Regex(@"\d{4}")).Match(Year);
                bool isExistingYear = IsExistingYear(connectionString);
                var file = Path.Combine(this._environment.ContentRootPath, "uploads", UploadFile.FileName);

                if (match.Success && !isExistingYear)
                {
                    using (FileStream fileStream = new FileStream(file, FileMode.Create)) 
                    {
                        UploadFile.CopyTo(fileStream);
                    }

                    await PDFReader.CreateOrdersInDBByPDFText(path, Year, connectionString);
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

        private async Task DeletePointsByYear(string connectionString, string year)
        {
            string deleteQuery = $"DELETE FROM ENTScoresByYear WHERE ENTYear = '{year}'";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString)) 
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand(deleteQuery, sqlConnection);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
