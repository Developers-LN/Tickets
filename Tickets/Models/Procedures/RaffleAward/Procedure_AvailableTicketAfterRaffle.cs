using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Tickets.Models.ModelsProcedures.RaffleAward;

namespace Tickets.Models.Procedures.RaffleAward
{
    public class Procedure_AvailableTicketAfterRaffle
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<ModelProcedure_AvailableTicketsAfterRaffle> AvailableTicketsAfterRaffle(int raffle)
        {
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
                    var ItemList = sqlDataReader.Cast<IDataRecord>().AsEnumerable().Select(s => new
                    {
                        Data = true,
                        RaffleId = Convert.ToInt32(s["RaffleId"].ToString()),
                        TicketNumber = Convert.ToInt32(s["Number"].ToString()),
                        AvailableFractions = Convert.ToInt32(s["AvailableFractions"].ToString())
                    }).ToList();

                    var jsonSerialize = JsonConvert.SerializeObject(ItemList);
                    AuxList = JsonConvert.DeserializeObject<List<ModelProcedure_AvailableTicketsAfterRaffle>>(jsonSerialize);
                }
                else
                {
                    var item = new ModelProcedure_AvailableTicketsAfterRaffle()
                    {
                        Data = false,
                        RaffleId = 0,
                        TicketNumber = 0,
                        AvailableFractions = 0
                    };
                    AuxList.Add(item);
                }
                sqlConnection.Close();
            }
            return AuxList;
        }
    }
}
