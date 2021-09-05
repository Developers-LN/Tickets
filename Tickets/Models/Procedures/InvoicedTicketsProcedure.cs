using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class InvoicedTicketsProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelInvoicedTickets> ConsultaBilletesVendidos(int raffle)
        {
            var lista = new List<ModelInvoicedTickets>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("InvoicedTickets", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Ventas = new ModelInvoicedTickets()
                        {
                            Data = true,
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            Number = Convert.ToInt32(sqlDataReader["TicketNumber"].ToString())
                        };
                        lista.Add(Ventas);
                    }
                }
                else
                {
                    var Ventas = new ModelInvoicedTickets()
                    {
                        Data = false,
                        RaffleId = raffle,
                        Number = 0
                    };
                    lista.Add(Ventas);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}