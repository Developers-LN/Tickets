using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.IdentifiBachDetails;

namespace Tickets.Models.Procedures.IdentifyBachDetails
{
    public class ProcedureIdentifyBachNoteCreditPayment
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<object> GetIdentifyBachPaymentNoteCredit(int Bach)
        {
            var IdentifyBachPaymentNoteCredit = new List<object>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("GetIdentifyBachPaymentNoteCredit", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Bach", Bach);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var bachPayment = new ModelProcedure_IdentifyBachPaymentNoteCredit()
                        {
                            Data = true,
                            Id = Convert.ToInt32(sqlDataReader["Id"].ToString()),
                            Name = sqlDataReader["Name"].ToString(),
                            Concepts = sqlDataReader["Concepts"].ToString(),
                            TotalCash = Convert.ToDecimal(sqlDataReader["TotalCash"].ToString())
                        };
                        IdentifyBachPaymentNoteCredit.Add(bachPayment);
                    }
                }
                else
                {
                    var bachPayment = new ModelProcedure_IdentifyBachPaymentNoteCredit()
                    {
                        Data = false,
                        Id = 0,
                        Name = "",
                        Concepts = "",
                        TotalCash = 0
                    };
                    IdentifyBachPaymentNoteCredit.Add(bachPayment);
                }
                sqlConnection.Close();
            }
            return IdentifyBachPaymentNoteCredit;
        }
    }
}
