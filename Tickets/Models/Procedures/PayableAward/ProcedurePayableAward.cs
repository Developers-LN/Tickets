using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures.PayableAward
{
    public class ProcedurePayableAward
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_PayableAwardByClient> ConsultaBilletesPagables(int raffle)
        {
            var lista = new List<ModelProcedure_PayableAwardByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AllPayableAward", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
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