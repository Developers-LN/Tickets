using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class CuadreSorteoResumidoProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelCuadreSorteoResumido> CuadreSorteoResumido(int raffle)
        {
            var lista = new List<ModelCuadreSorteoResumido>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("CuadreSorteoResumido", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var resumen = new ModelCuadreSorteoResumido()
                        {
                            Data = true,
                            RaffleId = raffle,
                            Premio = sqlDataReader["Premio"].ToString(),
                            ProspectoFracciones = Convert.ToInt32(sqlDataReader["CantidadFracciones"].ToString()),
                            ProspectoPremio = Convert.ToDecimal(sqlDataReader["ValorPremio"].ToString()),
                            CasaFracciones = Convert.ToInt32(sqlDataReader["FraccionesCasa"].ToString()),
                            CasaMonto = Convert.ToDecimal(sqlDataReader["MontoFraccionesCasa"].ToString()),
                            CalleFracciones = Convert.ToInt32(sqlDataReader["FraccionesCalle"].ToString()),
                            CalleMonto = Convert.ToDecimal(sqlDataReader["MontoCalle"].ToString()),
                            NoImpresoFracciones = Convert.ToInt32(sqlDataReader["FraccionesNoImpresas"].ToString()),
                            NoImpresoMonto = Convert.ToDecimal(sqlDataReader["MontoNoImpreso"].ToString())
                        };
                        lista.Add(resumen);
                    }
                }
                else
                {
                    var resumen = new ModelCuadreSorteoResumido()
                    {
                        Data = false,
                        RaffleId = raffle,
                        Premio = "",
                        ProspectoFracciones = 0,
                        ProspectoPremio = 0,
                        CasaFracciones = 0,
                        CasaMonto = 0,
                        CalleFracciones = 0,
                        CalleMonto = 0,
                        NoImpresoFracciones = 0,
                        NoImpresoMonto = 0
                    };
                    lista.Add(resumen);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
