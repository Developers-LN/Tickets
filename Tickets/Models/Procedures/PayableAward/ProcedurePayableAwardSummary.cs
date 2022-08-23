using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.PayableAward;

namespace Tickets.Models.Procedures.PayableAward
{
    public class ProcedurePayableAwardSummary
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelPayableAwardSummary> payableAwardSummary(int raffle)
        {
            var lista = new List<ModelPayableAwardSummary>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("PayableAwardSummary", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagables = new ModelPayableAwardSummary()
                        {
                            Data = true,
                            RaffleId = raffle,
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["Id_Name"].ToString(),
                            CountAward = Convert.ToInt32(sqlDataReader["CountAward"].ToString()),
                            TotalPayable = Convert.ToDecimal(sqlDataReader["TotalPayable"].ToString()),
                            CountPayed = Convert.ToInt32(sqlDataReader["CountPayed"].ToString()),
                            TotalPayed = Convert.ToDecimal(sqlDataReader["TotalPayed"].ToString()),
                            CountPending = Convert.ToInt32(sqlDataReader["CountPending"].ToString()),
                            TotalPending = Convert.ToDecimal(sqlDataReader["TotalPending"].ToString())
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelPayableAwardSummary()
                    {
                        Data = false,
                        RaffleId = raffle,
                        ClientId = 0,
                        ClientName = "N/A",
                        CountAward = 0,
                        TotalPayable = 0,
                        CountPayed = 0,
                        TotalPayed = 0,
                        CountPending = 0,
                        TotalPending = 0
                    };
                    lista.Add(pagables);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}