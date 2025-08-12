using System;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace DataConnector.SqlServer
{
    public class DBQueryExecutor
    {
        private readonly DBConnectionManager _connManager;

        public DBQueryExecutor(DBConnectionManager connManager)
        {
            _connManager = connManager;
        }

        public DBResult ExecuteQuery(string sql)
        {
            DBResult result = new DBResult();
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, _connManager.GetConnection()))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    result.Success = true;
                    result.Message = "Consulta ejecutada correctamente";
                    result.Data = JsonConvert.SerializeObject(dt);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error ejecutando consulta: " + ex.Message;
            }
            return result;
        }

        public DBResult ExecuteNonQuery(string sql)
        {
            DBResult result = new DBResult();
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, _connManager.GetConnection()))
                {
                    int rows = cmd.ExecuteNonQuery();
                    result.Success = true;
                    result.RowsAffected = rows;
                    result.Message = "Operación ejecutada correctamente";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error ejecutando operación: " + ex.Message;
            }
            return result;
        }
    }
}
