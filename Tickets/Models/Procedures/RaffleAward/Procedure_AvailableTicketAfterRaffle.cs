using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
//using System.Text.Json;
using Tickets.Models.ModelsProcedures.RaffleAward;

namespace Tickets.Models.Procedures.RaffleAward
{
    public class Procedure_AvailableTicketAfterRaffle
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<object> ListaFacturas(int raffle, int client)
        {
            var lista = new List<object>();
            var AuxList = new List<ModelProcedure_AvailableTicketsAfterRaffle>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AvailableTicketsAfterRaffle", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    var ItemList = sqlDataReader.Cast<IDataRecord>().Select(s => new
                    {
                        RaffleId = s["RaffleId"],
                        TicketNumber = s["Number"],
                        AvailableFractions = s["AvailableFractions"]
                    }).ToList();

                    //var jsonSerialize = JsonSerializer.Serialize(ItemList);
                    //AuxList = JsonSerializer.Deserialize<List<ModelProcedure_AvailableTicketsAfterRaffle>>(jsonSerialize);
                }
                else
                {
                    var facturas = new ModelProcedure_AvailableTicketsAfterRaffle()
                    {
                        Data = false,
                        RaffleId = 0,
                        TicketNumber = 0,
                        AvailableFractions = 0
                    };
                    lista.Add(facturas);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
