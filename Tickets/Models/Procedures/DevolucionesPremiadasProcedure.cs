using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class DevolucionesPremiadasProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelDevolucionesPremiadas> ConsultaDevolucionesPremiadas(int raffle, string grupo)
        {
            var lista = new List<ModelDevolucionesPremiadas>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("DevolucionesPremiadas", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlCommand.Parameters.AddWithValue("@ReturnGroup", grupo);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var devolucion = new ModelDevolucionesPremiadas()
                        {
                            Data = true,
                            RaffleId = raffle,
                            ReturnedGroup = sqlDataReader["ReturnedGroup"].ToString(),
                            ClientId = Convert.ToInt32(sqlDataReader["IdCliente"].ToString()),
                            Cliente = sqlDataReader["Cliente"].ToString(),
                            Numero = Convert.ToInt32(sqlDataReader["Numero"].ToString()),
                            Premio = sqlDataReader["Premio"].ToString(),
                            CantidadFracciones = Convert.ToInt32(sqlDataReader["CantidadFracciones"].ToString()),
                            ValorFraccion = Convert.ToDecimal(sqlDataReader["ValorFraccion"].ToString()),
                            Monto = Convert.ToDecimal(sqlDataReader["Monto"].ToString())
                        };
                        lista.Add(devolucion);
                    }
                }
                else
                {
                    var devolucion = new ModelDevolucionesPremiadas()
                    {
                        Data = false,
                        RaffleId = raffle,
                        ReturnedGroup = "",
                        ClientId = 0,
                        Cliente = "",
                        Numero = 0,
                        Premio = "",
                        CantidadFracciones = 0,
                        ValorFraccion = 0,
                        Monto = 0
                    };
                    lista.Add(devolucion);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
