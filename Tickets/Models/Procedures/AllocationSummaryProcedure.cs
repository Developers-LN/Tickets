using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class AllocationSummaryProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<AllocationSummary> ConsultaAsignacionesSorteo(int raffle)
        {
            var lista = new List<AllocationSummary>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AllocationSummary", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagables = new AllocationSummary()
                        {
                            Data = true,
                            AsignacionId = Convert.ToInt32(sqlDataReader["AsignacionId"].ToString()),
                            RaffleId = raffle,
                            Fecha = Convert.ToDateTime(sqlDataReader["Fecha"].ToString()),
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            Cliente = sqlDataReader["Cliente"].ToString(),
                            Fracciones = Convert.ToInt32(sqlDataReader["Fracciones"].ToString()),
                            Hojas = Convert.ToInt32(sqlDataReader["Hojas"].ToString()),
                            Billetes = Convert.ToInt32(sqlDataReader["Billetes"].ToString()),
                            Monto = Convert.ToDecimal(sqlDataReader["Monto"].ToString()),
                            Descuento = Convert.ToDecimal(sqlDataReader["Descuento"].ToString()),
                            MontoAPagar = Convert.ToDecimal(sqlDataReader["MontoAPagar"].ToString())
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new AllocationSummary()
                    {
                        Data = false,
                        AsignacionId = 0,
                        RaffleId = raffle,
                        Fecha = DateTime.Now,
                        ClientId = 0,
                        Cliente = "0",
                        Fracciones = 0,
                        Hojas = 0,
                        Billetes = 0,
                        Monto = 0,
                        Descuento = 0,
                        MontoAPagar = 0
                    };
                    lista.Add(pagables);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
