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
        public static async Task InsertDataIntoDb(string connectionString, string insertQuery)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString)) 
            {
                await sqlConnection.OpenAsync();

                SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlConnection);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        public static async Task DeleteDataFromDB(string connectionString, string deleteQuery) 
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString)) 
            {
                await sqlConnection.OpenAsync();
                SqlCommand sqlCommand = new SqlCommand(deleteQuery, sqlConnection);
                await sqlCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
