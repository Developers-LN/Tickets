using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class PayableAwardByClientProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelPayableAwardByClient> ConsultaBilletesPagablesPorCliente(int raffle)
        {
            var lista = new List<ModelPayableAwardByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("PayableAwardByClient", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagables = new ModelPayableAwardByClient()
                        {
                            Data = true,
                            number = Convert.ToInt32(sqlDataReader["number"].ToString()),
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            RaffleId = raffle,
                            nameaward = sqlDataReader["name"].ToString(),
                            fracciones = Convert.ToInt32(sqlDataReader["fracciones"].ToString()),
                            valorpagar = Convert.ToDecimal(sqlDataReader["valorapagar"].ToString()),
                            value = Convert.ToDecimal(sqlDataReader["value"].ToString()),
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelPayableAwardByClient()
                    {
                        Data = false,
                        number = 0,
                        ClientId = 0,
                        RaffleId = raffle,
                        nameaward = "N/A",
                        fracciones = 0,
                        valorpagar = 0,
                        value = 0,
                    };
                    lista.Add(pagables);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}