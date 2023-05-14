using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Diplomka.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Diplomka.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DiplomDBContext _context;
        private readonly string _dbConnection = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";
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
                                      Value = a.Number,
                                      Text = a.SubjectName
                                  }).ToList();
        }
        
        public IActionResult OnPost(string SelectedSubject, string points)
        {
            this.SelectedSubject = SelectedSubject;
            OnGet();
            return Page();
        }

        public IActionResult OnGetAjax(string value, string points)
        {
            
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT CodeName, Description " +
                           "FROM EducationPrCode " +
                           "WHERE Id IN (SELECT EducationCodeId " +
                           "             FROM EducationProgram " +
                           "             WHERE SubjId = (SELECT Id" +
                           "                             FROM Subjects" +
                           "                             WHERE Number = '" + value + "'));");

            using (SqlConnection sqlConnection = new SqlConnection(this._dbConnection)) 
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(sbQuery.ToString(), sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        string codeName = reader.GetValue(0).ToString();
                        string description = reader.GetValue(1).ToString();
                        float prediction = 0;
                        int point = Convert.ToInt32(points);

                        StringBuilder sbQueryAVG = new StringBuilder();
                        sbQueryAVG.Append($"SELECT AVGPrediction FROM ENTAVGPredictions WHERE Code = '{codeName}'");
                        int avgPrediction = GetPointsByCodeName(sbQueryAVG.ToString());

                        StringBuilder sbQueryMIN = new StringBuilder();
                        sbQueryMIN.Append($"SELECT MinPrediction FROM ENTMINPredictions WHERE Code = '{codeName}'");
                        int minPrediction = GetPointsByCodeName(sbQueryMIN.ToString());

                        if (point >= minPrediction && point <= avgPrediction)
                        {
                            prediction = ((float)point / (float)avgPrediction) * 100;
                        }
                        else if (point > avgPrediction) {
                            prediction = 100;
                        }

                        sbResult.Append($"CodeName: {codeName}, Description: {description}, Prediction: {prediction}\n");
                    }
                }

                reader.Close();
            }

            return new JsonResult(sbResult.ToString());
        }

        public int GetPointsByCodeName(string sbQuery)
        {
            int point = 0;
            using (SqlConnection sqlConnection = new SqlConnection(this._dbConnection))
            {
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(sbQuery, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        point = Convert.ToInt32(reader.GetValue(0));
                    }
                }
            }

            return point;
        }
    }
}