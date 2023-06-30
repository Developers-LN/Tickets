using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.Allocations;

namespace Tickets.Models.Procedures.Allocations
{
    public class Procedure_AllocatedSummary
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_AllocatedSummary> AllocatedSummary(int raffle)
        {
            var lista = new List<ModelProcedure_AllocatedSummary>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AllocatedSummary", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Allocation = new ModelProcedure_AllocatedSummary()
                        {
                            Data = true,
                            RaffleId = raffle,
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            ClientIdName = sqlDataReader["Id_Name"].ToString(),
                            TotalAllocated = Convert.ToDecimal(sqlDataReader["TotalAllocated"].ToString()),
                            TotaConsignated = Convert.ToDecimal(sqlDataReader["TotalConsignated"].ToString()),
                            TotalReturned = Convert.ToDecimal(sqlDataReader["TotalReturned"].ToString()),
                            TotalSold = Convert.ToDecimal(sqlDataReader["TotalSold"].ToString()),
                            TotalCanceled = Convert.ToDecimal(sqlDataReader["TotalCanceled"].ToString())
                        };
                        lista.Add(Allocation);
                    }
                }
                else
                {
                    var Allocation = new ModelProcedure_AllocatedSummary()
                    {
                        Data = false,
                        RaffleId = raffle,
                        ClientId = 0,
                        ClientName = "N/A",
                        ClientIdName = "N/A",
                        TotalAllocated = 0,
                        TotaConsignated = 0,
                        TotalReturned = 0,
                        TotalSold = 0,
                        TotalCanceled = 0
                    };
                    lista.Add(Allocation);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}