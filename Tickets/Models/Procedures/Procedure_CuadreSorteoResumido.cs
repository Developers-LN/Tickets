using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class Procedure_CuadreSorteoResumido
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_CuadreSorteo> CuadreSorteo(int raffle)
        {
            var lista = new List<ModelProcedure_CuadreSorteo>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("CuadreSorteoDetallado", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var resumen = new ModelProcedure_CuadreSorteo()
                        {
                            Data = true,
                            RaffleId = raffle,
                            Premio = sqlDataReader["Premio"].ToString(),
                            CantidadPremios = Convert.ToInt32(sqlDataReader["CantidadPremios"].ToString()),
                            PremioOrden = Convert.ToInt32(sqlDataReader["OrdenPremio"].ToString()),
                            ProspectoFracciones = Convert.ToInt32(sqlDataReader["CantidadFracciones"].ToString()),
                            MontoEnPremios = Convert.ToDecimal(sqlDataReader["MontoPremios"].ToString()),
                            MontoPremioProspecto = Convert.ToDecimal(sqlDataReader["ValorPremio"].ToString()),
                            CasaFracciones = Convert.ToInt32(sqlDataReader["FraccionesCasa"].ToString()),
                            CasaMonto = Convert.ToDecimal(sqlDataReader["MontoFraccionesCasa"].ToString()),
                            CalleFracciones = Convert.ToInt32(sqlDataReader["FraccionesCalle"].ToString()),
                            CalleMonto = Convert.ToDecimal(sqlDataReader["MontoFraccionesCalle"].ToString()),
                            NoImpresoFracciones = Convert.ToInt32(sqlDataReader["FraccionesNoImpresas"].ToString()),
                            NoImpresoMonto = Convert.ToDecimal(sqlDataReader["MontoFraccionesNoImpresas"].ToString()),
                            FraccionesPagadas = Convert.ToInt32(sqlDataReader["FraccionesPagadas"].ToString()),
                            MontoFraccionesPagadas = Convert.ToDecimal(sqlDataReader["MontoFraccionesPagadas"].ToString()),
                            FraccionesNoPagadas = Convert.ToInt32(sqlDataReader["FraccionesPendientePago"].ToString()),
                            MontoFraccionesNoPagadas = Convert.ToDecimal(sqlDataReader["MontoFraccionesPendientePago"].ToString()),
                        };
                        lista.Add(resumen);
                    }
                }
                else
                {
                    var resumen = new ModelProcedure_CuadreSorteo()
                    {
                        Data = false,
                        RaffleId = raffle,
                        Premio = "",
                        CantidadPremios = 0,
                        PremioOrden = 0,
                        ProspectoFracciones = 0,
                        MontoEnPremios = 0,
                        MontoPremioProspecto = 0,
                        CasaFracciones = 0,
                        CasaMonto = 0,
                        CalleFracciones = 0,
                        CalleMonto = 0,
                        NoImpresoFracciones = 0,
                        NoImpresoMonto = 0,
                        FraccionesPagadas = 0,
                        MontoFraccionesPagadas = 0,
                        FraccionesNoPagadas = 0,
                        MontoFraccionesNoPagadas = 0,
                    };
                    lista.Add(resumen);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
