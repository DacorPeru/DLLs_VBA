using System;
using System.Data.SqlClient;

namespace DataConnector.SqlServer
{
    public class DBCompanyManager
    {
        public DBResult GetConnectionString(string masterConn, string companyId)
        {
            DBResult result = new DBResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(masterConn))
                {
                    conn.Open();
                    string query = "SELECT ConnectionString FROM Empresas WHERE EmpresaID = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", companyId);
                        object res = cmd.ExecuteScalar();
                        result.Success = res != null;
                        result.Message = res != null ? "Conexión obtenida correctamente" : "No se encontró la empresa";
                        result.Data = res?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error obteniendo cadena: " + ex.Message;
            }
            return result;
        }
    }
}
