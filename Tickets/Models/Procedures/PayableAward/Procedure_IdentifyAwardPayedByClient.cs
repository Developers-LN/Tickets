﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.PayableAward;

namespace Tickets.Models.Procedures.PayableAward
{
    public class Procedure_IdentifyAwardPayedByClient
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_IdentifyAwardPayedByClient> ConsultaBilletesPagablesPorCliente(int raffle, int client)
        {
            var lista = new List<ModelProcedure_IdentifyAwardPayedByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("IdentifyAwardPayedByClientProcedure", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlCommand.Parameters.AddWithValue("@ClientId", client);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagados = new ModelProcedure_IdentifyAwardPayedByClient()
                        {
                            Data = true,
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            RaffleName = sqlDataReader["RaffleName"].ToString(),
                            Id_Name_Raffle = sqlDataReader["Id_Name_Raffle"].ToString(),
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            Id_Name_Client = sqlDataReader["Id_Name_Client"].ToString(),
                            Fracciones = Convert.ToInt32(sqlDataReader["Fracciones"].ToString()),
                            Monto = Convert.ToDecimal(sqlDataReader["Monto"].ToString()),
                            TotalAward = Convert.ToDecimal(sqlDataReader["TotalAward"].ToString()),
                            Bono = Convert.ToDecimal(sqlDataReader["Bono"].ToString()),
                        };
                        lista.Add(pagados);
                    }
                }
                else
                {
                    var pagados = new ModelProcedure_IdentifyAwardPayedByClient()
                    {
                        Data = false,
                        RaffleId = 0,
                        RaffleName = "",
                        Id_Name_Raffle = "",
                        ClientId = 0,
                        ClientName = "",
                        Id_Name_Client = "",
                        Fracciones = 0,
                        Monto = 0.0m,
                        TotalAward = 0.0m,
                        Bono = 0.0m
                    };
                    lista.Add(pagados);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
