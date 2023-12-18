using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Tickets.Models.ModelsProcedures;
using Tickets.Models.ModelsProcedures.Raffle;

namespace Tickets.Models.Procedures.Raffle
{
    public class Procedure_RaffleSales
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_RaffleSales> RaffleSales(int raffle)
        {
            var lista = new List<ModelProcedure_RaffleSales>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("RaffleSales", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var pagables = new ModelProcedure_RaffleSales()
                        {
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            RaffleName = sqlDataReader["RaffleName"].ToString(),
                            RaffleDate = sqlDataReader["RaffleDate"].ToString(),
                            GrossSales = Convert.ToDecimal(sqlDataReader["GrossSales"].ToString()),
                            Order = Convert.ToInt32(sqlDataReader["OrderAward"].ToString()),
                            NetSales = Convert.ToDecimal(sqlDataReader["NetSales"].ToString()),
                            Award = sqlDataReader["Award"].ToString(),
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelProcedure_RaffleSales()
                    {
                        RaffleId = 0,
                        RaffleName = "",
                        RaffleDate = "",
                        GrossSales = 0,
                        NetSales = 0,
                        Order = 0,
                        Award = ""
                    };
                    lista.Add(pagables);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
