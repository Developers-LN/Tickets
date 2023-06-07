using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class InvoiceListProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<object> ListaFacturas(int raffle, int client)
        {
            var lista = new List<object>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("InvoiceList", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@RaffleId", raffle);
                sqlCommand.Parameters.AddWithValue("@ClientId", client);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var facturas = new ModelProcedure_InvoiceListModel()
                        {
                            Data = true,
                            Id = Convert.ToInt32(sqlDataReader["IdFactura"].ToString()),
                            RaffleId = Convert.ToInt32(sqlDataReader["IdSorteo"].ToString()),
                            ClientId = Convert.ToInt32(sqlDataReader["IdCliente"].ToString()),
                            ClientDesc = sqlDataReader["NombreCliente"].ToString(),
                            totalInvoice = Convert.ToDecimal(sqlDataReader["Cantidad"].ToString()),
                            discount = Convert.ToDecimal(sqlDataReader["Descuento"].ToString()),
                            totalQuantity = Convert.ToDecimal(sqlDataReader["CantidadTotal"].ToString()),
                            totalRestant = Convert.ToDecimal(sqlDataReader["Restante"].ToString()),
                            PaymentStatu = Convert.ToInt32(sqlDataReader["EstadoPago"].ToString()),
                            InvoiceDate = sqlDataReader["FechaFactura"].ToString(),
                            xpiredDate = sqlDataReader["FechaExpiracion"].ToString(),
                            PaymentStatuDesc = sqlDataReader["EstadoFactura"].ToString()
						};
                        lista.Add(facturas);
                    }
                }
                else
                {
                    var facturas = new ModelProcedure_InvoiceListModel()
                    {
                        Data = false,
                        Id = 0,
                        RaffleId = raffle,
                        ClientId = 0,
                        ClientDesc = "",
                        totalInvoice = 0,
                        discount = 0,
                        totalQuantity = 0,
                        totalRestant = 0,
                        InvoiceDate = "",
                        xpiredDate = "",
                        PaymentStatuDesc = "",
                        PaymentStatu = 0
                    };
                    lista.Add(facturas);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
