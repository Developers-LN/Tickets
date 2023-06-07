using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures.PayableAward
{
    public class ProcedureAllPayableAwards
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_PayableAwards> ConsultaTodosBilletesPagables(int raffle)
        {
            var lista = new List<ModelProcedure_PayableAwards>();

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
                        var pagables = new ModelProcedure_PayableAwards()
                        {
                            premios = true,
                            number = Convert.ToInt32(sqlDataReader["number"].ToString()),
                            tanId = Convert.ToInt32(sqlDataReader["tanid"].ToString()),
                            //typeAward = sqlDataReader["typeAward"].ToString(),
                            //payStatu = sqlDataReader["payStatu"].ToString(),
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            Id_Name = sqlDataReader["Id_Name"].ToString(),
                            raffle = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            quantity = Convert.ToInt32(sqlDataReader["Quantity"].ToString()),
                            nameaward = sqlDataReader["name"].ToString(),
                            terminal = Convert.ToInt32(sqlDataReader["terminal"].ToString()),
                            fracciones = Convert.ToInt32(sqlDataReader["fracciones"].ToString()),
                            valorpagar = Convert.ToDecimal(sqlDataReader["valorapagar"].ToString()),
                            value = Convert.ToDecimal(sqlDataReader["value"].ToString()),
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelProcedure_PayableAwards()
                    {
                        premios = false,
                        number = 0,
                        ClientId = 0,
                        Id_Name = "N/A",
                        tanId = 0,
                        payStatu = "N/A",
                        typeAward = "N/A",
                        raffle = raffle,
                        quantity = 0,
                        nameaward = "N/A",
                        terminal = 0,
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
