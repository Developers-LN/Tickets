using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class AllocatedByDayProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<object> AllocatedByDateProcedure(int allocateId)
        {
            var lista = new List<object>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("AllocatedByDate", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@AllocationId", allocateId);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Allocated = new AllocatedByDate()
                        {
                            Data = true,
                            AllocateDate = sqlDataReader["PrintDate"].ToString(),
                            TotalTickets = Convert.ToInt32(sqlDataReader["TotalTickets"].ToString()),
                            TicketPrice = Convert.ToDecimal(sqlDataReader["TicketPrice"].ToString()),
                            MountSold = Convert.ToDecimal(sqlDataReader["MountSold"].ToString()),
                            Discount = Convert.ToDecimal(sqlDataReader["Discount"].ToString()),
                            SubTotal = Convert.ToDecimal(sqlDataReader["SubTotal"].ToString()),
                            Total = Convert.ToDecimal(sqlDataReader["Total"].ToString())
                        };
                        lista.Add(Allocated);
                    }
                }
                else
                {
                    var Allocated = new AllocatedByDate()
                    {
                        Data = true,
                        AllocateDate = "",
                        TotalTickets = 0,
                        TicketPrice = 0.0m,
                        MountSold = 0.0m,
                        Discount = 0.0m,
                        SubTotal = 0.0m,
                        Total = 0.0m
                    };
                    lista.Add(Allocated);
                }
                sqlConnection.Close();
            }
            return lista;
        }
    }
}
