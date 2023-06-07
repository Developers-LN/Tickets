using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.Returns;

namespace Tickets.Models.Procedures.Returns
{
    public class Procedure_ReturnedGroupByClient
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_ReturnedGroupByClient> ReturnedGroupByClient(int raffleId, int clientId, string group)
        {
            var lista = new List<ModelProcedure_ReturnedGroupByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("ReturnedGroupByClient", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffleId);
                sqlCommand.Parameters.AddWithValue("@ClientId", clientId);
                sqlCommand.Parameters.AddWithValue("@Group", group);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var ReturnData = new ModelProcedure_ReturnedGroupByClient()
                        {
                            Data = true,
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            Client_Id_Name = sqlDataReader["Client_Id_Name"].ToString(),
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            RaffleName = sqlDataReader["RaffleName"].ToString(),
                            Raffle_Id_Name = sqlDataReader["Raffle_Id_Name"].ToString(),
                            TicketNumber = sqlDataReader["TicketNumber"].ToString(),
                            Grupo = sqlDataReader["Grupo"].ToString(),
                            FractionFrom = Convert.ToInt32(sqlDataReader["FractionFrom"].ToString()),
                            FractionTo = Convert.ToInt32(sqlDataReader["FractionTo"].ToString()),
                            CreateUser = Convert.ToInt32(sqlDataReader["CreateUser"].ToString()),
                            UserName = sqlDataReader["UserName"].ToString(),
                            EmployerName = sqlDataReader["EmployerName"].ToString(),
                            Production = Convert.ToInt32(sqlDataReader["Production"].ToString()),
                            Discount = Convert.ToDecimal(sqlDataReader["CreateUser"].ToString()),
                            TicketFraction = Convert.ToInt32(sqlDataReader["TicketFraction"].ToString())
                        };
                        lista.Add(ReturnData);
                    }
                }
                else
                {
                    var ReturnData = new ModelProcedure_ReturnedGroupByClient()
                    {
                        Data = false,
                        ClientId = 0,
                        ClientName = "",
                        Client_Id_Name = "",
                        RaffleId = 0,
                        RaffleName = "",
                        Raffle_Id_Name = "",
                        TicketNumber = "",
                        Grupo = "",
                        FractionFrom = 0,
                        FractionTo = 0,
                        CreateUser = 0,
                        UserName = "",
                        EmployerName = "",
                        Production = 0,
                        Discount = 0.0m,
                        TicketFraction = 0
                    };
                    lista.Add(ReturnData);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
