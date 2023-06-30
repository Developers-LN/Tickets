using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.Returns;

namespace Tickets.Models.Procedures.Returns
{
    public class Procedure_ReturnsSummary
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_ReturnsSummary> ReturnedSummary(int raffle)
        {
            var lista = new List<ModelProcedure_ReturnsSummary>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("ReturnsSummary", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Allocation = new ModelProcedure_ReturnsSummary()
                        {
                            Data = true,
                            RaffleId = raffle,
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            Assigned = Convert.ToInt32(sqlDataReader["Assigned"].ToString()),
                            Printed = Convert.ToInt32(sqlDataReader["Printed"].ToString()),
                            Consignate = Convert.ToInt32(sqlDataReader["Consignated"].ToString()),
                            TicketReturned = Convert.ToInt32(sqlDataReader["TicketReturned"].ToString()),
                            FractionReturned = Convert.ToInt32(sqlDataReader["FractionsReturned"].ToString()),
                            TicketSold = Convert.ToInt32(sqlDataReader["TicketsSold"].ToString()),
                            TotalFractionReturned = Convert.ToInt32(sqlDataReader["TotalFractionsReturned"].ToString()),
                            TotalTicketSold = Convert.ToInt32(sqlDataReader["TotalFractionsSold"].ToString()),
                            FractionSold = Convert.ToInt32(sqlDataReader["FractionsSold"].ToString())
                        };
                        lista.Add(Allocation);
                    }
                }
                else
                {
                    var Allocation = new ModelProcedure_ReturnsSummary()
                    {
                        Data = false,
                        RaffleId = raffle,
                        ClientId = 0,
                        ClientName = "N/A",
                        Assigned = 0,
                        Printed = 0,
                        Consignate = 0,
                        TicketReturned = 0,
                        TotalFractionReturned = 0,
                        TotalTicketSold = 0,
                        FractionReturned = 0,
                        TicketSold = 0,
                        FractionSold = 0
                    };
                    lista.Add(Allocation);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
