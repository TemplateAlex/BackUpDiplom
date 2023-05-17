using Diplomka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Diplomka.Pages
{
    public class AnalyzePageModel : PageModel
    {
        private readonly string _dbConnection;

        private readonly DiplomDBContext _context;

        [BindProperty(SupportsGet = true)]
        public string Number { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Points { get; set; }

        public List<ENTPrediction> entPredictions = new List<ENTPrediction>();

        public string SubjectName { get; set; }

        public AnalyzePageModel(DiplomDBContext context)
        {
            this._dbConnection = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";
            this._context = context;
        }
        public void OnGet()
        {
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT CodeName, Description " +
                           "FROM EducationPrCode " +
                           "WHERE Id IN (SELECT EducationCodeId " +
                           "             FROM EducationProgram " +
                           "             WHERE SubjId = (SELECT Id" +
            "                             FROM Subjects" +
                           "                             WHERE Number = '" + Number + "'));");

            using (SqlConnection sqlConnection = new SqlConnection(this._dbConnection))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(sbQuery.ToString(), sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string codeName = reader.GetValue(0).ToString();
                        string description = reader.GetValue(1).ToString();
                        int prediction = 0;
                        int point = Convert.ToInt32(Points);

                        StringBuilder sbQueryAVG = new StringBuilder();
                        sbQueryAVG.Append($"SELECT AVGPrediction FROM ENTAVGPredictions WHERE Code = '{codeName}'");
                        int avgPrediction = GetPointsByCodeName(sbQueryAVG.ToString());

                        StringBuilder sbQueryMIN = new StringBuilder();
                        sbQueryMIN.Append($"SELECT MinPrediction FROM ENTMINPredictions WHERE Code = '{codeName}'");
                        int minPrediction = GetPointsByCodeName(sbQueryMIN.ToString());

                        if (point >= minPrediction && point <= avgPrediction)
                        {
                            prediction = (int)(((float)point / (float)avgPrediction) * 100);
                        }
                        else if (point > avgPrediction)
                        {
                            prediction = 100;
                        }

                        if (prediction != 0 && entPredictions.Count < 5) 
                        {
                            entPredictions.Add(new ENTPrediction() { CodeName = codeName, Description = description, Prediction = prediction });
                        }
                     
                    }
                }
                reader.Close();

                string query = "SELECT SubjectName FROM Subjects WHERE Number = '" + Number + "'";

                SubjectName = GetSubjectByNumber(query);

                entPredictions.Sort(delegate (ENTPrediction x, ENTPrediction y) {
                    return y.Prediction.CompareTo(x.Prediction);
                });
            }
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
                    while (reader.Read())
                    {
                        point = Convert.ToInt32(reader.GetValue(0));
                    }
                }
            }

            return point;
        }

        private string GetSubjectByNumber(string query) 
        {
            using (SqlConnection sqlConnection = new SqlConnection(this._dbConnection)) 
            {
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) 
                {
                    if (reader.Read()) 
                    {
                        return reader.GetString(0);
                    }
                }
            }

            return null;
        }
    }

    public class ENTPrediction 
    {
        public string CodeName { get; set; }
        public string Description { get; set; }
        public int Prediction { get; set; }
    }
}
