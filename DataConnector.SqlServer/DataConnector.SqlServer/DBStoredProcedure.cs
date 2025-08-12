using System;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataConnector.SqlServer
{
    public class DBStoredProcedure
    {
        private readonly DBConnectionManager _connManager;

        public DBStoredProcedure(DBConnectionManager connManager)
        {
            _connManager = connManager;
        }

        public DBResult Execute(string procedureName, string parametersJson)
        {
            DBResult result = new DBResult();
            try
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, _connManager.GetConnection()))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (!string.IsNullOrWhiteSpace(parametersJson))
                    {
                        var jsonParams = JObject.Parse(parametersJson);
                        foreach (var kv in jsonParams)
                        {
                            cmd.Parameters.AddWithValue(
                                "@" + kv.Key,
                                kv.Value == null || kv.Value.Type == JTokenType.Null
                                    ? (object)DBNull.Value
                                    : (object)kv.Value.ToString()
                            );

                        }
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        result.Success = true;
                        result.Message = "Procedimiento ejecutado correctamente";
                        result.Data = JsonConvert.SerializeObject(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error ejecutando procedimiento: " + ex.Message;
            }
            return result;
        }
    }
}
