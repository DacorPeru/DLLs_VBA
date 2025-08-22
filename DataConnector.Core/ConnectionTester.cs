using System;
using System.Data.SqlClient;

namespace DataConnector.Core
{
    /// <summary>
    /// Servicio mínimo para validar conectividad a SQL Server.
    /// Ejecuta "SELECT 1" y reporta:
    ///  • ok: true/false
    ///  • message: texto amigable para usuario
    ///  • technical: detalle técnico para soporte (stack/SQL)
    /// </summary>
    public static class ConnectionTester
    {
        public static (bool ok, string message, string technical) Test()
        {
            try
            {
                var cs = ConfigManager.BuildConnectionString();
                using (var cn = new SqlConnection(cs))
                {
                    cn.Open(); // Si falla, lanza SqlException / Authentication exception, etc.

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
