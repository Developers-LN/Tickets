﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures.IdentifiBach;

namespace Tickets.Models.Procedures.IdentifyBach
{
    public class Procedure_IdentifyBachPayent
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<object> GetIdentifyBachPayment(int Bach)
        {
            var IdentifyBachPayment = new List<object>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("GetIdentifyBachPayment", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Bach", Bach);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var bachPayment = new ModelProcedure_IdentifyBachPayment()
                        {
                            Data = true,
                            Id = Convert.ToInt32(sqlDataReader["Id"].ToString()),
                            Name = sqlDataReader["Name"].ToString(),
                            Nota = sqlDataReader["Nota"].ToString(),
                            Value = Convert.ToDecimal(sqlDataReader["Value"].ToString())
                        };
                        IdentifyBachPayment.Add(bachPayment);
                    }
                }
                else
                {
                    var bachPayment = new ModelProcedure_IdentifyBachPayment()
                    {
                        Data = false,
                        Id = 0,
                        Name = "",
                        Nota = "",
                        Value = 0
                    };
                    IdentifyBachPayment.Add(bachPayment);
                }
                sqlConnection.Close();
            }
            return IdentifyBachPayment;
        }
    }
}
