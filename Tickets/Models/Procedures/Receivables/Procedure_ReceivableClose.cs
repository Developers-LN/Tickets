﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures.Receivables
{
	public class Procedure_ReceivableClose
	{
		public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

		public IEnumerable<ModelProcedure_SalesAndPendingPayments> ConsultaVentasCierre(string FechaInicio, string FechaFin, int RaffleId, int ReportType)
		{
			var lista = new List<ModelProcedure_SalesAndPendingPayments>();

			DateTime FI, FF;

			using (SqlConnection sqlConnection = new SqlConnection(ConDB))
			{
                SqlCommand sqlCommand = new SqlCommand("ReceivableClose", sqlConnection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                if (FechaInicio == "undefined" && FechaInicio == "undefined")
				{
					sqlCommand.Parameters.AddWithValue("@FechaInicio", "");
					sqlCommand.Parameters.AddWithValue("@FechaFin", "");
				}
				else
				{
					FI = Convert.ToDateTime(FechaInicio);
					FF = Convert.ToDateTime(FechaFin);
					sqlCommand.Parameters.AddWithValue("@FechaInicio", FI.ToString("yyyy-MM-dd"));
					sqlCommand.Parameters.AddWithValue("@FechaFin", FF.ToString("yyyy-MM-dd"));
				}
				sqlCommand.Parameters.AddWithValue("@RaffleId", RaffleId);
				sqlConnection.Open();
				SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				if (sqlDataReader.HasRows)
				{
					while (sqlDataReader.Read())
					{
						var Devoluciones = new ModelProcedure_SalesAndPendingPayments()
						{
							Data = true,
							ReportType = ReportType,
                            IdClient = Convert.ToInt32(sqlDataReader["ID_Cliente"].ToString()),
							NameClient = sqlDataReader["Nombre_Cliente"].ToString(),
                            ReceivableStatusId = Convert.ToInt32(sqlDataReader["Id_Estado_Factura"].ToString()),
                            TypeClient = sqlDataReader["Tipo_Cliente"].ToString(),
							IdRaffle = Convert.ToInt32(sqlDataReader["ID_Sorteo"].ToString()),
							NameRaffle = sqlDataReader["Nombre_Sorteo"].ToString(),
							IdInvoice = Convert.ToInt32(sqlDataReader["ID_Factura"].ToString()),
							DateInvoice = Convert.ToDateTime(sqlDataReader["Fecha_Factura"].ToString()),
							StatusInvoice = sqlDataReader["Estado_Factura"].ToString(),
							TotalFractions = Convert.ToInt32(sqlDataReader["Total_Fracciones"].ToString()),
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
                            SequenceNumberInvoice = Convert.ToInt32(sqlDataReader["SequenceNumberInvoice"].ToString()),
                            NomenclatureRaffle = sqlDataReader["NomenclatureRaffle"].ToString(),
                            SequenceNumberRaffle = Convert.ToInt32(sqlDataReader["SequenceNumberRaffle"].ToString())
                        };
						lista.Add(Devoluciones);
					}
				}
				else
				{
					var Devoluciones = new ModelProcedure_SalesAndPendingPayments()
					{
						Data = false,
                        ReportType = ReportType,
                        IdClient = 0,
						ReceivableStatusId = 0,
						NameClient = "",
						TypeClient = "",
						IdRaffle = 0,
						NameRaffle = "",
						IdInvoice = 0,
						DateInvoice = DateTime.Now,
						StatusInvoice = "",
						TotalFractions = 0,
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
						SequenceNumberInvoice = 0,
						NomenclatureRaffle = "",
						SequenceNumberRaffle = 0
					};
					lista.Add(Devoluciones);
				}
				sqlConnection.Close();
			}
			return lista;
		}
	}
}
