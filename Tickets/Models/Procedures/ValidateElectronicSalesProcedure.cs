using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tickets.Models.ModelsProcedures;

namespace Tickets.Models.Procedures
{
    public class ValidateElectronicSalesProcedure
    {
        public string ConDB = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public IEnumerable<ModelProcedure_ValidateElectronicSale> ValidateElectronicSales(int AllocationId, int CreateUser)
        {
            var resultado = new List<ModelProcedure_ValidateElectronicSale>();

            using (SqlConnection sqlConnection = new SqlConnection(ConDB))
            {
                SqlCommand sqlCommand = new SqlCommand("GenerateElectronicReturns", sqlConnection);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@AllocationId", AllocationId);
                sqlCommand.Parameters.AddWithValue("@CreateUser", CreateUser);
                sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        var Validate = new ModelProcedure_ValidateElectronicSale()
                        {
                            Statu = Convert.ToInt32(sqlDataReader["Statu"].ToString()),
                            Mensaje = sqlDataReader["Mensaje"].ToString()
                        };
                        resultado.Add(Validate);
                    }
                }
                else
                {
                    var Validate = new ModelProcedure_ValidateElectronicSale()
                    {
                        Statu = 0,
                        Mensaje = "Error al intentar validar la venta electrónica"
                    };
                    resultado.Add(Validate);
                }
                sqlConnection.Close();
            }
            return resultado;
        }
    }
}
