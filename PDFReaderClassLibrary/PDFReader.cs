using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;

namespace PDFReaderClassLibrary
{
    public class PDFReader
    {
        public static async Task<string> CreateOrdersInDBByPDFText(string path, string connectionString)
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
                    await CreateOrdersByEditedText(textWithPoints, match.Groups[0].Value, connectionString);



                }
                else if (match.Success)
                {
                    await CreateOrdersByEditedText(resultString.Substring(0, resultString.Length), match.Groups[0].Value, connectionString);
                }

                match = match.NextMatch();
            }

            return "OK";
        }

        protected static async Task CreateOrdersByEditedText(string editedText, string code, string connectionString)
        {
            string tmpText = editedText;
            string patternIdIncoming = @"\d{9}";
            Regex regex = new Regex(patternIdIncoming);
            Match match = regex.Match(tmpText);

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
                        //string connectionString = "Server=(localdb)\\MSSQLLocalDB; Database=DBDiplom; Trusted_Connection=True;";
                        string insertQuery = $"INSERT INTO ENTScoresByYear(Id, Points, Code, Year) VALUES('{Guid.NewGuid().ToString().ToUpper()}', {number}, '{code}', '2019')";
                        await SQLHelper.InsertDataIntoDb(connectionString, insertQuery);
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
    }
}
