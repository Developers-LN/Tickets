using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class AvailableTicketToInvoice
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_AvailableTicketsToInvoice> AvailableTicketsToInvoice(int raffle, int Allocation)
        {
            var lista = new List<ModelProcedure_AvailableTicketsToInvoice>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AvailableTicketToInvoice", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
                sqlCommand.Parameters.AddWithValue("@Allocation", Allocation);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Disponibles = new ModelProcedure_AvailableTicketsToInvoice()
                        {
                            Data = true,
                            RaffleId = raffle,
                            Statu = Convert.ToInt32(sqlDataReader["Statu"].ToString()),
                            AllocationId = Convert.ToInt32(sqlDataReader["IdAllocation"].ToString()),
                            AllocationNumberId = Convert.ToInt32(sqlDataReader["IdAllocationNumber"].ToString()),
                            AvailableFractions = Convert.ToInt32(sqlDataReader["AvailableFractions"].ToString()),
                            TicketFraction = Convert.ToInt32(sqlDataReader["TicketFractions"].ToString()),
                            Number = Convert.ToInt32(sqlDataReader["TicketNumber"].ToString())
                        };
                        lista.Add(Disponibles);
                    }
                }
                else
                {
                    var Disponibles = new ModelProcedure_AvailableTicketsToInvoice()
                    {
                        Data = true,
                        RaffleId = raffle,
                        AllocationId = 0,
                        Statu = 0,
                        AllocationNumberId = 0,
                        TicketFraction = 0,
                        AvailableFractions = 0,
                        Number = 0
                    };
                    lista.Add(Disponibles);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
