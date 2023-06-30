using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures.PayableAward
{
    public class Procedure_PayableAwardByClient
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_PayableAwardByClient> ConsultaBilletesPagablesPorCliente(int raffle, int client)
        {
            var lista = new List<ModelProcedure_PayableAwardByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AllPayableAwardByClient", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlCommand.Parameters.AddWithValue("@ClientId", client);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagables = new ModelProcedure_PayableAwardByClient()
                        {
                            Data = true,
                            TaId = Convert.ToInt32(sqlDataReader["taid"].ToString()),
                            TanId = Convert.ToInt32(sqlDataReader["tanid"].ToString()),
                            Number = Convert.ToInt32(sqlDataReader["number"].ToString()),
                            Id_Name = sqlDataReader["Id_Name"].ToString(),
                            ControlNumber = sqlDataReader["ControlNumber"].ToString(),
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            RaffleId = raffle,
                            NameAward = sqlDataReader["name"].ToString(),
                            RaffleAwardId = Convert.ToInt32(sqlDataReader["RaffleAwardId"].ToString()),
                            Fracciones = Convert.ToInt32(sqlDataReader["fracciones"].ToString()),
                            ValorPagar = Convert.ToDecimal(sqlDataReader["valorapagar"].ToString()),
                            Value = Convert.ToDecimal(sqlDataReader["value"].ToString()),
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelProcedure_PayableAwardByClient()
                    {
                        Data = false,
                        TaId = 0,
                        TanId = 0,
                        Number = 0,
                        ControlNumber = "N/A",
                        ClientName = "N/A",
                        Id_Name = "N/A",
                        ClientId = 0,
                        RaffleId = raffle,
                        NameAward = "N/A",
                        RaffleAwardId = 0,
                        Fracciones = 0,
                        ValorPagar = 0,
                        Value = 0,
                    };
                    lista.Add(pagables);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
