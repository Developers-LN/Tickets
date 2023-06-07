using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class AvailableTicketsProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_AvailableTickets> ConsultaBilletesDisponible(int raffle)
        {
            var lista = new List<ModelProcedure_AvailableTickets>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AvailableTickets", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Disponibles = new ModelProcedure_AvailableTickets()
                        {
                            Data = true,
                            RaffleId = raffle,
                            AvailableFractions = Convert.ToInt32(sqlDataReader["AvailableFractions"].ToString()),
                            Number = Convert.ToInt32(sqlDataReader["TicketNumber"].ToString())
                        };
                        lista.Add(Disponibles);
                    }
                }
                else
                {
                    var Disponibles = new ModelProcedure_AvailableTickets()
                    {
                        Data = false,
                        RaffleId = raffle,
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