using System;
using System.Data.SqlClient;

namespace DataConnector.Core
{
    // ▼ HAZLO PUBLIC para que el proyecto COM pueda llamarlo
    public static class ConnectionTester
    {
        public static (bool ok, string message, string technical) Test()
        {
            try
            {
                using (var cn = new SqlConnection(ConfigManager.BuildConnectionString()))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", cn))
                    {
                        var r = cmd.ExecuteScalar();
                        return (true, "Conexión establecida.", $"SELECT1={r}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "No se pudo establecer conexión.", ex.ToString());
            }
        }
    }
}
