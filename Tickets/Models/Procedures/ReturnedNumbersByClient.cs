using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class ReturnedNumbersByClient
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelReturnedNumbersByClient> ConsultarBilletesDevueltosPorCliente(int raffle)
        {
            var lista = new List<ModelReturnedNumbersByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("ReturnedNumbersByClient", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagables = new ModelReturnedNumbersByClient()
                        {
                            datos = true,
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            Billetes = Convert.ToInt32(sqlDataReader["BilletesDevueltos"].ToString()),
                            FraccionesRestantes = Convert.ToInt32(sqlDataReader["FraccionesRestantes"].ToString()),
                            Fracciones = Convert.ToInt32(sqlDataReader["FraccionesDevueltas"].ToString()),
                            PrecioFraccion = Convert.ToDecimal(sqlDataReader["PrecioFraccion"].ToString()),
                            PorcientoDescuento = Convert.ToDecimal(sqlDataReader["Descuento"].ToString()),
                            Factura = Convert.ToDecimal(sqlDataReader["TotalFactura"].ToString()),
                            Descuento = Convert.ToDecimal(sqlDataReader["TotalDescuento"].ToString()),
                            Total = Convert.ToDecimal(sqlDataReader["MontoTotal"].ToString())
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelReturnedNumbersByClient()
                    {
                        datos = false,
                        RaffleId = raffle,
                        ClientId = 0,
                        ClientName = "N/A",
                        Billetes = 0,
                        FraccionesRestantes = 0,
                        Fracciones = 0,
                        PrecioFraccion = 0,
                        PorcientoDescuento = 0,
                        Factura = 0,
                        Descuento = 0,
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