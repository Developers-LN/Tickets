using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.IdentifiBachDetails;

namespace Tickets.Models.Procedures.IdentifyBachDetails
{
    public class ProcedureIdentifyBachNumbers
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModeProcedureIdentifyBachNumbers> GetIdentifyBachNumbers(int Bach)
        {
            var IdentifyBachNumbers = new List<ModeProcedureIdentifyBachNumbers>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("GetIdentifyBachNumbers", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Bach", Bach);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var bachPayment = new ModeProcedureIdentifyBachNumbers()
                        {
                            Data = true,
                            Id = Convert.ToInt32(sqlDataReader["Id"].ToString()),
                            Numero = sqlDataReader["Numero"].ToString(),
                            FractionFrom = Convert.ToInt32(sqlDataReader["FractionFrom"].ToString()),
                            FractionTo = Convert.ToInt32(sqlDataReader["FractionTo"].ToString()),
                            NombrePremio = sqlDataReader["NombrePremio"].ToString(),
                            CantidadFraccionesPremiadas = Convert.ToInt32(sqlDataReader["CantidadFraccionesPremiadas"].ToString()),
                            PremioPorFraccion = Convert.ToDecimal(sqlDataReader["PremioPorFraccion"].ToString()),
                            TotalEnPremio = Convert.ToDecimal(sqlDataReader["TotalEnPremio"].ToString()),
                            Descuento = Convert.ToDecimal(sqlDataReader["Descuento"].ToString()),
                            MontoPagar = Convert.ToDecimal(sqlDataReader["MontoPagar"].ToString())
                        };
                        IdentifyBachNumbers.Add(bachPayment);
                    }
                }
                /*else
                {
                    var bachPayment = new ModeProcedureIdentifyBachNumbers()
                    {
                        Data = false,
                        Id = 0,
                        Numero = "",
                        FractionFrom = 0,
                        FractionTo = 0,
                        NombrePremio = "",
                        CantidadFraccionesPremiadas = 0,
                        PremioPorFraccion = 0,
                        TotalEnPremio = 0,
                        Descuento = 0,
                        MontoPagar = 0
                    };
                    IdentifyBachNumbers.Add(bachPayment);
                }*/
                sqlConnection.Close();
            }
            return IdentifyBachNumbers;
        }
    }
}
