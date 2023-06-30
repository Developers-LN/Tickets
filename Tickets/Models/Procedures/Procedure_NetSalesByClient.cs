using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class Procedure_NetSalesByClient
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_NetSalesByClient> ConsultaVentaNetaPorCliente(int raffle)
        {
            var lista = new List<ModelProcedure_NetSalesByClient>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("NetSalesByClient", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Raffle", raffle);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Ventas = new ModelProcedure_NetSalesByClient()
                        {
                            Data = true,
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            ProspectId = Convert.ToInt32(sqlDataReader["ProspectId"].ToString()),
                            ProspectFractions = Convert.ToInt32(sqlDataReader["ProspectFractions"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            Id_Name = sqlDataReader["Id_Name"].ToString(),
                            TotalConsigned = Convert.ToInt32(sqlDataReader["TotalConsigned"].ToString()),
                            ConsignedTickets = Convert.ToInt32(sqlDataReader["ConsignedTickets"].ToString()),
                            ConsignedFractions = Convert.ToInt32(sqlDataReader["ConsignedFractions"].ToString()),
                            TotalReturned = Convert.ToInt32(sqlDataReader["TotalReturned"].ToString()),
                            ReturnedTickets = Convert.ToInt32(sqlDataReader["ReturnedTickets"].ToString()),
                            ReturnedFractions = Convert.ToInt32(sqlDataReader["ReturnedFractions"].ToString()),
                            TotalAvailable = Convert.ToInt32(sqlDataReader["TotalAvailable"].ToString()),
                            AvailableTickets = Convert.ToInt32(sqlDataReader["AvailableTickets"].ToString()),
                            AvailableFractions = Convert.ToInt32(sqlDataReader["AvailableFractions"].ToString()),
                            Percentage = Convert.ToDecimal(sqlDataReader["Percentage"].ToString()),
                        };
                        lista.Add(Ventas);
                    }
                }
                else
                {
                    var Ventas = new ModelProcedure_NetSalesByClient()
                    {
                        Data = false,
                        ClientId = 0,
                        RaffleId = 0,
                        ProspectId = 0,
                        ProspectFractions = 0,
                        ClientName = "",
                        Id_Name = "",
                        TotalConsigned = 0,
                        ConsignedTickets = 0,
                        ConsignedFractions = 0,
                        TotalReturned = 0,
                        ReturnedTickets = 0,
                        ReturnedFractions = 0,
                        TotalAvailable = 0,
                        AvailableTickets = 0,
                        AvailableFractions = 0,
                        Percentage = 0.0m,
                    };
                    lista.Add(Ventas);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
