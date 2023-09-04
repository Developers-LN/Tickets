using System.Collections.Generic;
using System.Data.SqlClient;

namespace Tickets.Models.Procedures.Accounting
{
    public class Procedure_Accounting_PaymentAwardNoteCredit
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<RequestResponseModel> Upload_Accounting_PaymentAwardNoteCredit(int @NotaCreditoId)
        {
            var lista = new List<RequestResponseModel>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("Accounting_PaymentAwardNoteCredit", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@NotaCreditoId", NotaCreditoId);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var result = new RequestResponseModel()
                        {
                            Result = true,
                            Message = "",
                            Object = null
                        };
                        lista.Add(result);
                    }
                }
                else
                {
                    var rsult = new RequestResponseModel()
                    {
                        Result = false,
                        Message = "",
                        Object = null
                    };
                    lista.Add(rsult);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
