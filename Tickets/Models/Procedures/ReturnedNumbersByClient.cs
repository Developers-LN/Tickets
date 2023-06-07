using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class ReturnedNumbersByClient
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_ReturnedNumbersByClient> ConsultarBilletesDevueltosPorCliente(int raffle)
        {
            var lista = new List<ModelProcedure_ReturnedNumbersByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("ReturnedNumbersByClient", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagables = new ModelProcedure_ReturnedNumbersByClient()
                        {
                            Datos = true,
                            RaffleId = raffle,
                            ClientId = Convert.ToInt32(sqlDataReader["IdCliente"].ToString()),
                            ClientName = sqlDataReader["NombreCliente"].ToString(),
                            Billetes = Convert.ToInt32(sqlDataReader["Billetes"].ToString()),
                            FraccionesRestantes = Convert.ToInt32(sqlDataReader["FraccionesRestantes"].ToString()),
                            Fracciones = Convert.ToInt32(sqlDataReader["TotalFracciones"].ToString()),
                            Hojas = Convert.ToDecimal(sqlDataReader["Hojas"].ToString()),
                            PrecioFraccion = Convert.ToDecimal(sqlDataReader["PrecioFraccion"].ToString()),
                            Total = Convert.ToDecimal(sqlDataReader["Total"].ToString())
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelProcedure_ReturnedNumbersByClient()
                    {
                        Datos = false,
                        RaffleId = raffle,
                        ClientId = 0,
                        ClientName = "N/A",
                        Billetes = 0,
                        FraccionesRestantes = 0,
                        Fracciones = 0,
                        Hojas = 0,
                        PrecioFraccion = 0,
                        Total = 0
                    };
                    lista.Add(pagables);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}