using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class SalesAndPendingPayments
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelSalesAndPendingPayments> ConsultaVentasCuentasPendientes(string FechaInicio, string FechaFin)
        {
            var lista = new List<ModelSalesAndPendingPayments>();

            DateTime FI = Convert.ToDateTime(FechaInicio);
            DateTime FF = Convert.ToDateTime(FechaFin);

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("SalesAndPendingPayments", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@FechaInicio", FI.ToString("yyyy-MM-dd"));
                sqlCommand.Parameters.AddWithValue("@FechaFin", FF.ToString("yyyy-MM-dd"));
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Devoluciones = new ModelSalesAndPendingPayments()
                        {
                            Data = true,
                            IdClient = Convert.ToInt32(sqlDataReader["ID_Cliente"].ToString()),
                            NameClient = sqlDataReader["Nombre_Cliente"].ToString(),
                            TypeClient = sqlDataReader["Tipo_Cliente"].ToString(),
                            IdRaffle = Convert.ToInt32(sqlDataReader["ID_Sorteo"].ToString()),
                            NameRaffle = sqlDataReader["Nombre_Sorteo"].ToString(),
                            TicketReturn = Convert.ToInt32(sqlDataReader["Tickets_Devueltos"].ToString()),
                            FractionReturn = Convert.ToInt32(sqlDataReader["Fracciones_Devueltas"].ToString()),
                            IdInvoice = Convert.ToInt32(sqlDataReader["ID_Factura"].ToString()),
                            DateInvoice = Convert.ToDateTime(sqlDataReader["Fecha_Factura"].ToString()),
                            StatusInvoice = sqlDataReader["Estado_Factura"].ToString(),
                            TotalTickets = Convert.ToInt32(sqlDataReader["Total_Billetes"].ToString()),
                            PriceTicket = Convert.ToDecimal(sqlDataReader["Precio_Billete"].ToString()),
                            TotalInvoice = Convert.ToDecimal(sqlDataReader["Total_Factura"].ToString()),
                            DiscountPercent = Convert.ToDecimal(sqlDataReader["Porcentaje_Descuento"].ToString()),
                            TotalDiscount = Convert.ToDecimal(sqlDataReader["Total_Descuento"].ToString()),
                            TotalToPay = Convert.ToDecimal(sqlDataReader["Total_A_Pagar"].ToString()),
                            CashPayment = Convert.ToDecimal(sqlDataReader["Pagos_Efectivo"].ToString()),
                            NoteCreditPayment = Convert.ToDecimal(sqlDataReader["Pagos_Credito"].ToString()),
                            TotalPayed = Convert.ToDecimal(sqlDataReader["Total_Pagado"].ToString()),
                            TotalPending = Convert.ToDecimal(sqlDataReader["Total_Faltante"].ToString()),
                        };
                        lista.Add(Devoluciones);
                    }
                }
                else
                {
                    var Devoluciones = new ModelSalesAndPendingPayments()
                    {
                        Data = false,
                        IdClient = 0,
                        NameClient = "",
                        TypeClient = "",
                        IdRaffle = 0,
                        NameRaffle = "",
                        TicketReturn = 0,
                        FractionReturn = 0,
                        IdInvoice = 0,
                        DateInvoice = DateTime.Now,
                        StatusInvoice = "",
                        TotalTickets = 0,
                        PriceTicket = 0,
                        TotalInvoice = 0,
                        DiscountPercent = 0,
                        TotalDiscount = 0,
                        TotalToPay = 0,
                        CashPayment = 0,
                        NoteCreditPayment = 0,
                        TotalPayed = 0,
                        TotalPending = 0,
                    };
                    lista.Add(Devoluciones);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}