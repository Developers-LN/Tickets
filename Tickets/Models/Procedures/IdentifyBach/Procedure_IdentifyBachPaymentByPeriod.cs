using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using Tickets.Models.ModelsProcedures.IdentifiBach;
using DocumentFormat.OpenXml.Bibliography;

namespace Tickets.Models.Procedures.IdentifyBach
{
    public class Procedure_IdentifyBachPaymentByPeriod
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_IdentifyBachPaymentByPeriod> IdentifyBachPaymentByPeriod(int RaffleId, string StartDate, string EndDate)
        {
            var IdentifyBachPayment = new List<ModelProcedure_IdentifyBachPaymentByPeriod>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("IdenfityBachPayedByPeriod", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@StartDate", StartDate);
                sqlCommand.Parameters.AddWithValue("@EndDate", EndDate);
                sqlCommand.Parameters.AddWithValue("@RaffleId", RaffleId);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var bachPayment = new ModelProcedure_IdentifyBachPaymentByPeriod()
                        {
                            Data = true,
                            DocumentNumber = sqlDataReader["DocumentNumber"].ToString(),
                            DocumentType = sqlDataReader["DocumentType"].ToString(),
                            Winner = sqlDataReader["Winner"].ToString(),
                            Genre = sqlDataReader["Genre"].ToString(),
                            RaffleId = Convert.ToInt32(sqlDataReader["RaffleId"].ToString()),
                            BachId = Convert.ToInt32(sqlDataReader["BachId"].ToString()),
                            PaymentDate = sqlDataReader["PaymentDate"].ToString(),
                            PaymentType = sqlDataReader["PaymentType"].ToString(),
                            Day = Convert.ToInt32(sqlDataReader["Day"].ToString()),
                            Month = Convert.ToInt32(sqlDataReader["Month"].ToString()),
                            Year = Convert.ToInt32(sqlDataReader["Year"].ToString()),
                            TotalPayed = Convert.ToDecimal(sqlDataReader["TotalPayed"].ToString()),
                            PayedFractions = Convert.ToInt32(sqlDataReader["PayedFractions"].ToString()),
                            GenreId = Convert.ToInt32(sqlDataReader["GenreId"].ToString()),
                        };
                        IdentifyBachPayment.Add(bachPayment);
                    }
                }
                else
                {
                    var bachPayment = new ModelProcedure_IdentifyBachPaymentByPeriod()
                    {
                        Data = false,
                        DocumentNumber = "",
                        DocumentType = "",
                        Winner = "",
                        Genre = "",
                        RaffleId = 0,
                        BachId = 0,
                        PaymentDate = "",
                        PaymentType = "",
                        Day = 0,
                        Month = 0,
                        Year = 0,
                        TotalPayed = 0.0m,
                        PayedFractions = 0,
                        GenreId = 0
                    };
                    IdentifyBachPayment.Add(bachPayment);
                }
                sqlConnection.Close();
            }
            return IdentifyBachPayment;
        }
    }
}
