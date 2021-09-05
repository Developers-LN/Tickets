using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class AvailableTicketsProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelAvailableTickets> ConsultaBilletesDisponible(int raffle)
        {
            var lista = new List<ModelAvailableTickets>();

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
                        var Disponibles = new ModelAvailableTickets()
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
                    var Disponibles = new ModelAvailableTickets()
                    {
                        Data = true,
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