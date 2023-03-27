using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace PDFReaderClassLibrary
{
    internal class SQLHelper
    {
        public static async Task<string> InsertDataIntoDb(string connectionString, string query)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();

                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                    await sqlCommand.ExecuteNonQueryAsync();

                }

                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
