using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class NetSalesByClientProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelNetSalesByClient> ConsultaVentaNetaPorCliente(int raffle)
        {
            var lista = new List<ModelNetSalesByClient>();

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
                        var pagables = new ModelNetSalesByClient()
                        {
                            Data = true,
                            ClientId = Convert.ToInt32(sqlDataReader["ClientId"].ToString()),
                            ClientName = sqlDataReader["ClientName"].ToString(),
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            ProspectId = Convert.ToInt32(sqlDataReader["ProspectId"].ToString()),
                            TicketFractions = Convert.ToInt32(sqlDataReader["TicketFractions"].ToString()),
                            TicketNumber = Convert.ToInt32(sqlDataReader["TicketNumber"].ToString()),
                            FractionPrice = Convert.ToDecimal(sqlDataReader["FractionPrice"].ToString()),
                            IdAllocationNumber = Convert.ToInt32(sqlDataReader["IdAllocationNumber"].ToString()),
                            IdAllocation = Convert.ToInt32(sqlDataReader["IdAllocation"].ToString()),
                            AvailableFractions = Convert.ToInt32(sqlDataReader["AvailableFractions"].ToString()),
                            ReturnFractions = Convert.ToInt32(sqlDataReader["ReturnFractions"].ToString()),
                            NetSale = Convert.ToDecimal(sqlDataReader["NetSale"].ToString())
                        };
                        lista.Add(pagables);
                    }
                }
                else
                {
                    var pagables = new ModelNetSalesByClient()
                    {
                        Data = false,
                        ClientId = 0,
                        ClientName = "N/A",
                        RaffleId = 0,
                        ProspectId = 0,
                        TicketFractions = 0,
                        TicketNumber = 0,
                        FractionPrice = 0,
                        IdAllocationNumber = 0,
                        IdAllocation = 0,
                        AvailableFractions = 0,
                        ReturnFractions = 0,
                        NetSale = 0
                    };
                    lista.Add(pagables);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}