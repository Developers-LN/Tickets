using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class ReturnedTicketsProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelReturnedTickets> ConsultaBilletesDevueltos(int raffle)
        {
            var lista = new List<ModelReturnedTickets>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("ReturnedTickets", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Devoluciones = new ModelReturnedTickets()
                        {
                            Data = true,
                            RaffleId = raffle,
                            TicketNumber = Convert.ToInt32(sqlDataReader["TicketNumber"].ToString()),
                            ReturnFractions = Convert.ToInt32(sqlDataReader["ReturnFractions"].ToString())
                        };
                        lista.Add(Devoluciones);
                    }
                }
                else
                {
                    var Devoluciones = new ModelReturnedTickets()
                    {
                        Data = false,
                        RaffleId = raffle,
                        TicketNumber = 0,
                        ReturnFractions = 0
                    };
                    lista.Add(Devoluciones);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}