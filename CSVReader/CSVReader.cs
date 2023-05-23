using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSVReader
{
    public interface IActionCSV
    {
        void Execute(string path);
    }

    public class ActionWriterSql : IActionCSV
    {
        private readonly string _connectionString;

        public ActionWriterSql(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public void Execute(string path)
        {
            StringBuilder sbBackUpString = new StringBuilder();
            using (FileStream fsStream = File.OpenRead(path))
            {
                byte[] buffer = new byte[fsStream.Length];

                fsStream.Read(buffer, 0, buffer.Length);

                string text = Encoding.Default.GetString(buffer);
                Console.WriteLine(text);

            }
        } 

        private async Task WriteOrdersIntoTable(string something)
        {
            await Task.Run(() => {
                Console.WriteLine(something);
            
            });
        }
    }

    public class ActionWriterFile : IActionCSV
    {
        private readonly string _connectionString;

        public ActionWriterFile(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public void Execute(string path)
        {
            string queryAVGPoints = "SELECT Id, AVGPrediction, Code, Year FROM ENTAVGPredictions;";
            string queryMINPoints = "SELECT Id, MINPrediction, Code, Year FROM ENTMINPredictions;";

            List<Task> tasks = new List<Task>();

            tasks.Add(WritePredictionsFromTable(path, queryAVGPoints, "AVG"));
            tasks.Add(WritePredictionsFromTable(path, queryMINPoints, "MIN"));

            Task.WaitAll(tasks.ToArray());
        }

        public async Task WritePredictionsFromTable(string path, string query, string typeTable)
        {
            StringBuilder sbResult = new StringBuilder();

            using (SqlConnection sqlConnection = new SqlConnection(this._connectionString))
            {
                sqlConnection.Open();

                SqlCommand command = new SqlCommand(query, sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                sbResult.Append(typeTable + "\nId, AVGPrediction, Code, Year\n");

                if (reader.HasRows)
                {
                    while (reader.Read())
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

    public class CSVReader
    {
        private IActionCSV _actionCSV;

        public void SetAction(IActionCSV actionCSV)
        {
            this._actionCSV = actionCSV;
        }

        public void Execute(string path)
        {
            if (this._actionCSV != null)
            {
                this._actionCSV.Execute(path);
            }

            throw new Exception("You need to initialized action");
        }
    }
}
