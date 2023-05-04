using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace PDFReaderClassLibrary
{
    public class PDFReader
    {
        public static async Task CreateOrdersInDBByPDFText(string path, string year, string connectionString)
        {
            PdfReader pdfReader = new PdfReader(path);
            StringBuilder sbInitPdfText = new StringBuilder();

            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                sbInitPdfText.Append(PdfTextExtractor.GetTextFromPage(pdfReader, page));
            }

            pdfReader.Close();

            string resultString = sbInitPdfText.ToString(); //Not edited text
            string deletePartSubstring = "Список обладателей образовательных грантов по квоте для детей-сирот и детей, оставшихся без";

            if (resultString.IndexOf(deletePartSubstring) != -1)
            {
                resultString = resultString.Substring(0, resultString.IndexOf(deletePartSubstring)); // delete info about orphans, disabled and e.t.c
            }

            //Find education code matches in main text
            string patternForEducationCode = @"B\d{3}";
            Regex regexForEducationCode = new Regex(patternForEducationCode);
            Match match = regexForEducationCode.Match(resultString);

            while (true)
            {   
                if (!match.Success)
                {
                    break;
                }

                if (match.Success && match.NextMatch().Success)
                {
                    //Find first code in text and cut
                    int clipper = resultString.IndexOf(match.Groups[0].Value);
                    resultString = resultString.Substring(clipper);

                    //Find row with '№' and cut the text
                    clipper = resultString.IndexOf("№");
                    resultString = resultString.Substring(clipper);

                    //Find next education code and get text with points defined education code
                    clipper = resultString.IndexOf(match.NextMatch().Groups[0].Value);
                    string textWithPoints = resultString.Substring(0, clipper);
                    string codeName = match.Groups[0].Value;
                    await CreateOrdersByEditedText(textWithPoints, match.Groups[0].Value, year, connectionString);



                }
                else if (match.Success)
                {
                    await CreateOrdersByEditedText(resultString.Substring(0, resultString.Length), match.Groups[0].Value, year, connectionString);
                }

                match = match.NextMatch();
            }
        }

        protected static async Task CreateOrdersByEditedText(string editedText, string code, string year, string connectionString)
        {
            string tmpText = editedText;
            string patternIdIncoming = @"\d{9}";
            Regex regex = new Regex(patternIdIncoming);
            Match match = regex.Match(tmpText);

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT s.SubjectName ");
            sbQuery.Append("FROM EducationProgram ep ");
            sbQuery.Append("INNER JOIN Subjects s ON s.Id = ep.SubjId ");
            sbQuery.Append($"WHERE ep.EducationCodeId = (SELECT Id FROM EducationPrCode WHERE CodeName = '{code}') ");

            string subject = GetSubject(connectionString, sbQuery.ToString());

            while(match.Success)
            {
                string id = match.Groups[0].Value;
                int clipper = tmpText.IndexOf(id);
                tmpText = tmpText.Substring(clipper + 10);

                while(true)
                {
                    string possibleNumber = tmpText.Substring(0, tmpText.IndexOf(" "));
                    int number;

                    bool isNumber = int.TryParse(possibleNumber, out number);

                    if (isNumber)
                    {
                        if ((subject == "Творческий экзамен/Творческий экзамен" && number >= 35) || number >= 65)
                        {
                            string insertQuery = $"INSERT INTO ENTScoresByYear(Id, Points, Code, Year) VALUES('{Guid.NewGuid().ToString().ToUpper()}', {number}, '{code}', '{year}')";
                            await SQLHelper.InsertDataIntoDb(connectionString, insertQuery);
                        }
                        break;
                    }
                    else
                    {
                        tmpText = tmpText.Substring(possibleNumber.Length + 1);
                    }
                }

                match = match.NextMatch();
            }
        }

        protected static string GetSubject(string connectionString, string query)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }

                reader.Close();
            }

            return null;
        }
    }
}
