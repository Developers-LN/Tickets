using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.IdentifiBachDetails;

namespace Tickets.Models.Procedures.IdentifyBachDetails
{
    public class ProcedureIdentifyBachDetails
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<ModelProcedure_IdentifyBach> GetIdentifyBachInfo(int bach)
        {
            var DetalleLote = new List<ModelProcedure_IdentifyBach>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("GetIdentifyBachDetails", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Bach", bach);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Lote = new ModelProcedure_IdentifyBach()
                        {
                            Data = true,
                            IdLote = Convert.ToInt32(sqlDataReader["IdLote"].ToString()),
                            CodigoEstadoLote = Convert.ToInt32(sqlDataReader["CodigoEstadoLote"].ToString()),
                            Cedula = sqlDataReader["Cedula"].ToString(),
                            PagadoA = sqlDataReader["PagadoA"].ToString(),
                            Telefono = sqlDataReader["Telefono"].ToString(),
                            PorcentajeComision = Convert.ToDecimal(sqlDataReader["PorcentajeComision"].ToString()),
                            ComisionMayorista = Convert.ToDecimal(sqlDataReader["ComisionMayorista"].ToString()),
                            Nota = sqlDataReader["Nota"].ToString(),
                            NombreSorteo = sqlDataReader["NombreSorteo"].ToString(),
                            PropietarioLote = sqlDataReader["PropietarioLote"].ToString(),
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            MontoPagado = Convert.ToDecimal(sqlDataReader["MontoPagado"].ToString()),
                            MontoRestante = Convert.ToDecimal(sqlDataReader["MontoRestante"].ToString()),
                            EstadoLote = sqlDataReader["EstadoLote"].ToString(),
                            MontoPagar = Convert.ToDecimal(sqlDataReader["MontoPagar"].ToString()),
                            MontoEnPremios = Convert.ToDecimal(sqlDataReader["MontoEnPremios"].ToString()),
                            PagoNotaCredito = Convert.ToInt32(sqlDataReader["PagoNotaCredito"].ToString()),
                            PagoEfectivo = Convert.ToInt32(sqlDataReader["PagoEfectivo"].ToString())
                        };
                        DetalleLote.Add(Lote);
                    }
                }
                else
                {
                    var Lote = new ModelProcedure_IdentifyBach()
                    {
                        Data = false,
                        IdLote = 0,
                        CodigoEstadoLote = 0,
                        Cedula = "",
                        PagadoA = "",
                        Telefono = "",
                        PorcentajeComision = 0,
                        ComisionMayorista = 0,
                        Nota = "",
                        NombreSorteo = "",
                        PropietarioLote = "",
                        ClientId = 0,
                        MontoPagado = 0,
                        MontoRestante = 0,
                        EstadoLote = "",
                        MontoPagar = 0,
                        MontoEnPremios = 0,
                        PagoEfectivo = 0,
                        PagoNotaCredito = 0
                    };
                    DetalleLote.Add(Lote);
                }
                sqlConnection.Close();
            }
            return DetalleLote;
        }
    }
}
