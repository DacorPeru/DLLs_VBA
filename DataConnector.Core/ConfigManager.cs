using System;
using System.IO;
using Newtonsoft.Json;

namespace DataConnector.Core
{
    /// <summary>
    /// Modelo de configuración básico para UNA base (central).
    /// Si AuthMode = "SQL" se requiere User y PasswordDpapi (cifrada con DPAPI).
    /// </summary>
    public sealed class AppConfig
    {
        public string Server { get; set; }                 // ej: SRV-SQL\INSTANCIA
        public string Database { get; set; }               // ej: ERP_Central
        public string AuthMode { get; set; } = "SQL";      // "SQL" | "Windows"
        public string User { get; set; }                   // requerido si AuthMode = SQL
        public string PasswordDpapi { get; set; }          // Base64 protegido con DPAPI
        public bool Encrypt { get; set; } = true;          // TLS/SSL
        public bool TrustServerCertificate { get; set; } = false; // true solo si certificados no confiables
        public int ConnectTimeoutMs { get; set; } = 15000; // timeout en milisegundos
    }

    /// <summary>
    /// Carga y mantiene la configuración en memoria, y construye la connection string.
    /// Responsabilidades:
    ///  • Cargar JSON desde disco.
    ///  • Validar campos mínimos.
    ///  • Desproteger la contraseña (DPAPI) en memoria si AuthMode = SQL.
    ///  • Exponer BuildConnectionString() para los servicios de Core.
    /// </summary>
    public static class ConfigManager
    {
        private static AppConfig _cfg;
        private static readonly object _lock = new object();

        /// <summary>
        /// Carga el JSON desde la ruta indicada. Lanzará excepción si falta o es inválido.
        /// </summary>
        public static void LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config no encontrado: {path}");

            var json = File.ReadAllText(path);
            var cfg = JsonConvert.DeserializeObject<AppConfig>(json)
                      ?? throw new InvalidOperationException("Config inválida (JSON nulo).");

            // Si se usa autenticación SQL, descifra PasswordDpapi en memoria.
            if (string.Equals(cfg.AuthMode, "SQL", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(cfg.User))
                    throw new InvalidOperationException("User faltante para AuthMode=SQL.");
                if (string.IsNullOrWhiteSpace(cfg.PasswordDpapi))
                    throw new InvalidOperationException("PasswordDpapi faltante para AuthMode=SQL.");

                cfg.PasswordDpapi = SecureString.Unprotect(cfg.PasswordDpapi); // queda en claro solo en RAM
            }

            lock (_lock) _cfg = cfg;
        }

        /// <summary>
        /// Devuelve la configuración actual; lanza si aún no se cargó.
        /// </summary>
        public static AppConfig Current
        {
            get
            {
                lock (_lock)
                    return _cfg ?? throw new InvalidOperationException("Config no cargada. Llama primero a LoadFromFile().");
            }
        }

        /// <summary>
        /// Construye la cadena de conexión de manera consistente con la política de seguridad.
        /// </summary>
        public static string BuildConnectionString()
        {
            var c = Current; // asegura que esté cargada
            var sb = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                DataSource = c.Server,
                InitialCatalog = c.Database,
                ConnectTimeout = Math.Max(1, c.ConnectTimeoutMs / 1000),
                Encrypt = c.Encrypt,
                TrustServerCertificate = c.TrustServerCertificate
            };

            if (string.Equals(c.AuthMode, "Windows", StringComparison.OrdinalIgnoreCase))
            {
                sb.IntegratedSecurity = true; // usa credenciales del proceso (usuario actual)
            }
            else
            {
                sb.IntegratedSecurity = false;
                sb.UserID = c.User;
                sb.Password = c.PasswordDpapi; // ya descifrada en LoadFromFile
            }

            return sb.ConnectionString;
        }

        /// <summary>
        /// Utilidad para generar un Base64 protegido (DPAPI LocalMachine). Úsalo una sola vez para armar el JSON.
        /// </summary>
        public static string GeneratePasswordDpapi(string plainPassword) => SecureString.Protect(plainPassword);
    }
}
